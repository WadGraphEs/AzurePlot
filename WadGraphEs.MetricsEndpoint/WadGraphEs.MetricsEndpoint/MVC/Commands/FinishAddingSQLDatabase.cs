using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class FinishAddingSQLDatabase {
		public string Servername { get; set; }

		public string Username { get; set; }

		public string Version { get; set; }
		public string SessionId{get;set;}
	}
}