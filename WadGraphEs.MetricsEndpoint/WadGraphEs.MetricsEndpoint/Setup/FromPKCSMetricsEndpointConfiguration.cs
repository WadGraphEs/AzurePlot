using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Setup {
	class FromPKCSMetricsEndpointConfiguration : MetricsEndpointConfiguration{
		private byte[] _pfx;
		private string _password;
		private string _azureSubscriptionId;

		public string SubscriptionId {
			get { return _azureSubscriptionId; }
		}

		public FromPKCSMetricsEndpointConfiguration(byte[] pfx,string password,string azureSubscriptionId) {
			_pfx = pfx;
			_password = password;
			_azureSubscriptionId = azureSubscriptionId;
		}
		
		public Microsoft.WindowsAzure.CertificateCloudCredentials GetCertificateCloudCredentials() {
			
            var x509Certificate = new X509Certificate2(_pfx,_password,X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            return new CertificateCloudCredentials(_azureSubscriptionId, x509Certificate);
		}
	}
}
