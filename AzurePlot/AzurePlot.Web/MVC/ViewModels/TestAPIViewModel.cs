using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AzurePlot.Web.MVC.Commands;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.MVC.ViewModels {
    public class TestAPIViewModel {
        public TestAPICommand Command{get;set;}
        public TestAPIResult Result{get;set;}
    }
}