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
            
			RegisterAccount(command);

            return RedirectToRoute("ThankYouForCreatingAccount");
        }

		private void RegisterAccount(CreateAdminAccount command) {
			var dbContext = new DataContext();

			var dbMigrator = new System.Data.Entity.Migrations.DbMigrator(new WadGraphEs.MetricsEndpoint.Migrations.Configuration());

			var migrations = dbMigrator.GetPendingMigrations();

			if(migrations.Any()) {
				dbMigrator.Update();
			}
											
			var manager = new UserManager<ProxyUser>(new UserStore<ProxyUser>(dbContext));

			manager.PasswordValidator = new PasswordValidator() { RequiredLength  = 1};

			var result = manager.Create(new ProxyUser { UserName = command.Username},command.Password);
			if(!result.Succeeded) {
				throw new Exception();
			}
		}

		[HttpGet]
		public ActionResult ThankYou() {
			return View();
		}
    }
}