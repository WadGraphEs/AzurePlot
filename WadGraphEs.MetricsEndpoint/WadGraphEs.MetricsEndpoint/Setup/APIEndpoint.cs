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
			DataContext.Do(dataContext=>{
			    dataContext.APIKeys.Add(new APIKeyRecord{ APIKey = apiKey });

			    dataContext.SaveChanges();
            });
		}
        
		internal static bool IsAPIKeyCreated() {
			return DataContext.Do(ctx=>ctx.APIKeys.Any());
		}

		internal static bool AuthenticateKey(string key) {
			return DataContext.Do(ctx=>ctx.APIKeys.Any(_=>_.APIKey == key));
		}

        internal static ApiSettings GetApiSettings() {
            return DataContext.Do(dataContext=>{
                var key = dataContext.APIKeys.First().APIKey;
                var endpoint = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                return new ApiSettings {
                    APIKey =  key,
                    EndpointUrl = endpoint
                };
            });
        }

        internal static TestAPIResult TestEndpoint(MVC.Commands.TestAPICommand cmd) {
            var wc = WebRequest.CreateHttp(cmd.EndpointUrl.Trim('/') + "/usages");

            wc.AllowAutoRedirect = false;

            wc.Accept = cmd.Accept;
            wc.Headers.Add("Authorization", string.Format("MetricsEndpoint-Key {0}", cmd.APIKey));

            var sb = new StringBuilder();
            
            try {
                var response = wc.GetResponse() as HttpWebResponse;

                AppendRequest(wc,sb);
                AppendResponse(sb,response);
                
                if(response.StatusCode!=HttpStatusCode.OK) {
                    return TestAPIResult.Failed(sb.ToString());
                }
                return TestAPIResult.IsSuccess(sb.ToString());

            }
            catch(WebException e) {
                AppendRequest(wc,sb);
                AppendResponse(sb,e.Response as HttpWebResponse);

                return TestAPIResult.Failed(sb.ToString());
            }
        }

        private static void AppendRequest(HttpWebRequest wc,StringBuilder sb) {
            sb.AppendLine("Request:");
            sb.AppendLine("========");
            sb.AppendLine(string.Format("GET {0}", wc.RequestUri));
            sb.AppendLine();
            sb.AppendLine("Headers:");
            sb.AppendLine("========");

            foreach(var key in wc.Headers.AllKeys) {
                sb.AppendLine(string.Format("{0}: {1}", key,wc.Headers[key]));
            }
        }

        private static void AppendResponse(StringBuilder sb,HttpWebResponse response) {
            sb.AppendLine();
            sb.AppendLine("Response:");
            sb.AppendLine("=========");
            sb.AppendLine(string.Format("Status: {0} {1} ({2})", (int)response.StatusCode, response.StatusCode,response.StatusDescription));
            sb.AppendLine();
            sb.AppendLine("Headers:");
            sb.AppendLine("========");
            var msg = ReadStream(response.GetResponseStream());

            foreach(var key in response.Headers.AllKeys) {
                sb.AppendLine(string.Format("{0}: {1}", key, response.Headers[key]));
            }
            sb.AppendLine();
            sb.AppendLine("Body:");
            sb.AppendLine("=====");
            sb.AppendLine(msg);
        }

        private static string ReadStream(Stream stream) {
            using(var streamReader = new StreamReader(stream)) {
                return streamReader.ReadToEnd();
                
            }
        }
    }
}