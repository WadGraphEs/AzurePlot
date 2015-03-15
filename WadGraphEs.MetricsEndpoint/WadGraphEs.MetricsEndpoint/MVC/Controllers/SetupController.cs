using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WadGraphEs.MetricsEndpoint.DataAccess;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.Setup;

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
			if(ApplicationSetup.HasAdminUser()) {
				ModelState.AddModelError("admin-already-exists", "Admin already exists");
			}

            if(!ModelState.IsValid) {
                return View(command);
            }
			try {
				RegisterAccount(command);
			}
			catch(Exception e) {
				ModelState.AddModelError("exception",e);
				return View(command);
			}

            return RedirectToRoute("ThankYouForCreatingAccount");
        }

		private void RegisterAccount(CreateAdminAccount command) {
			ApplicationSetup.UpdateDatabaseToLatestSchema();
			
			var createUserResult = Users.Handle(command);
			
			if(!createUserResult.Succeeded) {
				throw new Exception(string.Join("\n",createUserResult.Errors));
			}
		}

		[HttpGet]
		public ActionResult ThankYou() {
			return View();
		}
    }
}