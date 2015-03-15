using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    public class HomeController : Controller{
		[HttpGet]
        public ActionResult Index() {
            return View(new OverviewViewModel());
        }
		[HttpGet]
		public ActionResult AddAzureSubscription() {
			return View();
		}

		[HttpGet]
		public ActionResult AddAzureSubscriptionStep1() {
			return View(new CreateManagementCertificateCommand ());
		}

		[HttpPost]
		public ActionResult AddAzureSubscriptionStep1(CreateManagementCertificateCommand cmd) {
			if(!ModelState.IsValid) {
				return View(cmd);
			}
			return View(new CreateManagementCertificateCommand ());
		}
    }
}