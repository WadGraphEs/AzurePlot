using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Factories {
	public class MetricsConfigEndpointConfigurationFactory {
		internal static MetricsConfigEndpointConfiguration New() {
			return new MetricsConfigEndpointConfiguration(HttpContext.Current.Server.MapPath("~"));
		}
	}
}