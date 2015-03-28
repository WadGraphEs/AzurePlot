using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class AddSQLDatabaseSession {
		[MaxLength(128)]
		[Key]
		public string SessionId { get; set; }
		[MaxLength]
		public string Username { get; set; }
		[MaxLength]
		public string Servername { get; set; }

		[MaxLength]
		public string Password { get; set; }
	}
}
