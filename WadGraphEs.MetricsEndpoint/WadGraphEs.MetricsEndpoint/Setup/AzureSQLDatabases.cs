using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.MVC.ViewModels;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AzureSQLDatabases {
		internal static TestConnectionResult TestConnection(MVC.Commands.CreateAzureSQLDatabaseCommand cmd) {
			return SQLDatabaseUsageClient.CreateServerUsagesClient(cmd.Servername,cmd.Username,cmd.Password).TestConnection();
		}

		internal static bool AlreadyHasServer(string servername) {
			var normalized = SQLDatabaseUsageClient.NormalizeServername(servername);

			return DataContext.Do(ctx=>{
				var db = ctx.SQLDatabases.FirstOrDefault(_=>_.Servername == normalized);
				return db != null;
			});
		}

		
		internal static void StoreInSession(string sessionId,MVC.Commands.CreateAzureSQLDatabaseCommand cmd) {
			DataContext.Do(ctx=>{
				ctx.AddSQLDatabaseSessions.Add(new AddSQLDatabaseSession {
					SessionId = sessionId,
					Username = cmd.Username,
					Password = cmd.Password,
					Servername = cmd.Servername
				});
				ctx.SaveChanges();
			});
		}

		internal static CreateAzureSQLDatabaseCommand RetrieveCreateCommandFromSession(string sessionId) {
			return DataContext.Do(ctx=>{
				var result = ctx.AddSQLDatabaseSessions.Find(sessionId);

				return new CreateAzureSQLDatabaseCommand {
					Password = result.Password,
					Servername = result.Servername,
					SessionId = result.SessionId,
					Username = result.Username
				};
			});
		}

		internal static object GetFinishCommandForSession(string sessionId) {
			return DataContext.Do(ctx=>{

				var session = ctx.AddSQLDatabaseSessions.Find(sessionId);

				return new FinishAddingSQLDatabase {
					Servername = session.Servername,
					SessionId = sessionId,
					Username = session.Username,
					Version = GetVersionFromSession(session)
				};
			});
		}

		private static string GetVersionFromSession(AddSQLDatabaseSession session) {
			return SQLDatabaseUsageClient.CreateServerUsagesClient(session.Servername,session.Username,session.Password).GetVersionString();
		}

		internal static void FinishSession(string sessionId) {
			DataContext.Do(ctx=>{
				var session = ctx.AddSQLDatabaseSessions.Find(sessionId);
				ctx.SQLDatabases.Add(new SQLDatabase {
					Servername = SQLDatabaseUsageClient.NormalizeServername(session.Servername),
					Password = session.Password,
					Username  = session.Username,
					Version = GetVersionFromSession(session)
				});
				ctx.SaveChanges();
			});
		}

		internal static List<ServiceViewModel> ListForOverview() {
			return DataContext.Do(ctx=>{
				return ctx.SQLDatabases.ToList().Select(_=>new ServiceViewModel { Name = _.Servername, Record = _ }).ToList();
			});
		}

		internal static ICollection<SQLDatabase> ListAll() {
			return DataContext.Do(ctx=>{
				return ctx.SQLDatabases.ToList();
			});
		}

        internal static ICollection<ChartInfo> ListAllCharts() {
            return ListAll().SelectMany(ListAllCharts).ToList();
        }

        private static IEnumerable<ChartInfo> ListAllCharts(SQLDatabase sqlDatabase) {
            return ChartsFacade.ListAllChartsForSqlDatabaseServer(sqlDatabase.Servername,sqlDatabase.Username,sqlDatabase.Password);
        }

        internal static SqlCredentials GetCredentials(string forServer) {
            return DataContext.Do(ctx=>{
                forServer = SQLDatabaseUsageClient.NormalizeServername(forServer);
                var record = ctx.SQLDatabases.FirstOrDefault(_=>_.Servername == forServer);
                if(record == null) {
                    throw new InvalidOperationException(string.Format("Couldn't fetch credentials for database server {0}", forServer));
                }

                return record.Credentials;
            });
        }
    }
}