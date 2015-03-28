using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	class SQLDatabaseVersion {
		readonly SQLDatabaseVersionEnum _version;
		readonly string _detailedVersion;

		public SQLDatabaseVersionEnum Version {
			get { return _version; }
		}

		public string DetailedVersion {
			get { return _detailedVersion; }
		}

		public SQLDatabaseVersion(SQLDatabaseVersionEnum version,string detailedVersion) {
			_version = version;
			_detailedVersion = detailedVersion;
		}
		internal static SQLDatabaseVersion FromProductVersionString(string stringVersion) {
			var version = stringVersion.Split('.').First();
			switch(version) {
				case "11":
					return new SQLDatabaseVersion(SQLDatabaseVersionEnum.V11,stringVersion);
			}
			return new SQLDatabaseVersion(SQLDatabaseVersionEnum.Unknown, stringVersion);
		}
	}
}
