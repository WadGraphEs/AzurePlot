using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class LoginCommand {
		public string Username { get; set; }

		public string Password { get; set; }
	}
}