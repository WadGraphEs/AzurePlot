using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Console {
	using Newtonsoft.Json;
	using Console = System.Console;
	class Program {
		static void Main(string[] args) {
			//var usages = new AzureUsageClient(new MetricsConfigEndpointConfiguration(Environment.CurrentDirectory))
			//	.GetWebsitesUsage()
			//	.Result;
			//new MetricsConfigEndpointConfiguration(Environment.CurrentDirectory)
			var cloudservices = new AzureUsageClient(null).GetCloudServiceUsages().Result;
			Console.WriteLine(JsonConvert.SerializeObject(cloudservices,Formatting.Indented));
		}
	}
}
