using Mono.Security.X509;
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

		internal static async Task<ICollection<ApiControllers.ChartInfo>> ListAllCharts() {
			var result = await Task.WhenAll(ListAll().Select(ListAllCharts));
			return result.SelectMany(_=>_).ToList();
		}

		private static async Task<ICollection<ChartInfo>> ListAllCharts(AzureSubscription subscription) {
			var infoClient = new AzureSubscriptionInfoClient(subscription.GetMetricsConfig());
			
			var websites = await infoClient.ListWebsites();

			return websites.SelectMany(_=>GetWebsiteCharts(subscription,_)).ToList();
		}

		private static ICollection<ChartInfo> GetWebsiteCharts(AzureSubscription subscription, AzureWebsite website) {
			Func<string,string,ChartInfo> initChartInfo = (name,uri)=>
				new ChartInfo {
					ResourceName = website.Name,
					ResourceType = "website",
					ServiceName = subscription.FormatName(),
					ServiceType = "Azure Subscription",
					Name = string.Format("{0} {1}", website.Name,name),
					Uri = uri
				};
			return new ChartInfo[] {
				initChartInfo("Requests", website.Uri.ToString()+"/requests"),
				initChartInfo("Memory", website.Uri.ToString()+"/memory"),
				initChartInfo("CPU", website.Uri.ToString()+"/cpu"),
				initChartInfo("Traffic", website.Uri.ToString()+"/traffic"),
			};
			//w=>new ChartInfo {
			//	Uri = w.Uri.ToString(),
			//	Name = w.Name,
			//	Service = subscription.FormatName(),
			//	ServiceType = "Azure Subscription"				
			//}
		}

		internal static Task<ChartData> GetChartData(string forUri) {
			var uri = new Uri(forUri);
			var subscription = GetSubscriptionById(uri.Host);
			var path = uri.LocalPath.Split(new [] {'/'},StringSplitOptions.RemoveEmptyEntries);
			if(path[0]!="websites") {
				throw new Exception("don't know how to handle " + path[0]);
			}
			var webspace = path[1];
			var websiteName = path[2];
			var counter = path[3];
			switch(counter) {
				case "requests":
					return GetWebsiteRequests(subscription,webspace,websiteName);
				case "cpu":
					return GetWebsiteCPU(subscription,webspace,websiteName);
				default:
					throw new Exception("Don't know how to get " + counter);
			}
		}

		private static Task<ChartData> GetWebsiteCPU(AzureSubscription subscription,string webspace,string websiteName) {
			return GetWebsiteUsages(subscription,webspace,websiteName,x=>x,string.Format("{0} CPU",websiteName), "^CpuTime");
		}

		private static Task<ChartData> GetWebsiteRequests(AzureSubscription subscription,string webspace,string websiteName) {
			return GetWebsiteUsages(subscription,webspace,websiteName,x=>x.Replace(".Count",""),string.Format("{0} requests", websiteName),"^Http", "^Requests");
		}

		private static async Task<ChartData> GetWebsiteUsages(AzureSubscription subscription, string webspace,string websiteName,Func<string,string> formatSeries,string charttitle,params string[] filters) {
			var usageClient = new AzureUsageClient(subscription.GetMetricsConfig());
			var usages = await usageClient.GetWebsitesUsageForWebsite(webspace,websiteName,TimeSpan.FromHours(1),filters);
			return new ChartData {
				Name = charttitle,
				Series = usages.GroupBy(_ => _.GraphiteCounterName).Select(_ =>
					new SeriesData {
						Name = formatSeries(_.Key),
						DataPoints = _.Select(dp => new DataPoint { Timestamp = dp.Timestamp,Value = dp.Value }).ToList()
					}
				).ToList()
			};
		}

		private static AzureSubscription GetSubscriptionById(string subscriptionId) {
			return GetDataContext().AzureSubscriptions.ToList().FirstOrDefault(_=>_.AzureSubscriptionId == subscriptionId);
		}
	}
}