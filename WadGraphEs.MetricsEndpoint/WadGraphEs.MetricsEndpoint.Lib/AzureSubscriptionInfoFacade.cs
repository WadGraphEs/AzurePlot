using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib {
    public class AzureSubscriptionInfoFacade {
        public static string GetSubcriptionName(MetricsEndpointConfiguration config) {
            var credentials = config.GetCertificateCloudCredentials();
            var client = new AzureCloudServicesClient(new AzureManagementRestClient(credentials),credentials);
            return client.GetSubscriptionNameSync();
        }
    }
}
