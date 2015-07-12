using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlot.Lib {
	class AzureWebsitesInfoApiClient {
		readonly AzureWebsiteApiClient _client;

		public AzureWebsitesInfoApiClient(AzureWebsiteApiClient client) {
			this._client = client;
		}

		
		public async Task<ICollection<AzureWebsiteId>> ListAzureWebsites() {
			var output = new List<AzureWebsiteId>();
			foreach(var webspace in await GetWebspaces()) {
				output.AddRange(await GetWebsites(webspace));
			}

			return output;
		}

		private async Task<ICollection<AzureWebsiteId>> GetWebsites(string webspaceName) {
            var output = await _client.DoWebsiteWebServiceCall("/WebSpaces/" + webspaceName + "/sites");

            
            return JArray.Parse(output)
				.Select(ws => new AzureWebsiteId((string)ws["Name"],webspaceName)).ToList();
        }

		

        private async Task<ICollection<String>> GetWebspaces() {
            var output = await _client.DoWebsiteWebServiceCall("/WebSpaces");

            return JArray.Parse(output)
				.Select(ws => (string)ws["Name"])
				.ToList();
        }
	}

    public class AzureWebsitesInfoApiClientFacade {
        public static string FindWebspace(MetricsEndpointConfiguration config, string websiteName) {
            var infoClient = new AzureWebsitesInfoApiClient(new AzureWebsiteApiClient(new AzureManagementRestClient(config.GetCertificateCloudCredentials())));

            var website = infoClient.ListAzureWebsites().Result.FirstOrDefault(_=>_.Name ==  websiteName);
            if(website == null) {
                return null;
            }
            return website.Webspace;
        }
    }
}
