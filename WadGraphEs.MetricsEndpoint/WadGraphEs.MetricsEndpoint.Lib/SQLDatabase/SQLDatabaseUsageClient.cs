using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	public class SQLDatabaseUsageClient {
		public static ServerUsagesClient CreateServerUsagesClient(string servername,string username,string password) {
			var connection = new SQLDatabaseConnection(servername,username,password,"master");
			
			var version = connection.GetVersion();

			switch(version) {
				case SQLDatabaseVersion.V11:
					return new V11ServerUsagesClient(connection);
			}

			throw new Exception(string.Format("Version not supported {0}", version));
		}
	}
}
