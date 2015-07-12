using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzurePlot.Web.MVC.ViewModels {
	public class OverviewViewModel {
		public bool HasConfiguredServices{
			get {
				return Services.Any();
			}
		}

		public ICollection<ServiceViewModel> Services { get; set; }
	}
}