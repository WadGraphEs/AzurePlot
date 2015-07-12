using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace AzurePlot.Web.Authentication {
	public class AuthenticationMessageHandler : DelegatingHandler{
		async protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,System.Threading.CancellationToken cancellationToken) {
			AuthenticateUser(request);

			var response = await base.SendAsync(request,cancellationToken);

			if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
				return AddAuthenticationHeaders(response);
			}	

			response.Headers.Add("X-MetricsEndpoint-IsAuthenticated", IsAuthenticated().ToString());

			return response;
		}

		private static bool IsAuthenticated() {
			return Thread.CurrentPrincipal.Identity!=null && Thread.CurrentPrincipal.Identity.IsAuthenticated;
		}

		private HttpResponseMessage AddAuthenticationHeaders(HttpResponseMessage response) {
			response.Headers.WwwAuthenticate.Clear();
            response.Headers.WwwAuthenticate.Add(new System.Net.Http.Headers.AuthenticationHeaderValue(MetricsEndpointKeyAuthenticationScheme, @"realm=""WadGraphEs.MetricsEndpoint"""));

			return response;
		}

		const string MetricsEndpointKeyAuthenticationScheme = "MetricsEndpoint-Key";

		private void AuthenticateUser(HttpRequestMessage request) {
			if(request.Headers.Authorization==null) {
				return;
			}

			if(!IsAuthenticationScheme(MetricsEndpointKeyAuthenticationScheme,request)) {
				return;
			}

			AuthenticatFromMetricsEndpointKey(request);
		}

		private void AuthenticatFromMetricsEndpointKey(HttpRequestMessage request) {
			var author = GetAuthorFromRequest(request);

			var keyvalue = request.Headers.Authorization.Parameter;

			Authenticate(author,keyvalue);
		}

		private bool IsAuthenticationScheme(string scheme,HttpRequestMessage request) {
			return request.Headers.Authorization.Scheme.Equals(scheme, StringComparison.OrdinalIgnoreCase);
		}

		private void Authenticate(string author, string usedKey) {

            if (!AuthenticationKeys.CheckKey(usedKey)) {
                return;
			}

			var principal= new MetricsEndpointPrincipal(new GenericIdentity(author),usedKey);
			Thread.CurrentPrincipal = principal;
			if(HttpContext.Current!=null) {
				HttpContext.Current.User = principal;
			}
		}

		private string GetAuthorFromRequest(HttpRequestMessage request) {
			if(!request.Headers.Contains(AuthorHeaderName)) {
				return FormatAuthor("Unknown");
				
			}
			return FormatAuthor(request.Headers.GetValues(AuthorHeaderName).First());
		}

		private string FormatAuthor(string authorName) {
			return string.Format("{1} ({0})", TryGetIp(),authorName);
		}

		const string AuthorHeaderName = "X-WadGraphEs.MetricsEndpoint-Author";

		private string TryGetIp() {
			if(HttpContext.Current==null) {
				return "0.0.0.0";
			}

			return HttpContext.Current.Request.UserHostAddress;
		}


		
	}

	class MetricsEndpointPrincipal : GenericPrincipal {
		readonly string _key;
		public MetricsEndpointPrincipal(IIdentity identity, string key) : base(identity, new string[] {}) {
			_key = key;
		}

		
    }
}