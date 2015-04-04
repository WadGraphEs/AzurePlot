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
			routes.MapRoute("update-schema", "setup/update-schema", new { controller = "Setup", action = "UpdateSchema" });
			routes.MapRoute("add-azure-subscription", "services/add-azure-subscription", new { controller = "Home", action = "AddAzureSubscription" });
			routes.MapRoute("add-azure-subscription-step-1", "services/add-azure-subscription/step1", new { controller = "Home", action = "AddAzureSubscriptionStep1" });
			routes.MapRoute("add-azure-subscription-step-2", "services/add-azure-subscription/step2", new { controller = "Home", action = "AddAzureSubscriptionStep2" });
			routes.MapRoute("add-azure-subscription-step-3", "services/add-azure-subscription/step3", new { controller = "Home", action = "AddAzureSubscriptionStep3" });
			routes.MapRoute("add-azure-subscription-step-4", "services/add-azure-subscription/step4", new { controller = "Home", action = "AddAzureSubscriptionStep4" });
			
			routes.MapRoute("add-azure-subscription-download-certificate", "services/add-azure-subscription/download-certificate", new { controller = "Home", action = "DownloadCertificate" });
            routes.MapRoute("api-settings", "api-settings", new { controller = "Home", action = "ApiSettings" });
            routes.MapRoute("test-api", "test-api", new { controller = "Home", action = "TestApi" });
			routes.MapRoute("logs", "logs", new { controller = "Home", action = "Logs" });
            
			routes.MapRoute("add-azure-sql-database", "services/add-azure-sql-database", new { controller = "Home", action = "AddAzureSQLDatabase" });
			routes.MapRoute("add-azure-sql-database-step-1", "services/add-azure-sql-database-step-1", new { controller = "Home", action = "AddAzureSQLDatabaseStep1" });
			routes.MapRoute("add-azure-sql-database-step-2", "services/add-azure-sql-database-step-2", new { controller = "Home", action = "AddAzureSQLDatabaseStep2" });
			routes.MapRoute("add-azure-sql-database-step-3", "services/add-azure-sql-database-step-3", new { controller = "Home", action = "AddAzureSQLDatabaseStep3" });
			
			
			
			routes.MapRoute("ThankYouForCreatingAccount", "setup/thankyou", new { controller = "Setup", action = "ThankYou" });
			
        }
    }
}