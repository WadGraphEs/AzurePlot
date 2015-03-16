using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
    public class TestAPICommand {
        public string EndpointUrl { get; set; }

        public string APIKey { get; set; }

        public string Accept { get; set; }
    }
}