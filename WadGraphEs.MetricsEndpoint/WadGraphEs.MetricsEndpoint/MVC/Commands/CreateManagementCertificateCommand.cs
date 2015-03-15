using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class CreateManagementCertificateCommand {
		
		[Required(ErrorMessage="Please enter a non-empty certificate name")]
		public string CertificateName{get;set;}

		public string SessionId { get; set; }
	}
}