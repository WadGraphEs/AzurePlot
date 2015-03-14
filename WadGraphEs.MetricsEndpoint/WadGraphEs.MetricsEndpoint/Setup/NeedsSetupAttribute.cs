using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WadGraphEs.MetricsEndpoint.MVC.Controllers;

namespace WadGraphEs.MetricsEndpoint.Setup {
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
            return false;
        }
    }
}