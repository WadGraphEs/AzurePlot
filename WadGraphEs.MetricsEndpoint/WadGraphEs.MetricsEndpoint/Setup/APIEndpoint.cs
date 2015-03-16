using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}