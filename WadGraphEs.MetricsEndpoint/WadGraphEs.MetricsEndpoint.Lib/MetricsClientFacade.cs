using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlot.Lib {
    class MetricsClientFacade {
        //todo figure out if metricsclient is indeed multithreaded :S
        readonly static ConcurrentDictionary<string, MetricsClient> _clients=  new ConcurrentDictionary<string,MetricsClient>();



        public static Task<Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models.MetricDefinitionListResponse> 
                ListDefinitionsAsync(SubscriptionCloudCredentials credentials, string resourceId,IList<string> metricNames,string metricNamespace) {
                    return GetClient(credentials).MetricDefinitions.ListAsync(resourceId,metricNames,metricNamespace);
        }

        private static MetricsClient GetClient(SubscriptionCloudCredentials credentials) {
            return _clients.GetOrAdd(credentials.SubscriptionId,c=>new MetricsClient(credentials));
        }

        internal static Task<Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models.MetricValueListResponse> 
                ListValuesAsync(
                    SubscriptionCloudCredentials credentials, 
                    string resourceId,IList<string> metricNames,string metricNamespace,TimeSpan timeGrain,DateTime startTime,DateTime endTime) {
            return GetClient(credentials).MetricValues.ListAsync(resourceId,metricNames,metricNamespace,timeGrain,startTime,endTime);
        }
    }
}
