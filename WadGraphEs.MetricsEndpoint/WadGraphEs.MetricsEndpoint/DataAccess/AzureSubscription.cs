using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.DataAccess {
	public class AzureSubscription {
		[Key]
		public int Id{get;set;}
		public string Name { get; set; }

		public string FromSessionId { get; set; }

		public string AzureSubscriptionId { get; set; }

		public DateTime AddedOnUtc { get; set; }
	
		[MaxLength]
		public byte[] Pfx { get; set; }

		public string Password { get; set; }
	}
}
