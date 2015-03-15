using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace WadGraphEs.MetricsEndpoint {
    public class WebRoutes {
        internal static void Register(System.Web.Routing.RouteCollection routes) {
            routes.MapRoute("home", "", new { controller = "Home", action = "Index" });
            routes.MapRoute("login", "login", new { controller = "Account", action = "Login" });
			routes.MapRoute("logout", "logout", new { controller = "Account", action = "Logout" });
            routes.MapRoute("setup", "setup/step1", new { controller = "Setup", action = "step1" });
			routes.MapRoute("create-admin-account", "setup/create-admin-account", new { controller = "Setup", action = "CreateAdmin" });
			routes.MapRoute("create-api-key", "setup/create-api-key", new { controller = "Setup", action = "CreateAPIKey" });
			
			
			routes.MapRoute("ThankYouForCreatingAccount", "setup/thankyou", new { controller = "Setup", action = "ThankYou" });
			
        }
    }
}