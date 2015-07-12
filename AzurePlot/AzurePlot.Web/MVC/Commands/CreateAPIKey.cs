using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AzurePlot.Web.MVC.Commands {
	public class CreateAPIKey {
		[Required]
		[MinLength(10, ErrorMessage="APIKey should at least be 10 characters long")]
		public string APIKey{get;set;}

		internal static CreateAPIKey NewRandom() {
			var guid = Guid.NewGuid();
			var sb = new StringBuilder();
			using(var md5 = MD5.Create()) {
				var hash = md5.ComputeHash(guid.ToByteArray());
				foreach(var @byte in hash) {
					sb.Append(@byte.ToString("x2"));
				}
				return new CreateAPIKey { APIKey = sb.ToString() };
			}
		}
	}
}