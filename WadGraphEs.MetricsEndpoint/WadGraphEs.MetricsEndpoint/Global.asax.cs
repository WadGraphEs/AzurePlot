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
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
		readonly static Logger _logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start() {
			_logger.Trace("Starting WadGraphEs Azure Dashboard");

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            WebRoutes.Register(RouteTable.Routes);
            GlobalFilters.Filters.Add(new NeedsSetupAttribute());
            GlobalFilters.Filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var razorViewEngine = ViewEngines.Engines.OfType<RazorViewEngine>().First();
            razorViewEngine.ViewLocationFormats = new [] {"~/MVC/Views/{1}/{0}.cshtml"};
			razorViewEngine.PartialViewLocationFormats = new [] {"~/MVC/Views/{1}/{0}.cshtml"};
        }
    }
}