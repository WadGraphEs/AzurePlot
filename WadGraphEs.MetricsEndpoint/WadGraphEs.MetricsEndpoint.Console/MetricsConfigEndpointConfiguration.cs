using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.Lib {
	public class MetricsConfigEndpointConfiguration : MetricsEndpointConfiguration {
		const string ApiKeyConfigKey = "AuthenticationKey";
		const string AzureSubscriptionIdConfigKey = "AzureSubscriptionId";
		const string CertFilenameConfigKey = "CertFilename";
		const string CertPasswordConfigKey = "CertPassword";

        readonly string _metricsFileName;

		public MetricsConfigEndpointConfiguration(string metricsConfigFilename) {
            

			_metricsFileName = metricsConfigFilename;
			_certificateFilename= new Lazy<string>(FindCertFilename);
			_apikey= new Lazy<string>(FindApiKey);
			_subscriptionId= new Lazy<string>(FindSubscriptionId);
			_certificatePassword= new Lazy<string>(FindCertPassword);
		}
		
		readonly Lazy<string> _apikey;
		readonly Lazy<string> _subscriptionId;

		public string SubscriptionId {
			get { return _subscriptionId.Value; }
		}

		readonly Lazy<string> _certificateFilename ;

		public string CertificateFilename {
			get { return _certificateFilename.Value; }
		}

		readonly Lazy<string> _certificatePassword;
		

		string CertificatePassword {
			get { return _certificatePassword.Value; }
		} 

		
		string FindApiKey() {
			return FindKeyValue(ApiKeyConfigKey,@"Key not set, please add {0}=<your key> to metricsendpoint.config",@"Key format not valid ({1}), please add {0}=<your key> to metricsendpoint.config");
		}

		private string FindKeyValue(string key, string notFoundFormat, string invalidFormatFormat) {
			var line = FindLineForConfigKeyOrNull(key);

			if(line == null) {
				throw new InvalidOperationException(string.Format(notFoundFormat,key));
			}

			var spl = line.Split('=');

			if(spl.Count()<2) {
				throw new InvalidOperationException(string.Format(invalidFormatFormat,key,line));
			}

			return spl[1];
		}

		private string FindLineForConfigKeyOrNull(string configKey) {
            //var filepath = Path.Combine(_baseDir,"metricsendpoint.config");

			if(!File.Exists(_metricsFileName)) {
				throw new InvalidOperationException(string.Format("Couldn't find config file: {0}", _metricsFileName));
			}

			var lines = File.ReadAllLines(_metricsFileName);

			return lines.FirstOrDefault(_ => _.StartsWith(configKey));
			
		}

		string ApiKey {
			get {
				return _apikey.Value;
			}
		}



		internal Microsoft.WindowsAzure.CertificateCloudCredentials GetCertificateCloudCredentials() {
            if(!File.Exists(CertificateFilename)) {
				throw new InvalidOperationException(string.Format("Couldn't locate certificate file in {0}", CertificateFilename));
			}

            var x509Certificate = new X509Certificate2(CertificateFilename, CertificatePassword, X509KeyStorageFlags.MachineKeySet);

            return new CertificateCloudCredentials(SubscriptionId, x509Certificate);
		}

		private string FindCertPassword() {
			return FindKeyValue(CertPasswordConfigKey,
				@"Certificate password not set  not set, please add {0}=<your certificate password> to metricsendpoint.config",
				@"Key format not valid ({1}), please add {0}=<your certificate password> to metricsendpoint.config"
				);
		}

		private string FindCertFilename() {
			return FindKeyValue(CertFilenameConfigKey,
				@"Certificate file name not set  not set, please add {0}=<your certificate filename> to metricsendpoint.config",
				@"Key format not valid ({1}), please add {0}=<your certificate filename> to metricsendpoint.config"
			);
		}

		private string FindSubscriptionId() {
			return FindKeyValue(AzureSubscriptionIdConfigKey,
				@"Subscription not set  not set, please add {0}=<your subscription id> to metricsendpoint.config",
				@"Key format not valid ({1}), please add {0}=<your subscription id> to metricsendpoint.config"
			);
		}

		public bool AuthenticateKey(string key) {
			return ApiKey == key;
		}

		CertificateCloudCredentials MetricsEndpointConfiguration.GetCertificateCloudCredentials() {
			return GetCertificateCloudCredentials();
		}
	}
}