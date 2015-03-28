using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;
using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;
using WadGraphEs.MetricsEndpoint.MVC.Commands;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AzureSQLDatabases {
		internal static TestConnectionResult TestConnection(MVC.Commands.CreateAzureSQLDatabaseCommand cmd) {
			return SQLDatabaseUsageClient.CreateServerUsagesClient(cmd.Servername,cmd.Username,cmd.Password).TestConnection();
		}

		internal static bool AlreadyHasServer(string servername) {
			var normalized = SQLDatabaseUsageClient.NormalizeServername(servername);

			using(var ctx = GetDataContext()) {
				var db = ctx.SQLDatabases.FirstOrDefault(_=>_.Servername == normalized);
				return db != null;
			}
		}

		private static DataContext GetDataContext() {
			return new DataContext();
		}

		
		internal static void StoreInSession(string sessionId,MVC.Commands.CreateAzureSQLDatabaseCommand cmd) {
			using(var ctx = GetDataContext()) {
				ctx.AddSQLDatabaseSessions.Add(new AddSQLDatabaseSession {
					SessionId = sessionId,
					Username = cmd.Username,
					Password = cmd.Password,
					Servername = cmd.Servername
				});
				ctx.SaveChanges();
			}
		}

		internal static CreateAzureSQLDatabaseCommand RetrieveCreateCommandFromSession(string sessionId) {
			using(var ctx=  GetDataContext()) {
				var result = ctx.AddSQLDatabaseSessions.Find(sessionId);

				return new CreateAzureSQLDatabaseCommand {
					Password = result.Password,
					Servername = result.Servername,
					SessionId = result.SessionId,
					Username = result.Username
				};
			}
		}

		internal static object GetFinishCommandForSession(string sessionId) {
			using(var ctx = GetDataContext()) {

				var session = ctx.AddSQLDatabaseSessions.Find(sessionId);

				return new FinishAddingSQLDatabase {
					Servername = session.Servername,
					SessionId = sessionId,
					Username = session.Username,
					Version = SQLDatabaseUsageClient.CreateServerUsagesClient(session.Servername,session.Username,session.Password).GetVersionString()
				};
			}
		}

		internal static void FinishSession(string sessionId) {
			using(var ctx = GetDataContext()) {
				var session = ctx.AddSQLDatabaseSessions.Find(sessionId);
				ctx.SQLDatabases.Add(new SQLDatabase {
					Servername = SQLDatabaseUsageClient.NormalizeServername(session.Servername),
					Password = session.Password,
					Username  = session.Username
				});
				ctx.SaveChanges();
			}
		}
	}
}