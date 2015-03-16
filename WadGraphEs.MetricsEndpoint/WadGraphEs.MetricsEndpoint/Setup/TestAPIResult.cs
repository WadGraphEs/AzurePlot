using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Setup {
    public class TestAPIResult {
        internal static TestAPIResult FromException(Exception e) {
            return new TestAPIResult {
                Success = false,
                Message = e.ToString()
            };
        }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
