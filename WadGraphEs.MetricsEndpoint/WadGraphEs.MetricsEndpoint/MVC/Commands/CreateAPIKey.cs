using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class CreateAPIKey {
		[Required]
		public string APIKey{get;set;}
	}
}