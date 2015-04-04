using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	public class SQLDatabaseUsageClient {
		readonly SQLDatabaseConnection _connection;

		SQLDatabaseUsageClient(SQLDatabaseConnection connection) {
			_connection = connection;
		}
		public static SQLDatabaseUsageClient CreateServerUsagesClient(string servername,string username,string password) {
			return new SQLDatabaseUsageClient(new SQLDatabaseConnection(servername,username,password,"master"));
			
		}

		public TestConnectionResult TestConnection() {
			var result = _connection.TestOpenConnection();
			if(result.Failed) {
				return result;
			}
			return TestVersion();
		}

		private TestConnectionResult TestVersion() {
			var version = GetVersion();
			if(version.Version == SQLDatabaseVersionEnum.Unknown) {
				return new TestConnectionResult(string.Format("Not supported SQL Server version ({0}), currently only V11 databases are supported",version.DetailedVersion), null);
			}

			return TestConnectionResult.Success;
		}

		SQLDatabaseVersion _version = null;

		private SQLDatabaseVersion GetVersion() {
			if(_version == null) {
				_version = _connection.GetVersion();
			}
			return _version;
		}

		public ICollection<UsageObject> GetUsages(DateTime fromTimeUTC) {
			return GetUsagesClient().GetUsages(fromTimeUTC);
		}

		private ServerUsagesClient GetUsagesClient() {
			var version = GetVersion();
			switch(version.Version) {
				case SQLDatabaseVersionEnum.V11:
					return new V11ServerUsagesClient(_connection);
			}

			throw new Exception(string.Format("Version not supported {0}",version));
		}

		public static string NormalizeServername(string servername) {
			return SQLDatabaseConnection.NormalizeServername(servername);
		}

		public string GetVersionString() {
			return GetVersion().DetailedVersion;
		}		
	}
}
