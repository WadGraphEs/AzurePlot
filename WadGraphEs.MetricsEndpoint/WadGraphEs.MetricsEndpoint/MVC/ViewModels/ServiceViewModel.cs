using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.ViewModels {
	public class ServiceViewModel {
		public string Name { get; set; }
		
		public int Id { get; set; }

		public object Record { get; set; }
	}
}