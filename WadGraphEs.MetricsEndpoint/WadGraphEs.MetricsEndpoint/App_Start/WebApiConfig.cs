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
                name: "status",
                routeTemplate: "status",
                defaults: new { controller = "Status" }
            );

            config.Routes.MapHttpRoute(
                name: "usages",
                routeTemplate: "usages",
                defaults: new { controller = "Usages" }
            );

            config.MessageHandlers.Add(new AuthenticationMessageHandler());
			config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
