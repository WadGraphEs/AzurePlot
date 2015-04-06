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

			config.Routes.MapHttpRoute(
                name: "api/list-all-charts",
                routeTemplate: "api/list-all-charts",
                defaults: new { controller = "Charts", action="list-all-charts" }
            );

			config.Routes.MapHttpRoute(
                name: "api/charts/get-chart-data",
                routeTemplate: "api/charts/get-chart-data",
                defaults: new { controller = "Charts", action="get-chart-data" }
            );
			

            config.MessageHandlers.Add(new AuthenticationMessageHandler());
			config.Filters.Add(new AuthorizeAttribute());
        }
    }
}
