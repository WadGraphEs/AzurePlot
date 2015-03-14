using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using WadGraphEs.MetricsEndpoint.Authentication;

namespace WadGraphEs.MetricsEndpoint {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new AuthenticationMessageHandler());
			config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
