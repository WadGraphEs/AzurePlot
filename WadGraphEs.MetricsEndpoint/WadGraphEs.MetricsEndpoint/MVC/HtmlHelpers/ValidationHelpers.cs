using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WadGraphEs.MetricsEndpoint.MVC.HtmlHelpers {
	public static class ValidationHelpers {
		public static bool HasValidationErrors(this HtmlHelper helper, string fieldName) {
			var state = helper.ViewData.ModelState[fieldName];
			if(state == null) {
				return false;
			}
			return helper.ViewData.ModelState[fieldName].Errors.Any();
		}

		public static string GetValidationClass(this HtmlHelper helper,string fieldname) {
			if(helper.ViewData.ModelState.IsValid) {
				return "";
			}
			var modelState = helper.ViewData.ModelState[fieldname];

			if(!modelState.Errors.Any()) {
				return "has-success";
			}

			return "has-error";
		}
	}
}