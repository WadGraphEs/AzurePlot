using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WadGraphEs.MetricsEndpoint.MVC.HtmlHelpers {
    public static class JsonHelper {
        public static string ToJson(this HtmlHelper html, object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}