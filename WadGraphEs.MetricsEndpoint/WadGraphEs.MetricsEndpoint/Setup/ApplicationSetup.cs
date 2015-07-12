using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AzurePlot.Web.DataAccess;

namespace AzurePlot.Web.Setup {
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

            ResetPendingMigrations();
		}

		private static System.Data.Entity.Migrations.DbMigrator GetDbMigrator() {
			var dbMigrator = new System.Data.Entity.Migrations.DbMigrator(new Migrations.Configuration());
			return dbMigrator;
		}

		private static void CreateDatabase() {
			DataContext.CreateDatabase();
			
		}

		public static bool IsDatabaseCreated() {
			
			return DataContext.IsDatabaseCreated();
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

		public static bool IsSchemaUpToDate() {
			return !GetPendingMigrations().Any();
		}

		internal static bool IsApplicationConfigured() {
			if(!IsDatabaseCreated()) {
				return false;
			}

			if(!IsSchemaUpToDate()) {
				return false;
			}

			if(!HasAdminUser()) {
				return false;
			}

			if(!IsAPIKeyCreated()) {
				return false;
			}
			
			return true;		
		}

		public static bool IsAPIKeyCreated() {
			return APIEndpoint.IsAPIKeyCreated();
		}


        static Lazy<ICollection<string>> _pendingMigrations;

        static ApplicationSetup() {
            ResetPendingMigrations();
        }

        private static void ResetPendingMigrations() {
            _pendingMigrations = new Lazy<ICollection<string>>(() => GetDbMigrator().GetPendingMigrations().ToList());
        }

		internal static ICollection<string> GetPendingMigrations() {
            return _pendingMigrations.Value;
		}
	}
}