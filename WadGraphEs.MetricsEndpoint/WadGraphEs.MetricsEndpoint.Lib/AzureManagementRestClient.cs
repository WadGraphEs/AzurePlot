using NLog;
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

		readonly static Logger _logger = LogManager.GetCurrentClassLogger();
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
			HttpWebRequest request = BuildRequest(path,requestBuilder);

			try {
				var response = await request.GetResponseAsync();

				return ReadResponse(response);
			}
			catch(WebException webException) {
				var response = "<not available>";
				if(webException.Response!=null) {
					response = ReadResponse(webException.Response);
				}
				_logger.ErrorException(

					string.Format("Failed requesting {0}; StatusCode: {1}; Body:\n{2}", request.RequestUri, webException.Status,response),
					webException);
				throw;
			}
		}

		private string GETSync(string path,Action<HttpWebRequest> requestBuilder) {
			HttpWebRequest request = BuildRequest(path,requestBuilder);

			try {
				var response = request.GetResponse();

				return ReadResponse(response);
			}
			catch(WebException webException) {
				var response = "<not available>";
				if(webException.Response!=null) {
					response = ReadResponse(webException.Response);
				}
				_logger.ErrorException(

					string.Format("Failed requesting {0}; StatusCode: {1}; Body:\n{2}", request.RequestUri, webException.Status,response),
					webException);
				throw;
			}
		}

		private HttpWebRequest BuildRequest(string path,Action<HttpWebRequest> requestBuilder) {
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ManagementBaseUrl + _credentials.SubscriptionId + path);

			request.ClientCertificates.Add(_credentials.ManagementCertificate);

			requestBuilder(request);
			return request;
		}

		private static string ReadResponse(WebResponse response) {
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

		internal string GetXmlSync(string path,string apiVersion) {
			return GETSync(path,request=> {
				AddApiVersion(request,apiVersion);
				request.Accept = "application/xml";
			});
		}
	}
}
