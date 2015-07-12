using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AzurePlot.Lib.SQLDatabase {
	class SQLDatabaseConnection {
		readonly string _servername;
		readonly string _username;
		readonly string _password;
		readonly string _database;

		public SQLDatabaseConnection(string servername,string username,string password,string database) {
			_servername = NormalizeServername(servername);
			_username = username;
			_password = password;
			_database = database;
		}

		public static string NormalizeServername(string servername) {
			if(!servername.Contains(".")) {
				servername = string.Format("{0}.database.windows.net",servername);
			}
			return servername;
		}

		internal SQLDatabaseVersion GetVersion() {
			return SQLDatabaseVersion.FromProductVersionString(GetSqlProductVersion());
			
		}
		string GetSqlProductVersion() {
			
			using(var sqlConnection = GetConnection()) {	
				sqlConnection.Open();
				var cmd = sqlConnection.CreateCommand();
				cmd.CommandText = @"select serverproperty('productversion')";
				return (string)cmd.ExecuteScalar();
			}
		}

		public SqlConnection GetConnection() {
			return GetConnection(x=>{});
		}

		public SqlConnection GetConnection(Action<SqlConnectionStringBuilder> builder) {
			return new SqlConnection(BuildConnectionString(builder));
		}

		private string BuildConnectionString(Action<SqlConnectionStringBuilder> builderFilter) {
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = _servername;
			builder.IntegratedSecurity = false;
			builder.UserID = _username;
			builder.Password = _password;
			builder.InitialCatalog = _database;
			builderFilter(builder);
			return builder.ConnectionString;
		}



		internal TestConnectionResult TestOpenConnection() {
			using(var sqlConnection = GetConnection(p=>p.ConnectTimeout=5)) {
				try {
					sqlConnection.Open();
					return TestConnectionResult.Success;
				}
				catch(SqlException e){
					return TestConnectionResult.FromSQLException(e);
				}
			}
		}


		public string Servername {
			get {
				return _servername.Split('.')[0];
			}
		}

		public string Database {
			get {
				return _database;
			}
		}
	}
}
