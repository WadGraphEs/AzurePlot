using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureMetricsApiClient {
		readonly MetricsClient _metricsClient;
		public AzureMetricsApiClient(CertificateCloudCredentials credentials) {
			_metricsClient = new MetricsClient(credentials);
		}

        internal System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForWebsite(AzureWebsiteId websiteId, TimeSpan history) {
            return GetMetricsForWebsite(websiteId,history,MetricsFilter.None);
        }

		internal async System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForWebsite(AzureWebsiteId websiteId, TimeSpan history,MetricsFilter filter) {
			
			return await GetMetricsForResourceId(websiteId.ResourceId,history,filter);
		}

        //the limit seems to be 2880 metrics per request
		async System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForResourceId(string resourceId, TimeSpan forHistory, MetricsFilter filter) {
            var metricsResult = await _metricsClient.MetricDefinitions.ListAsync(resourceId,null,null);

			var metrics = filter.FilterMetrics(metricsResult.MetricDefinitionCollection.Value.Where(_=>_.MetricAvailabilities.Any()).ToList());
            
			if(!metrics.Any()) {
				return new MetricValueSet[0];
			}

            var minTimeGrain = metrics.SelectMany(_=>_.MetricAvailabilities.Select(a=>a.TimeGrain)).Min();

			var metricNames = metrics.Select(_=>_.Name).ToList();

			var values = await _metricsClient.MetricValues.ListAsync(
				resourceId, 
				metricNames,
				"",
				minTimeGrain,
				DateTime.UtcNow.Add(forHistory.Negate()),
				DateTime.UtcNow);
								

			//var res = await _client.GETJson(string.Format("/services/monitoring/metricdefinitions/query?resourceId={0}", Uri.EscapeDataString(resourceId)),apiVersion:ApiVersion);

			return values.MetricValueSetCollection.Value;
		}

		const string ApiVersion = "2013-10-01";


		internal async System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForCloudService(CloudServiceInstanceId instance,TimeSpan history) {
			return await GetMetricsForResourceId(instance.ResourceId,history, MetricsFilter.None);
		}
	}
}
