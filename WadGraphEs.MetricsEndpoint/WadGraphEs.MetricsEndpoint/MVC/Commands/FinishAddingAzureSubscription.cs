using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class FinishAddingAzureSubscription {
		public string SessionId{get;set;}
		public string AzureSubscriptionId{get;set;}
		public string AzureSubcriptionName{get;set;}

	}
}