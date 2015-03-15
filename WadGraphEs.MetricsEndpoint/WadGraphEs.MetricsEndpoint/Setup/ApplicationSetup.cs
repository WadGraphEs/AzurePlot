using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class ApplicationSetup {
		internal static void UpdateDatabaseToLatestSchema() {
			if(!IsDatabaseCreated()) {
				CreateDatabase();
			}

			UpdateSchema();
		}

		private static void UpdateSchema() {
			var dbMigrator = new System.Data.Entity.Migrations.DbMigrator(new WadGraphEs.MetricsEndpoint.Migrations.Configuration());

			var migrations = dbMigrator.GetPendingMigrations();

			if(migrations.Any()) {
				dbMigrator.Update();
			}
		}

		private static void CreateDatabase() {
			var dbContext = new DataContext();
			dbContext.Database.Create();
		}

		private static bool IsDatabaseCreated() {
			var dbContext = new DataContext();
			return dbContext.Database.Exists();
		}

		internal static bool HasAdminUser() {
			return Users.HasAUser();
		}
	}
}