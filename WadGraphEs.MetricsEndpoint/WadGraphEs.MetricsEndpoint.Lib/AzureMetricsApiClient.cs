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


			return await Fetch(new FetchData {
                ResourceId = resourceId,
                MetricNames = metricNames,
                TimeGrain = minTimeGrain,
                From = from,
                Till = till
            });
		}

        class FetchData {
            public string ResourceId{get;set;}
            public List<string> MetricNames{get;set;}
            public TimeSpan TimeGrain{get;set;}
            public DateTime From{get;set;}
            public DateTime Till{get;set;}
                
            public bool PartitionTresholdReached() {
                return MetricNames.Count * ((From-Till).TotalSeconds / TimeGrain.TotalSeconds) >= MinDataPoints;
            }

            const int MinDataPoints = 100;

            internal ICollection<FetchData> Partition() {
                //TODO: maybe partition on MetricNames first, but not sure if that's gonna affect minimal timegrain
                //TODO: make sure we actually decreased the interval
                var range = Till-From;
                var half = TimeSpan.FromSeconds(Math.Floor(range.TotalSeconds/2));
                return new [] {
                    new FetchData { ResourceId = ResourceId, From = From, Till = From.Add(half), MetricNames = MetricNames, TimeGrain = TimeGrain },
                    new FetchData { ResourceId = ResourceId, From = From.Add(half), Till = Till, MetricNames = MetricNames, TimeGrain = TimeGrain },
                };
            }
        }

        private async Task<ICollection<MetricValueSet>> Fetch(FetchData data) {
            CloudException thrown = null;
            try {
                var values = await _metricsClient.MetricValues.ListAsync(
				    data.ResourceId, 
				    data.MetricNames,
                    "",
				    data.TimeGrain,
				    data.From,
				    data.Till);
			
			    return values.MetricValueSetCollection.Value;
            }
            catch(CloudException e) {
                thrown = e;
            }

            if(thrown.Response.StatusCode == System.Net.HttpStatusCode.BadRequest && thrown.Message.Contains(TooManyValuesErrorMessage)) {
                if(!data.PartitionTresholdReached()) {
                    return await PartitionAndFetch(data);
                }
            }

            throw thrown;
        }

        private async Task<ICollection<MetricValueSet>> PartitionAndFetch(FetchData data) {
            var results = data.Partition().Select(_=>Fetch(_)).ToArray();

            return Join(await Task.WhenAll(results));
        }

        private ICollection<MetricValueSet> Join(ICollection<ICollection<MetricValueSet>> resultSets) {
            var byName = new Dictionary<string,MetricValueSet>();
            foreach(var resultSet in resultSets) {
                foreach(var metricResult in resultSet) {
                    if(!byName.ContainsKey(metricResult.Name)) {
                        byName[metricResult.Name] = metricResult;
                        continue;
                    }
                    
                    byName[metricResult.Name].MetricValues = byName[metricResult.Name].MetricValues.Concat(metricResult.MetricValues).OrderBy(_=>_.Timestamp).ToList();
                }
            }

            return byName.Values;
        }
        
        

        const string TooManyValuesErrorMessage = "You've exceeded the maximum number of allowed values per metric in a single request.";

		const string ApiVersion = "2013-10-01";


		internal async System.Threading.Tasks.Task<ICollection<MetricValueSet>> GetMetricsForCloudService(CloudServiceInstanceId instance,TimeSpan history) {
			return await GetMetricsForResourceId(instance.ResourceId,history, MetricsFilter.None);
		}
	}
}
