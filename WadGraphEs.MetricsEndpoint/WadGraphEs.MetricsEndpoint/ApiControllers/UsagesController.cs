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
using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class UsagesController:ApiController {
		readonly static Logger _logger = LogManager.GetCurrentClassLogger(); 
		public async Task<IEnumerable<UsageObject>> Get() {
			_logger.Info("Getting usages");

			try {
				var tasks = AzureSubscriptions.ListAll().Select(GetUsageForSubscription);
				
				var syncResults = GetSQLDatabaseUsages();

				var asyncResults = await Task.WhenAll(tasks);
				return new [] { new UsageObject { 
					GraphiteCounterName = new GraphiteCounterName("WadGraphEs.Diagnostics.Proxy.TimeOfDay").ToString(),
					Timestamp = DateTime.UtcNow.ToString("o"),
					Value = DateTime.UtcNow.TimeOfDay.TotalSeconds
				}}.Concat(asyncResults.SelectMany(_=>_)).Concat(syncResults).ToList();
			}
			catch(Exception e) {
				_logger.ErrorException("Error getting usages", e);
				throw;
			}
		}

		private ICollection<UsageObject> GetSQLDatabaseUsages() {
			var result = new List<UsageObject>();
			foreach(var database in AzureSQLDatabases.ListAll()) {
				try {
					result.AddRange(GetDatabaseUsages(database));
				}
				catch(Exception e) {
					_logger.ErrorException(string.Format("Failed fetch metrics for {0}", database.Servername),e);
				}
			}
			return result;
		}

		private ICollection<UsageObject> GetDatabaseUsages(DataAccess.SQLDatabase database) {
			return SQLDatabaseUsageClient.CreateServerUsagesClient(database.Servername,database.Username,database.Password).GetUsages(DateTime.Today.ToUniversalTime());
		}

		private async Task<IEnumerable<UsageObject>> GetUsageForSubscription(DataAccess.AzureSubscription subscription) {
			var azureUsageService = new AzureUsageClient(subscription.GetMetricsConfig());

			var websiteUsage = azureUsageService.GetWebsitesUsage();
			var cloudServiceUsage = azureUsageService.GetCloudServiceUsages();

			return (await websiteUsage).Concat(await cloudServiceUsage);
		}
	}
}