using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WadGraphEs.MetricsEndpoint {
    public class BundleConfig {
        internal static void RegisterBundles(System.Web.Optimization.BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/scripts/layout.js").Include(
                "~/Scripts/jquery-1.9.1.js",
                "~/Scripts/bootstrap.js"
            ));

            bundles.Add(new StyleBundle("~/content/layout.css").Include(
                "~/content/bootstrap.css"
                
            ));

            bundles.Add(new StyleBundle("~/content/setup").Include(
                "~/content/setup.css"
            ));
        }
    }
}