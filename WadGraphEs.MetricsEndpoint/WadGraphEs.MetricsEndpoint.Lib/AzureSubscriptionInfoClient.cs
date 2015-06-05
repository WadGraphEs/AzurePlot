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
					Uri = GetWebsiteUri(_)
				}
			).ToList();
		}

		private Uri GetWebsiteUri(AzureWebsiteId azureWebsiteId) {
			return new Uri(string.Format("wadgraphes://{0}/websites/{1}/{2}", _subscriptionId, azureWebsiteId.Webspace, azureWebsiteId.Name));
		}

        internal async Task<ICollection<AzureCloudService>> ListCloudserviceInstances() {
            var infoClient =new AzureCloudServiceInfoApiClient(_client);

            var services = await infoClient.ListInstances();

            return services.GroupBy(_=>_.CloudServiceId).Select(_=> new AzureCloudService {
                ServiceResourceId = _.Key,
                ServiceName = _.First().ServiceName,
                SubscriptionId = _subscriptionId,
                Instances = _.ToList()
            }).ToList();
        }
    }
}
