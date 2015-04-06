using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	public interface MetricsEndpointConfiguration {
		Microsoft.WindowsAzure.CertificateCloudCredentials GetCertificateCloudCredentials();
		string SubscriptionId{get;}
	}
}
