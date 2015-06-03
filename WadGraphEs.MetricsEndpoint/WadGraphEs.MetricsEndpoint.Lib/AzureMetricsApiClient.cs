using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var minTimeGrain = metrics.SelectMany(_=>_.MetricAvailabilities.Where(ma=>forHistory<ma.Retention).Select(a=>a.TimeGrain)).Min();

			var metricNames = metrics.Select(_=>_.Name).ToList();

            var till = DateTime.UtcNow.AddMinutes(1);
            var from = till.Add(forHistory.Negate());

                
			return await Partition(resourceId,metricNames,minTimeGrain,from,till);
		}

        private async Task<ICollection<MetricValueSet>> Partition(string resourceId,List<string> metricNames,TimeSpan timeGrain,DateTime from,DateTime till) {
            try {
                var values = await _metricsClient.MetricValues.ListAsync(
				    resourceId, 
				    metricNames,
                    "",
				    timeGrain,
				    from,
				    till);
			
			    return values.MetricValueSetCollection.Value;
            }
            catch(CloudException e) {
                if(e.Response.StatusCode == System.Net.HttpStatusCode.BadRequest && e.Message.Contains(TooManyValuesErrorMessage)) {
                    if(CanStillPartition(metricNames,timeGrain,from,till)) {
                        throw;
                    }
                }
                throw;
            }
        }

        private static bool CanStillPartition(List<string> metricNames,TimeSpan timeGrain,DateTime from,DateTime till) {
            return metricNames.Count * ((from-till).TotalSeconds / timeGrain.TotalSeconds) >= 100;
        }

        const string TooManyValuesErrorMessage = "You've exceeded the maximum number of allowed values per metric in a single request.";

		const string ApiVersion = "2013-10-01";


		internal async System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForCloudService(CloudServiceInstanceId instance,TimeSpan history) {
			return await GetMetricsForResourceId(instance.ResourceId,history, MetricsFilter.None);
		}
	}
}
