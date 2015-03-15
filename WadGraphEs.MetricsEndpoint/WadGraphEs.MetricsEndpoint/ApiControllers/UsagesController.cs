using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Xml;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using WadGraphEs.MetricsEndpoint.Lib;
using NLog;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class UsagesController:ApiController {
		readonly static Logger _logger = LogManager.GetCurrentClassLogger(); 
		public async Task<IEnumerable<UsageObject>> Get() {
			_logger.Info("Getting usages");

			try {
				var tasks = AzureSubscriptions.ListAll().Select(GetUsageForSubscription);
			
				var results = await Task.WhenAll(tasks);
				return new [] { new UsageObject { 
					GraphiteCounterName = new GraphiteCounterName("WadGraphEs.Diagnostics.Proxy.TimeOfDay").ToString(),
					Timestamp = DateTime.UtcNow.ToString("o"),
					Value = DateTime.UtcNow.TimeOfDay.TotalSeconds
				}}.Concat(results.SelectMany(_=>_)).ToList();
			}
			catch(Exception e) {
				_logger.ErrorException("Error getting usages", e);
				throw;
			}
		}

		private async Task<IEnumerable<UsageObject>> GetUsageForSubscription(DataAccess.AzureSubscription subscription) {
			var azureUsageService = new AzureUsageClient(subscription.GetMetricsConfig());

			var websiteUsage = azureUsageService.GetWebsitesUsage();
			var cloudServiceUsage = azureUsageService.GetCloudServiceUsages();

			return (await websiteUsage).Concat(await cloudServiceUsage);
		}
	}
}