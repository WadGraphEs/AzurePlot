using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureManagementRestClient {
		readonly Microsoft.WindowsAzure.CertificateCloudCredentials _credentials;

		const string ManagementBaseUrl = "https://management.core.windows.net/";

		public AzureManagementRestClient(Microsoft.WindowsAzure.CertificateCloudCredentials credentials) {
			_credentials = credentials;
		}

		internal async Task<string> GETJson(string path,string apiVersion=null) {
			return await GET(path,request => {
				request.Accept = "application/json";
				if(apiVersion!=null) {
					AddApiVersion(request,apiVersion);
				}
			});
		}

		private static void AddApiVersion(HttpWebRequest request,string apiVersion) {
			request.Headers.Add("x-ms-version",apiVersion);
		}

		private async Task<string> GET(string path,Action<HttpWebRequest> requestBuilder) {
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ManagementBaseUrl + _credentials.SubscriptionId + path);

			request.ClientCertificates.Add(_credentials.ManagementCertificate);
			
			requestBuilder(request);

			var response = await request.GetResponseAsync();

			// Pipes the stream to a higher level stream reader with the required encoding format. 
			using(var readStream = new StreamReader(response.GetResponseStream(),System.Text.Encoding.UTF8)) {
				var output = readStream.ReadToEnd();

				return output;
			}
		}

		internal Task<string> GETXml(string path,string apiVersion) {
			return GET(path, request=> {
				AddApiVersion(request,apiVersion);
				request.Accept = "application/xml";
			});
		}
	}
}
