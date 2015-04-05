using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WadGraphEs.MetricsEndpoint {
    public class BundleConfig {
        internal static void RegisterBundles(System.Web.Optimization.BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/scripts/layout").Include(
                "~/Scripts/jquery-1.9.1.js",
                "~/Scripts/bootstrap.js"
            ));

            bundles.Add(new StyleBundle("~/content/layout").Include(
                "~/content/bootstrap.css"
                
            ));

            bundles.Add(new StyleBundle("~/content/setup").Include(
                "~/content/setup.css"
            ));

			bundles.Add(new StyleBundle("~/content/login").Include(
                "~/content/login.css"
            ));

			bundles.Add(new StyleBundle("~/content/dashboard").Include(
                "~/content/dashboard.css"
            ));

			bundles.Add(new ScriptBundle("~/scripts/dashboard/dashboard")
				.IncludeDirectory("~/scripts/dashboard", "*.js")
			);
        }
    }
}