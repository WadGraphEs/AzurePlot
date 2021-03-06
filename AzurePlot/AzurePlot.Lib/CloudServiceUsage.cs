﻿using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlot.Lib {
	class CloudServiceUsage {
		readonly CloudServiceInstanceId _instanceId;
		readonly AzureCloudServiceMetricsApiClient _azureMetricsApiClient;
        readonly MetricsFilter _filter;

        public CloudServiceUsage(CloudServiceInstanceId instanceId,AzureCloudServiceMetricsApiClient azureMetricsApiClient)  : this(instanceId,azureMetricsApiClient,MetricsFilter.None) {
        }

		public CloudServiceUsage(CloudServiceInstanceId instanceId,AzureCloudServiceMetricsApiClient azureMetricsApiClient, MetricsFilter filter) {
			_azureMetricsApiClient = azureMetricsApiClient;
			_instanceId = instanceId;
            _filter = filter;
		}

		

		internal async Task<ICollection<UsageObject>> GetMetrics(TimeSpan history) {
			var metrics = await _azureMetricsApiClient.GetMetricsForInstance(_instanceId,history,_filter);

			var res = new List<UsageObject>();

			foreach(var metric in metrics) {
				foreach(var value in metric.MetricValues.OrderBy(_=>_.Timestamp)) {
					AddUsageObjectIfAvailable(_instanceId,metric,value,value.Average,"Average",res);
					//AddUsageObjectIfAvailable(instance,metric,value,value.Minimum,"Minimum",res);
					//AddUsageObjectIfAvailable(instance,metric,value,value.Maximum,"Maximum",res);
					
				}
			}

			return res;
		}

		private static void AddUsageObjectIfAvailable(CloudServiceInstanceId instance,MetricValueSet metric,MetricValue value,double? metricValue,string valueName, List<UsageObject> usageObjects) {
			if(metricValue == null) {
				return;
			}
			var usageObject = new UsageObject {
				GraphiteCounterName = instance.BuildGraphiteCounterName(metric.Name,metric.Unit,valueName).ToString(),
				Timestamp = value.Timestamp.ToString("o"),
				Value = metricValue.Value
			};
			usageObjects.Add(usageObject);
		}
	}
}
