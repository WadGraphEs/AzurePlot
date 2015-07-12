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
			return DoUserManager(manager=>{
			    var result = manager.Create(new ProxyUser { UserName = command.Username},command.Password);

			    return result;
            });
		}

		static T DoUserManager<T>(Func<UserManager<ProxyUser>,T> action) {
            return DoUserStore(store=>{
			    using(var manager = new UserManager<ProxyUser>(store)) {
			        manager.PasswordValidator = new PasswordValidator() { RequiredLength  = 1 };

			        return action(manager);
                }
            });
		}

        //private static UserStore<ProxyUser> GetUserStore() {
        //    var dbContext = new DataContext();
        //    return new UserStore<ProxyUser>(dbContext);
        //}

        static T DoUserStore<T>(Func<UserStore<ProxyUser>,T> action) {
            return DataContext.Do(ctx=> {
                using(var userStore = new UserStore<ProxyUser>(ctx)) {
                    return action(userStore);
                }
            });
        }

		internal static bool HasAUser() {
            return DoUserStore(store=>store.Users.Any());
		}

		internal static ProxyUser FindOrNull(string username,string password) {
			return DoUserManager(manager=>manager.Find(username,password));
		}

        internal static System.Security.Claims.ClaimsIdentity CreateIdentity(ProxyUser user,string authenticationType) {
            return DoUserManager(manager=>{
                return manager.CreateIdentity(user,authenticationType);
            });
        }
    }
}