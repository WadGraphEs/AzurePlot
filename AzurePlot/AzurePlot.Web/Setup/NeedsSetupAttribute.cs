using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AzurePlot.Web.DataAccess;
using AzurePlot.Web.MVC.Controllers;

namespace AzurePlot.Web.Setup {
    public class NeedsSetupAttribute : ActionFilterAttribute{
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if(!ApplicationIsConfigured() && !IsInSetupController(filterContext)) {
                filterContext.Result = new RedirectResult(new UrlHelper(filterContext.RequestContext).RouteUrl("setup"));
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        private bool IsInSetupController(ActionExecutingContext filterContext) {
            return filterContext.Controller is SetupController;
        }

        private bool ApplicationIsConfigured() {
			return ApplicationSetup.IsApplicationConfigured();
            

        }
    }
}