using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace AzurePlot.Web.DataAccess {
	public class SQLDatabase {
		[Key]
		[MaxLength(256)]
		public string Servername{get;set;}
		[MaxLength]
		public string Username{get;set;}
		[MaxLength]
		public string Password{get;set;}
		[MaxLength]
		public string Version{get;set;}

		public string FormatName() {
			return string.Format("{0}@{1}", Username,Servername);
		}



        public Lib.SqlCredentials Credentials {
            get {
                return new Lib.SqlCredentials { Username = Username, Password = Password };
            }
        }
    }
}
