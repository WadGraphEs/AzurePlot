using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureCloudServiceMetricsApiClient {
		readonly AzureMetricsApiClient _azureMetricsApiClient;

		public AzureCloudServiceMetricsApiClient(AzureMetricsApiClient azureMetricsApiClient) {
			
			_azureMetricsApiClient = azureMetricsApiClient;
		}

		public async Task<ICollection<MetricValueSet>> GetMetricsForInstance(CloudServiceInstanceId instance, TimeSpan history) {
			//var resourceId=ResourceIdBuilder.BuildCloudServiceResourceId(instance.ServiceName,instance.DeploymentName,instance.RoleName,instance.InstanceName);
			return await _azureMetricsApiClient.GetMetricsForCloudService(instance, history);

		}
	}
}
