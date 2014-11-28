using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WadGraphEs.MetricsEndpoint.Controllers {
	[AllowAnonymous]
	public class StatusController : ApiController{
		public string Get() {
			return "Running";
		}
	}
}