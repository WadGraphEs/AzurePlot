using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class APIEndpoint {
		internal static void AddAPIKey(string apiKey) {
			var dataContext = GetDataContext();

			dataContext.APIKeys.Add(new APIKeyRecord{ APIKey = apiKey });

			dataContext.SaveChanges();
		}

		private static DataContext GetDataContext() {
			var dataContext=  new DataContext();
			return dataContext;
		}

		internal static bool IsAPIKeyCreated() {
			return GetDataContext().APIKeys.Any();
		}

		internal static bool AuthenticateKey(string key) {
			var dataContext = GetDataContext();
			return dataContext.APIKeys.Any(_=>_.APIKey == key);
		}

        internal static ApiSettings GetApiSettings() {
            var dataContext = GetDataContext();
            var key = dataContext.APIKeys.First().APIKey;
            var endpoint = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            return new ApiSettings {
                APIKey =  key,
                EndpointUrl = endpoint
            };
        }

        internal static TestAPIResult TestEndpoint(MVC.Commands.TestAPICommand cmd) {
            var wc = WebRequest.CreateHttp(cmd.EndpointUrl.Trim('/') + "/usages");

            wc.AllowAutoRedirect = false;

            wc.Accept = cmd.Accept;
            wc.Headers.Add("Authorization", string.Format("MetricsEndpoint-Key {0}", cmd.APIKey));

            var sb = new StringBuilder();

            AppendRequest(wc,sb);


            try {
                var response = wc.GetResponse() as HttpWebResponse;

                AppendResponse(sb,response);

                return TestAPIResult.IsSuccess(sb.ToString());

            }
            catch(WebException e) {
                AppendResponse(sb,e.Response as HttpWebResponse);

                return TestAPIResult.Failed(sb.ToString());
            }
        }

        private static void AppendRequest(HttpWebRequest wc,StringBuilder sb) {
            sb.AppendLine("Request:");
            sb.AppendLine(string.Format("GET {0}", wc.RequestUri));
            sb.AppendLine("Headers:");

            for(var i=0;i<wc.Headers.Count;i++) {
                sb.AppendLine(wc.Headers[i]);
            }
        }

        private static void AppendResponse(StringBuilder sb,HttpWebResponse response) {
            sb.AppendLine();
            sb.AppendLine("Response:");
            sb.AppendLine(string.Format("Status: {0}", (int)response.StatusCode));
            sb.AppendLine("Headers:");
            var msg = ReadStream(response.GetResponseStream());

            for(var i=0;i<response.Headers.Count;i++) {
                sb.AppendLine(response.Headers[i]);
            }
            sb.AppendLine("Body:");
            sb.AppendLine(msg);
        }

        private static string ReadStream(Stream stream) {
            using(var streamReader = new StreamReader(stream)) {
                return streamReader.ReadToEnd();
                
            }
        }
    }
}