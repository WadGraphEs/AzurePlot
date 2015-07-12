using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzurePlot.Web.MVC.Commands {
	public class CreateAzureSQLDatabaseCommand {
		[Required]
		public string Username{get;set;}
		[Required]
		public string Password{get;set;}
		[Required]
		public string Servername{get;set;}

		public string SessionId { get; set; }
	}
}