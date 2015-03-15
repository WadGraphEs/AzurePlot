﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureCloudServicesClient {
		
		readonly AzureCloudServiceMetricsApiClient _azureMetricsApiClient;
		readonly AzureCloudServiceInfoApiClient _azureCloudServiceInfoClient;

		public AzureCloudServicesClient(AzureManagementRestClient client, CertificateCloudCredentials credentials) {
			_azureCloudServiceInfoClient = new AzureCloudServiceInfoApiClient(client);
			_azureMetricsApiClient = new AzureCloudServiceMetricsApiClient(new AzureMetricsApiClient(credentials));
		}

		internal async System.Threading.Tasks.Task<ICollection<UsageObject>> GetUsageCollection(TimeSpan history) {
			var instances = await _azureCloudServiceInfoClient.ListInstances();

			var res = await Task.WhenAll(instances.Select(i=>new CloudServiceUsage(i,_azureMetricsApiClient).GetMetrics(history)));
						
			return res.SelectMany(_=>_).ToList();
		}

		internal void TestConnection() {
			_azureCloudServiceInfoClient.TestConnection();
		}

		internal string GetSubscriptionNameSync() {
			return _azureCloudServiceInfoClient.GetSubscriptionNameSync();
		}
	}
}
