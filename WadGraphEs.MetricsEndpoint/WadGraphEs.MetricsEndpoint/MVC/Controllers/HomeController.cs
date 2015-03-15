using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    public class HomeController : Controller{
        public ActionResult Index() {
            return View(new OverviewViewModel());
        }
    }
}