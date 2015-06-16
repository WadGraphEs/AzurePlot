using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Console {
    using Console = System.Console;
    class ListAllChartsForSubscription {
        readonly MetricsConfigEndpointConfiguration _config;
        readonly Lazy<string> _subscriptionName;

        public ListAllChartsForSubscription(List<string> args) {
            _config = new MetricsConfigEndpointConfiguration(args[0]);
            _subscriptionName = new Lazy<string>(FindSubcriptionName);
        }

        private string FindSubcriptionName() {
            return AzureSubscriptionInfoFacade.GetSubcriptionName(_config);
        }

        internal void Print() {
            var charts = ChartsFacade.ListAllChartsForSubscription(_config,_subscriptionName.Value).Result;

            foreach(var chart in charts) {
                Console.WriteLine("{0} {1}", chart.Name, chart.Uri);
            }
        }
    }
}
