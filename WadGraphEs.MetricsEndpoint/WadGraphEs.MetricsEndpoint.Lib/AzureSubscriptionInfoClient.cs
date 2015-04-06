using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib {
	public class AzureSubscriptionInfoClient {
		readonly AzureManagementRestClient _client;
		readonly AzureWebsitesInfoApiClient _websiteInfoClient;
		readonly string _subscriptionId;
		public AzureSubscriptionInfoClient(MetricsEndpointConfiguration metricsEndpointConfiguration) {
			_subscriptionId = metricsEndpointConfiguration.SubscriptionId;
			_client = new AzureManagementRestClient(metricsEndpointConfiguration.GetCertificateCloudCredentials());
			_websiteInfoClient = new AzureWebsitesInfoApiClient(new AzureWebsiteApiClient(_client));
		}

		public async Task<ICollection<AzureWebsite>> ListWebsites() {
			return (await _websiteInfoClient.ListAzureWebsites()).Select(_=>
				new AzureWebsite {
					Name = _.Name,
					Uri = GetUri(_)
				}
			).ToList();
		}

		private Uri GetUri(AzureWebsiteId azureWebsiteId) {
			return new Uri(string.Format("wadgraphes://{0}/websites/{1}/{2}", _subscriptionId, azureWebsiteId.Webspace, azureWebsiteId.Name));
		}
	}
}
