using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzurePlot.Web.MVC.Commands {
	public class EnterSubscriptionIdCommand {
		public string SessionId{get;set;}
		[Required(ErrorMessage="Please enter a valid subscription id")]
		public string AzureSubscriptionId{get;set;}
	}
}