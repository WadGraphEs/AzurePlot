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

		readonly static string[] NotCounterColumns = new string[] { "start_time", "end_time","sku", "database_name"};

		private ICollection<UsageObject> GetResultFromReader(System.Data.SqlClient.SqlDataReader reader) {
			var databaseName = (string)reader["database_name"];
			var start_time = (DateTime)reader["start_time"];
			var result = new List<UsageObject>();
			for(var i=0;i<reader.FieldCount;i++) {
				var name = reader.GetName(i);
				if(NotCounterColumns.Contains(name)) {
					continue;
				}

				var valueResult = GetDouble(reader[i]);

				if(valueResult == null) {
					continue;
				}

				result.Add(new UsageObject { Timestamp = start_time.ToString("o"), Value = (double)valueResult, GraphiteCounterName = GetCounterName(databaseName,name) });
			}

			return result;
		}

		private static double? GetDouble(object value) {
			double? valueResult=null;

			if(value is decimal) {
				valueResult = (double)(decimal)value;
			}
			if(value is int) {
				valueResult = (double)(int)value;
			}
			if(value is long) {
				valueResult = (double)(long)value;
			}
			if(value is double) {
				valueResult = (double)value;
			}
			return valueResult;
		}

		private string GetCounterName(string database, string name) {
			return new GraphiteCounterName("Azure.SQLDatabase", _connection.Servername, database, name).ToString();
		}


        public List<string> ListDatabases() {
            var result = new List<string>();

			using(var connection = _connection.GetConnection()) {
				connection.Open();
				var cmd =connection.CreateCommand();
				cmd.CommandText = "select distinct(database_name) from sys.resource_stats";
					
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						result.Add((string)reader["database_name"]);
					}
				}
			}
			return result;
        }
    }
}
