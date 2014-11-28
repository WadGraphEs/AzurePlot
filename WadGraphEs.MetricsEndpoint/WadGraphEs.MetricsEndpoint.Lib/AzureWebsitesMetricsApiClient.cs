using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureWebsitesMetricsApiClient {
		readonly AzureWebsiteApiClient _client;

		public AzureWebsitesMetricsApiClient(AzureWebsiteApiClient client) {
			_client = client;
		}

		internal async System.Threading.Tasks.Task<ICollection<WebsiteApiMetricsData>> GetWebsiteUsage(AzureWebsiteId websiteId) {
			 return await _client.DoWebsiteWebServiceCall<WebsiteApiMetricsData[]>(websiteId.ResourceId + "/usages");
		}
	}


}
