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
			if(IsSchemaUpToDate()) {
				return;
			}
			var dbMigrator = GetDbMigrator();

			dbMigrator.Update();
		}

		private static System.Data.Entity.Migrations.DbMigrator GetDbMigrator() {
			var dbMigrator = new System.Data.Entity.Migrations.DbMigrator(new WadGraphEs.MetricsEndpoint.Migrations.Configuration());
			return dbMigrator;
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
			if(!IsDatabaseCreated()) {
				return false;
			}
			if(!IsSchemaUpToDate()) {
				return false;
			}
			return Users.HasAUser();
		}

		private static bool IsSchemaUpToDate() {
			var migrator = GetDbMigrator();
			return !migrator.GetPendingMigrations().Any();
		}
	}
}