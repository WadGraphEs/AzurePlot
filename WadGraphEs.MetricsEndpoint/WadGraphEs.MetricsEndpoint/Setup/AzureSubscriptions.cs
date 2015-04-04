using Mono.Security.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AzureSubscriptions {
		internal static void StorePfxForSession(string sessionId,byte[] pfx,string password) {
			var dataContext = GetDataContext();

			var session = dataContext.AddAzureSubscriptionSessions.Find(sessionId);
			if(session == null) {
				session = new AddAzureSubscriptionSession {
					SessionId = sessionId
				};
				dataContext.AddAzureSubscriptionSessions.Add(session);
			}
			
			session.Pfx = pfx;
			session.Password = password;

			dataContext.SaveChanges();
		}

		private static DataContext GetDataContext() {
			return new DataContext();
		}

		internal static byte[] GetCertificateForSession(string sessionId) {
			var record = GetSessionRecord(sessionId);

			return X509Tools.GenerateCertificate.GetCertificateForBytes(record.Pfx,record.Password);
		}

		private static AddAzureSubscriptionSession GetSessionRecord(string sessionId) {
			var record = GetDataContext().AddAzureSubscriptionSessions.Find(sessionId);
			if(record==null) {
				throw new KeyNotFoundException("Session not found");
			}
			return record;
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
			var context = GetDataContext();
			var record = context.AddAzureSubscriptionSessions.Find(sessionId);
			record.AzureSubscriptionId = subscriptionId;
			context.SaveChanges();
		}

		internal static void Handle(FinishAddingAzureSubscription cmd) {
			var context = GetDataContext();

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
		}

		internal static ICollection<ServiceViewModel> ListForOverview() {
			var ctx = GetDataContext();

			return ctx.AzureSubscriptions.ToList().Select(_=>new ServiceViewModel {
				//Id = _.Id,
				Name = _.FormatName(),
				Record = _
			}).ToList();
		}


		internal static ICollection<AzureSubscription> ListAll() {
			return GetDataContext().AzureSubscriptions.ToList();
		}
	}
}