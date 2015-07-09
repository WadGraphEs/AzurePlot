using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WadGraphEs.MetricsEndpoint.ApiControllers;
using WadGraphEs.MetricsEndpoint.DataAccess;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AzureSubscriptions {
		internal static void StorePfxForSession(string sessionId,byte[] pfx,string password) {
			DataContext.Do(ctx=>{
			    var session = ctx.AddAzureSubscriptionSessions.Find(sessionId);
			    if(session == null) {
				    session = new AddAzureSubscriptionSession {
					    SessionId = sessionId
				    };
				    ctx.AddAzureSubscriptionSessions.Add(session);
			    }
			
			    session.Pfx = pfx;
			    session.Password = password;

			    ctx.SaveChanges();
            });
		}

		
		internal static byte[] GetCertificateForSession(string sessionId) {
			var record = GetSessionRecord(sessionId);

			return X509Tools.GenerateCertificate.GetCertificateForBytes(record.Pfx,record.Password);
		}

		private static AddAzureSubscriptionSession GetSessionRecord(string sessionId) {
            return DataContext.Do(ctx=>{
			    var record = ctx.AddAzureSubscriptionSessions.Find(sessionId);
			    if(record==null) {
				    throw new KeyNotFoundException("Session not found");
			    }
			    return record;
            });
		}

		internal static ICollection<Exception> TestConnection(string sessionId,string azureSubscriptionId) {
			try {
				var record = GetSessionRecord(sessionId);

				var usageClient = GetAzureUsageClient(azureSubscriptionId,record);

				usageClient.TestConnection();
				
				return new Exception[0];
			}
			catch(Exception e) {
				return new [] { e};
			}			
		}

		
		private static AzureUsageClient GetAzureUsageClient(string azureSubscriptionId,AddAzureSubscriptionSession record) {
			return new AzureUsageClient(new FromPKCSMetricsEndpointConfiguration(record.Pfx,record.Password,azureSubscriptionId));
		}

		internal static FinishAddingAzureSubscription CreateFinishCommandForSession(string sessionId) {
			var record = GetSessionRecord(sessionId);
			var client = GetAzureUsageClient(record.AzureSubscriptionId,record);
			return new FinishAddingAzureSubscription {
				AzureSubscriptionName = client.GetSubscriptionNameSync(),
				SessionId = sessionId,
				AzureSubscriptionId = record.AzureSubscriptionId
			};
		}

		internal static void AddAzureSubscriptionIdForSession(string sessionId,string subscriptionId) {
			DataContext.Do(context=>{
			    var record = context.AddAzureSubscriptionSessions.Find(sessionId);
			    record.AzureSubscriptionId = subscriptionId;
			    context.SaveChanges();
            });
		}

		internal static void Handle(FinishAddingAzureSubscription cmd) {
            DataContext.Do(context=>{
			    var session = context.AddAzureSubscriptionSessions.Find(cmd.SessionId);

			    context.AzureSubscriptions.Add(new AzureSubscription {
				    Name = cmd.AzureSubscriptionName,
				    AzureSubscriptionId  = session.AzureSubscriptionId,
				    AddedOnUtc = DateTime.UtcNow,
				    FromSessionId = cmd.SessionId,
				    Pfx = session.Pfx,
				    Password= session.Password,
			    });

			    context.AddAzureSubscriptionSessions.Remove(session);

			    context.SaveChanges();
            });
		}

		internal static ICollection<ServiceViewModel> ListForOverview() {
			return DataContext.Do(ctx=>{
			    return ctx.AzureSubscriptions.ToList().Select(_=>new ServiceViewModel {
				    //Id = _.Id,
				    Name = _.FormatName(),
				    Record = _
			    }).ToList();
            });
		}


		internal static ICollection<AzureSubscription> ListAll() {
			return DataContext.Do(ctx=>ctx.AzureSubscriptions.ToList());
		}

		internal static async Task<ICollection<ChartInfo>> ListAllCharts() {
			var result = await Task.WhenAll(ListAll().Select(ListAllCharts));
			return result.SelectMany(_=>_).ToList();
              
		}

		private static Task<ICollection<ChartInfo>> ListAllCharts(AzureSubscription subscription) {
            return ChartsFacade.ListAllChartsForSubscription(subscription.GetMetricsConfig(),subscription.FormatName());
			
		}

		internal static Task<ChartData> GetChartData(string forUri) {
            var fc = new ChartDataFacade(forUri);

            fc.SubscriptionCredentialsProvider = s => GetSubscriptionById(s).GetMetricsConfig();

            return fc.FetchChartData();

			
		}

       

		

		private static AzureSubscription GetSubscriptionById(string subscriptionId) {
			return DataContext.Do(ctx=>ctx.AzureSubscriptions.ToList().FirstOrDefault(_=>_.AzureSubscriptionId == subscriptionId));
		}

        internal static MetricsEndpointConfiguration GetCredentials(string forSubscriptionId) {
            var subscription = GetSubscriptionById(forSubscriptionId);
            if(subscription == null) {
                throw new InvalidOperationException("Couldn't fetch credentiasl for subscription " + forSubscriptionId);
            }

            return subscription.GetMetricsConfig();
        }
    }
}