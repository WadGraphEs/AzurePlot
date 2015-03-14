using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.MVC.Commands;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    [AllowAnonymous]
    public class SetupController : Controller{
        [HttpGet]
        public ActionResult Step1() {
            return View(new CreateAdminAccount());
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Step1(CreateAdminAccount command) {
            if(!ModelState.IsValid) {
                return View(command);
            }
            
            return RedirectToRoute("ThankYouForCreatingAccount");
        }

		[HttpGet]
		public ActionResult ThankYou() {
			return View();
		}
    }
}