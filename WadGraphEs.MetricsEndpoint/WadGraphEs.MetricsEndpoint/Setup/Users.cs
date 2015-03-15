using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class Users {
		internal static IdentityResult Handle(MVC.Commands.CreateAdminAccount command) {
			var manager = GetUserManager();

			var result = manager.Create(new ProxyUser { UserName = command.Username},command.Password);

			return result;
		}

		public static UserManager<ProxyUser> GetUserManager() {
			var userStore = GetUserStore();
			var manager = new UserManager<ProxyUser>(userStore);

			manager.PasswordValidator = new PasswordValidator() { RequiredLength  = 1 };

			return manager;
		}

		private static UserStore<ProxyUser> GetUserStore() {
			var dbContext = new DataContext();
			return new UserStore<ProxyUser>(dbContext);
		}

		internal static bool HasAUser() {
			return GetUserStore().Users.Any();
		}

		internal static ProxyUser FindOrNull(string username,string password) {
			return GetUserManager().Find(username,password);
		}
	}
}