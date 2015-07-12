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
        public ActionResult Login(bool? fromSignOut) {
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
			
            var userIdentity = Users.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

			SignIn(userIdentity);
           
            return RedirectToRoute("home");
        }

		private static void SignIn(System.Security.Claims.ClaimsIdentity userIdentity) {
			var authenticationManager = GetAuthenticationManager();
			authenticationManager.SignIn(new AuthenticationProperties() { },userIdentity);
		}

		private static IAuthenticationManager GetAuthenticationManager() {
			var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
			return authenticationManager;
		}

		[AllowAnonymous]
		[HttpGet]
        public ActionResult Logout() {
			GetAuthenticationManager().SignOut();
            return RedirectToRoute("login",new { fromSignOut = true });
        }
    }
}