using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzurePlot.Web.MVC.HtmlHelpers {
	public static class CSSClassHelpers {
		readonly static Dictionary<NLog.LogLevel,string> _logLevelToClassMap = new Dictionary<NLog.LogLevel,string> {
			{ NLog.LogLevel.Error, "danger" },
			{ NLog.LogLevel.Warn, "warning" },
			{ NLog.LogLevel.Fatal, "danger" },
		};
		public static string GetLogLevelClass(this HtmlHelper helper, NLog.LogLevel level) {
			if(!_logLevelToClassMap.ContainsKey(level)) {
				return "";
			}
			return _logLevelToClassMap[level];
		}
	}
}