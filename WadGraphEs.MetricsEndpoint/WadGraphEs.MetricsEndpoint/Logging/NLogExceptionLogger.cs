using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace WadGraphEs.MetricsEndpoint.Logging {
    public class NLogExceptionLogger : ExceptionLogger{
        readonly static Logger _logger = LogManager.GetLogger("WebApi_Exceptions");
        public override void Log(ExceptionLoggerContext context) {
            _logger.Log(LogLevel.Error, context.Exception.Message, context.Exception);            
        }
    }
}