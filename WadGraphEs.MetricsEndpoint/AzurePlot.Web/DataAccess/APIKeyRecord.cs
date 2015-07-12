using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace AzurePlot.Web.DataAccess {
	public class APIKeyRecord {
		[Key]
		public int Id{get;set;}
		[MaxLength(128)]
		[Index]
		public string APIKey{get;set;}
	}
}
