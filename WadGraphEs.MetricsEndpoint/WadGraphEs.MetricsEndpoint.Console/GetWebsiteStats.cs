using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Console {
    using Newtonsoft.Json;
    using Console = System.Console;
    class GetWebsiteStats {
        string _website;
        private TimeSpan _history;
        private MetricsConfigEndpointConfiguration _config;
        private ICollection<string> _filters;
        
        public GetWebsiteStats(List<string> args) {
            _config = new MetricsConfigEndpointConfiguration(args[0]);
            _website = args[1];
            _history = TimeSpan.Parse(args[2]);
            _filters = new List<string>();
            if(args.Count>3) {
                _filters = new [] { args[3] };
            }
        }

        internal void PrintStats() {
            var usages = GetUsageClient()
				.GetWebsitesUsageForWebsite(GetWebspace(), _website,_history, _filters.ToArray())
				.Result;

			Console.WriteLine(JsonConvert.SerializeObject(usages,Formatting.Indented));
        }

        private string GetWebspace() {
            return AzureWebsitesInfoApiClientFacade.FindWebspace(_config,_website);
        }

        private AzureUsageClient GetUsageClient() {
            return new AzureUsageClient(_config);
        }
    }
}
