using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	class V11ServerUsagesClient : ServerUsagesClient{
		readonly SQLDatabaseConnection _connection;

		public V11ServerUsagesClient(SQLDatabaseConnection connection) {
			_connection = connection;
		}
		public ICollection<UsageObject> GetUsages(DateTime from) {
			var result = new List<UsageObject>();
			using(var connection = _connection.GetConnection()) {
				connection.Open();
				var cmd =connection.CreateCommand();
				cmd.CommandText = "select * from sys.resource_stats where start_time > @from";
				cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("from", from));
					
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						result.AddRange(GetResultFromReader(reader));
					}
				}
			}
			return result;
		}

		readonly static string[] Columns = new string[] { "usage_in_seconds" };

		private ICollection<UsageObject> GetResultFromReader(System.Data.SqlClient.SqlDataReader reader) {
			var databaseName = (string)reader["database_name"];
			return new UsageObject[0];
		}


		public void TestConnection() {
			_connection.TestOpenConnection();
		}
	}
}
