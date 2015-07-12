using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlot.Lib {
	class AzureWebsiteUsage {
		
		readonly AzureWebsiteId _websiteId;
		readonly AzureWebsitesMetricsApiClient _websitesMetricsClient;
		readonly AzureMetricsApiClient _azureMetricsApiClient;

		public AzureWebsiteUsage(AzureWebsiteId websiteId,AzureMetricsApiClient metricsApiClient,AzureWebsitesMetricsApiClient azureWebsitesMetricsClient) {
			_websiteId = websiteId;
			_azureMetricsApiClient =metricsApiClient;
			_websitesMetricsClient = azureWebsitesMetricsClient;
		}
		
		//public string Name { get; set; }

		//public string Webspace { get; set; }

	

		internal async Task<ICollection<UsageObject>> GetUsage() {
			var fromAPI = GetWebsiteApiUsage();
			var fromMetrics = GetMetricsApiUsage(TimeSpan.FromMinutes(15));

			return (await fromAPI)
				.Concat(await fromMetrics)
				.ToList();
		}

		

		private async Task<ICollection<UsageObject>> GetWebsiteApiUsage() {
            var websiteUsage= await _websitesMetricsClient.GetWebsiteUsage(_websiteId);
			 

			return websiteUsage.Select(_=>_.ToUsageObjects(_websiteId)).SelectMany(_=>_).ToList();

        }

		private async Task<ICollection<UsageObject>> GetMetricsApiUsage(TimeSpan history) {
			var metrics = await _azureMetricsApiClient.GetMetricsForWebsite(_websiteId,history);
			var res = new List<UsageObject>();

			foreach(var metric in metrics) {
				foreach(var result in metric.MetricValues.OrderBy(_=>_.Timestamp)) {
					res.Add(new UsageObject {
						GraphiteCounterName =_websiteId.BuildGraphiteCounterName(WebsiteMetricsDataSource.MetricsApi, metric.Name,metric.Unit).ToString(),
						Value = result.Average.Value,
						Timestamp = result.Timestamp.ToString("o")
					});
				}
			}
			return res;
		}

		private string NormalizeMetricName(string name) {
			return name.Replace(" ", "");
		}




		
	}
}
