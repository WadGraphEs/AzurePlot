using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzurePlot.Web.DataAccess;
using AzurePlot.Web.MVC.Commands;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.MVC.Controllers {
    [AllowAnonymous]
    public class SetupController : Controller{
		[HttpGet]
		public ActionResult Step1() {
			if(!ApplicationSetup.IsDatabaseCreated()) {
				return View();
			}

			if(!ApplicationSetup.IsSchemaUpToDate()) {
				return RedirectToAction("UpdateSchema");
			}

			if(!ApplicationSetup.HasAdminUser()) {
				return View();
			}

			if(!ApplicationSetup.IsAPIKeyCreated()) {
				return RedirectToAction("CreateAPIKey");
			}            

			return RedirectToAction("ThankYou");
        }

		[HttpGet]
		public ActionResult UpdateSchema() {
			var pendingMigrations = ApplicationSetup.GetPendingMigrations();
			return View(pendingMigrations);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateSchema(bool? overload) {
			ApplicationSetup.UpdateDatabaseToLatestSchema();
			return RedirectToAction("Step1");
		}

        [HttpGet]
		public ActionResult CreateAdmin() {
			if(ApplicationSetup.HasAdminUser()) {
				return RedirectToAction("Step1");
			}
            return View(new CreateAdminAccount());
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(CreateAdminAccount command) {
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

            return RedirectToRoute("setup");
        }

		private void RegisterAccount(CreateAdminAccount command) {
			ApplicationSetup.UpdateDatabaseToLatestSchema();
			
			var createUserResult = Users.Handle(command);
			
			if(!createUserResult.Succeeded) {
				throw new Exception(string.Join("\n",createUserResult.Errors));
			}
		}

		[HttpGet]
		public ActionResult CreateAPIKey() {
			if(ApplicationSetup.IsAPIKeyCreated()) {
				return RedirectToAction("Step1");
			}
			return View(Commands.CreateAPIKey.NewRandom());
		}

		[HttpPost]
		public ActionResult CreateAPIKey(CreateAPIKey cmd) {
			if(!ModelState.IsValid) {
				return View(cmd);
			}

			APIEndpoint.AddAPIKey(cmd.APIKey);

			return RedirectToAction("Step1");
		}

		[HttpGet]
		public ActionResult ThankYou() {
			return View();
		}
    }
}