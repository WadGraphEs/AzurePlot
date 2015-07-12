using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlot.Lib {
	class AzureWebsiteApiClient {
	
		const string ApiVersion  = "2013-08-01";
		readonly   AzureManagementRestClient _client;

		public AzureWebsiteApiClient(AzureManagementRestClient client) {
			_client = client;
		}
		
		
        public async Task<string> DoWebsiteWebServiceCall(string path) {
			return await _client.GETJson("/services" + path, apiVersion:ApiVersion);
        }

		public async Task<T> DoWebsiteWebServiceCall<T>(string path) {
			var json = await _client.GETJson("/services" + path, apiVersion:ApiVersion);

			return JsonConvert.DeserializeObject<T>(json);
        }
	}
}
