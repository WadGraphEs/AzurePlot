using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzurePlot.Web.MVC.HtmlHelpers {
	public static class ValidationHelpers {
		public static bool HasValidationErrors(this HtmlHelper helper, string fieldName) {
			var state = helper.ViewData.ModelState[fieldName];
			if(state == null) {
				return false;
			}
			return helper.ViewData.ModelState[fieldName].Errors.Any();
		}

		public static ICollection<String> GetValidationErrors(this HtmlHelper helper, string fieldName) {
			if(!helper.HasValidationErrors(fieldName)) {
				return new string[0];
			}
			return helper.ViewData.ModelState[fieldName].Errors.Select(ErrorToString).ToList();
		}

		private static string ErrorToString(ModelError modelError) {
			if(modelError.Exception!=null) {
				return modelError.Exception.ToString();
			}
			return modelError.ErrorMessage;
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