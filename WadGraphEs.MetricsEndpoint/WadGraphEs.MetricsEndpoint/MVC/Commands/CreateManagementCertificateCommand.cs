using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
	public class CreateManagementCertificateCommand {
		public string Passphrase{get;set;}
		[Required(ErrorMessage="Please enter a non-empty certificate name")]
		public string CertificateName{get;set;}
	}
}