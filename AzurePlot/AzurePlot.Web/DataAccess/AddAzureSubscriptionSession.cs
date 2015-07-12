using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.DataAccess {
	public class AddAzureSubscriptionSession {
		[Key]
		[MaxLength(128)]
		public string SessionId{get;set;}
		public string Password{get;set;}
		[MaxLength]
		public byte[] Pfx{get;set;}

		public string AzureSubscriptionId { get; set; }

        public Lib.MetricsEndpointConfiguration MetricsEndpointConfiguration {
            get {
                return new FromPKCSMetricsEndpointConfiguration(Pfx,Password,AzureSubscriptionId);
            }
        }
    }
}
