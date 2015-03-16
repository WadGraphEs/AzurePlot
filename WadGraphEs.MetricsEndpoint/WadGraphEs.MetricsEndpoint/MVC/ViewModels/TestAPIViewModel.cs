using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.MVC.Commands;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.MVC.ViewModels {
    public class TestAPIViewModel {
        public TestAPICommand Command{get;set;}
        public TestAPIResult Result{get;set;}
    }
}