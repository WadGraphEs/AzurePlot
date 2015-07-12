using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
		readonly static Logger _logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start() {
			_logger.Trace("Starting AzurePlot");

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            WebRoutes.Register(RouteTable.Routes);
            GlobalFilters.Filters.Add(new NeedsSetupAttribute());
            GlobalFilters.Filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var razorViewEngine = ViewEngines.Engines.OfType<RazorViewEngine>().First();
            razorViewEngine.ViewLocationFormats = new [] {"~/MVC/Views/{1}/{0}.cshtml"};
			razorViewEngine.PartialViewLocationFormats = new [] {"~/MVC/Views/{1}/{0}.cshtml"};

            StartServicePointLogger();
        }

        private static void StartServicePointLogger() {
            bool doStart;
            
            if(!bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["StartServicePointLogger"],out doStart)) {
                return;
            }
            if(!doStart) {
                return;
            }

            var logger = LogManager.GetLogger("ServicePointMonitor");
            logger.Info("Logging is enabled");
            Lib.ServicePointMonitor.Start(TimeSpan.FromSeconds(5),logger.Info);
        }

        readonly static Logger _errorLogger = LogManager.GetLogger("Application_Error");

        protected void Application_Error(object sender, EventArgs e) {
            var exception = Server.GetLastError();
            
            _errorLogger.Log(LogLevel.Error, exception.Message, exception);

            return;
        }
    }
}