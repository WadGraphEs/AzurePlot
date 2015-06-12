using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Console {
    using Newtonsoft.Json;
    using Console = System.Console;
    class DisplayChartData {
        private List<string> _optionalArguments;
        private string _uri;
        
        public DisplayChartData(List<string> args) {
            _uri  =args[0];
            _optionalArguments = args.Skip(1).ToList();            
        }

        internal void PrintData() {
            var facade = new ChartDataFacade(_uri);

            facade.SubscriptionCredentialsProvider = GetSubscriptionCredentials;

            var usages = facade.FetchChartData().Result;

			Console.WriteLine(JsonConvert.SerializeObject(usages,Formatting.Indented));
        }

        private MetricsEndpointConfiguration GetSubscriptionCredentials(string subscriptionId) {
            var config = new MetricsConfigEndpointConfiguration(_optionalArguments.First());

            if(config.SubscriptionId != subscriptionId) {
                throw new InvalidOperationException("SubscriptionIds don't match");
            }

            return config;
        }

        
    }
}
