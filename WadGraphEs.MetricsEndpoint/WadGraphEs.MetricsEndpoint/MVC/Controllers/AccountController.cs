using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.Setup;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace WadGraphEs.MetricsEndpoint.MVC.Controllers {
    public class AccountController : Controller{
        [AllowAnonymous]
		[HttpGet]
        public ActionResult Login() {
            return View(new LoginCommand());
        }

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Login(LoginCommand cmd) {
			var user = Users.FindOrNull(cmd.Username, cmd.Password);

			if(user==null) {
				ModelState.AddModelError("user-not-found","username or password invalid");
				return View(cmd);
			}

			var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            var userIdentity = Users.GetUserManager().CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { }, userIdentity);
           

            return RedirectToRoute("home");
        }
    }
}