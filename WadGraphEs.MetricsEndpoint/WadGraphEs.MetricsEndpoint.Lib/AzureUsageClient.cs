using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WadGraphEs.MetricsEndpoint.Lib {
    public class AzureUsageClient {
		readonly CertificateCloudCredentials _credentials;
		readonly AzureManagementRestClient _client;
		
        public AzureUsageClient(MetricsEndpointConfiguration config) {
			_credentials  = config.GetCertificateCloudCredentials();
			_client = GetClient(config.GetCertificateCloudCredentials());
        }

		public async Task<ICollection<UsageObject>> GetWebsitesUsage() {
			var websiteClient =new AzureWebsitesUsageClient(_client,_credentials);


			return await websiteClient.GetUsageCollection();
        }

		public async Task<ICollection<UsageObject>> GetWebsitesUsageForWebsite(string webspace, string websiteName,TimeSpan history, params string[] filters) {
			var websiteId = new AzureWebsiteId(websiteName,webspace);
			var res = await new AzureWebsitesUsageClient(_client,_credentials).GetUsageCollectionForWebsite(websiteId,history);

            if(!filters.Any()) {
                return res;
            }

			var filtersRegex = filters.Select(_=>new Regex(_,RegexOptions.IgnoreCase)).ToList();
			return res.Where(_=>filtersRegex.Any(f=>f.IsMatch(_.GraphiteCounterName))).ToList();
		}

		private static AzureManagementRestClient GetClient(CertificateCloudCredentials credentials) {
			return new AzureManagementRestClient(credentials);
		}



		public async Task<ICollection<UsageObject>> GetCloudServiceUsages() {
			return await GetAzureCloudServicesClient().GetUsageCollection(TimeSpan.FromHours(1));
		}

		private AzureCloudServicesClient GetAzureCloudServicesClient() {
			return new AzureCloudServicesClient(_client,_credentials);
		}

		public void TestConnection() {
			GetAzureCloudServicesClient().TestConnection();
		}

		public string GetSubscriptionNameSync() {
			return GetAzureCloudServicesClient().GetSubscriptionNameSync();
		}

		
	}
}