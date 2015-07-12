using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.Web.Logging {
	public class LogMessage {
		public NLog.LogLevel Level { get; set; }

		public string Message { get; set; }

		public string Source { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
