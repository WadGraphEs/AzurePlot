using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.Lib.SQLDatabase {
	public class TestConnectionResult {
		public readonly static TestConnectionResult Success =new TestConnectionResult();

		internal static TestConnectionResult FromSQLException(System.Data.SqlClient.SqlException e) {
			switch(e.Number) {
				case SQLErrorNumbers.ErrorEstablishingConnectionToServer:
					return new TestConnectionResult("Error establishing a connection to the server. Is the servername correct?", e);
				case SQLErrorNumbers.DisallowedByFirewall:
					return new TestConnectionResult("The firewall rules on the Azure SQL Database disallowed access for this host. Did you configure the firewall rules?", e);
				case SQLErrorNumbers.LoginFailed:
					return new TestConnectionResult("Login failed, are the username and password correct?", e);
				case SQLErrorNumbers.CannotOpenDatabase:
					return new TestConnectionResult("Cannot open database with this login. Does the user exist on the database?", e);
			}
			return new TestConnectionResult(string.Format("Unknown error (SQL Server Error number: {0}):\n{1}", e.Number,e), e);
		}

		

		readonly bool _failed;
		readonly System.Data.SqlClient.SqlException _exception;
		readonly string _message;
		public bool Failed {
			get { return _failed; }
		} 

		

		public string Message {
			get { return _message; }
		} 

		

		public System.Data.SqlClient.SqlException Exception {
			get { return _exception; }
		}

		
		public TestConnectionResult(string message,System.Data.SqlClient.SqlException e) {
			_failed = true;
			_exception = e;
			_message = message;
		}

		TestConnectionResult() {
			_failed = false;
			_exception = null;
			_message = "Connection successful";
		}

		public override string ToString() {
			if(!_failed) {
				return "Success";
			}

			var sb = new StringBuilder("Connection failed");
			sb.AppendLine(_message);
			if(_exception!=null) {
				sb.AppendLine(_exception.ToString());
			}			

			return sb.ToString();
		}
	}
}
