using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace AzurePlot.Web {
    public class BundleConfig {
        internal static void RegisterBundles(System.Web.Optimization.BundleCollection bundles) {
			AddScripts(bundles);
			AddStyles(bundles);
        }

		private static void AddScripts(System.Web.Optimization.BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/scripts/layout")
				.Include(
					"~/Scripts/jquery-1.9.1.js",
					"~/Scripts/bootstrap.js"
				)
				.IncludeDirectory("~/scripts/common","*.js")
			);

			bundles.Add(new ScriptBundle("~/scripts/dashboard/dashboard")
				.Include("~/scripts/typeahead.bundle.js")
                .Include("~/scripts/dashboard/000-init.js")
                .Include("~/scripts/dashboard/100-chart-interval-selector.js")
                .Include("~/scripts/dashboard/500-add-chartlist.js")
                .Include("~/scripts/dashboard/500-charts.js")
			);
		}

		private static void AddStyles(System.Web.Optimization.BundleCollection bundles) {
			bundles.Add(new StyleBundle("~/content/layout").Include(
				"~/content/bootstrap.css",
				"~/content/layout.css"

			));

			bundles.Add(new StyleBundle("~/content/setup").Include(
				"~/content/setup.css"
			));

			bundles.Add(new StyleBundle("~/content/login").Include(
				"~/content/login.css"
			));

			bundles.Add(new StyleBundle("~/content/home").Include(
				"~/content/home.css"
			));

			bundles.Add(new StyleBundle("~/content/dashboard/dashboard").Include(
				"~/content/dashboard/dashboard.css"
			));
		}
    }
}