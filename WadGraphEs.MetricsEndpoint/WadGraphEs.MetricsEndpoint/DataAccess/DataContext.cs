using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.DataAccess {
	public class DataContext : IdentityDbContext<ProxyUser>{
		public DataContext() : base("wadgraphes-proxy"){
			Database.SetInitializer<DataContext>(null);
		}

		public DbSet<APIKeyRecord> APIKeys{get;set;}
		public DbSet<AddAzureSubscriptionSession> AddAzureSubscriptionSessions{get;set;}

		public DbSet<AzureSubscription> AzureSubscriptions{get;set;}

		public DbSet<SQLDatabase> SQLDatabases{get;set;}
		public DbSet<AddSQLDatabaseSession> AddSQLDatabaseSessions{get;set;}

		public DbSet<DashboardChart> DashboardCharts{get;set;}

        static object _synchronization = new object();

        public static T Do<T>(Func<DataContext,T> action) {
            lock(_synchronization) {
                using(var ctx = new DataContext()) {
                    return action(ctx);
                }
            }
        }

        public static void Do(Action<DataContext> action) {
            lock(_synchronization) {
                using(var ctx = new DataContext()) {
                    action(ctx);
                }
            }
        }

        internal static void CreateDatabase() {
            Do(ctx=>ctx.Database.Create());
        }

        internal static bool IsDatabaseCreated() {
            return Do(ctx=>ctx.Database.Exists());
        }
    }
}