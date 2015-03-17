using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.Logging {
	[Target("ViewableLogTarget")]
	public class ViewableLogTarget : TargetWithLayout{
		const int MaxLines = 5;
		readonly static CircularBuffer<LogMessage> _buffer = new CircularBuffer<LogMessage>(MaxLines);

		protected override void Write(NLog.LogEventInfo logEvent) {
			var	msg = AppendException(logEvent.FormattedMessage, logEvent.Exception);
			
			_buffer.Add(new LogMessage {
				Level = logEvent.Level,
				Message = msg,
				Source = logEvent.LoggerName,
				Timestamp = logEvent.TimeStamp
			});	
		}

		private string AppendException(string msg,Exception exception) {
			if(exception == null) {
				return msg;
			}
			var sb = new StringBuilder(msg);
			sb.AppendLine();
			sb.AppendLine(exception.ToString());
			if(exception.InnerException == null) {
				return sb.ToString();
			}
			return AppendException(sb.ToString(),exception.InnerException);
		}

		public static ICollection<LogMessage> GetLatestMessages() {
			return _buffer.ToCollection();
		}
	}
}