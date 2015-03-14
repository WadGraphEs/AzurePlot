using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    [AllowAnonymous]
    public class SetupController : Controller{
        public ActionResult Step1() {
            return Content("Step1");
        }
    }
}