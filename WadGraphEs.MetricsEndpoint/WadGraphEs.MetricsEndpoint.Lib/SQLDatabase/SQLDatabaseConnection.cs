using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	class SQLDatabaseConnection {
		readonly string _servername;
		readonly string _username;
		readonly string _password;
		readonly string _database;

		public SQLDatabaseConnection(string servername,string username,string password,string database) {
			if(!servername.Contains(".")) {
				servername = string.Format("{0}.database.windows.net",servername);
			}
			_servername = servername;
			_username = username;
			_password = password;
			_database = database;
		}

		internal SQLDatabaseVersion GetVersion() {
			var stringVersion = GetSqlProductVersion();
			var version = stringVersion.Split('.').First();
			switch(version) {
				case "11":
					return SQLDatabaseVersion.V11;
			}
			return SQLDatabaseVersion.Unknown;
		}
		string GetSqlProductVersion() {
			
			using(var sqlConnection = GetConnection()) {	
				sqlConnection.Open();
				var cmd = sqlConnection.CreateCommand();
				cmd.CommandText = @"select serverproperty('productversion')";
				return (string)cmd.ExecuteScalar();
			}
		}

		private SqlConnection GetConnection() {
			return new SqlConnection(BuildConnectionString());
		}

		private string BuildConnectionString() {
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = _servername;
			builder.IntegratedSecurity = false;
			builder.UserID = _username;
			builder.Password = _password;
			builder.InitialCatalog = _database;
			return builder.ConnectionString;
		}

		
	}
}
