using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Console {
	using Newtonsoft.Json;
	using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;
	using Console = System.Console;
	class Program {
		static void Main(string[] args) {
			//var usages = new AzureUsageClient(new MetricsConfigEndpointConfiguration(Environment.CurrentDirectory))
			//	.GetWebsitesUsage()
			//	.Result;
			//new MetricsConfigEndpointConfiguration(Environment.CurrentDirectory)
			//var cloudservices = new AzureUsageClient(null).GetCloudServiceUsages().Result;
			//Console.WriteLine(JsonConvert.SerializeObject(cloudservices,Formatting.Indented));

	//		CREATE LOGIN WAD_MonitorUser
	//WITH PASSWORD=N'passwd'

			var username = args[0];
			var password = args[1];
			var servername = args[2];
			var databasename = "master";

			Console.WriteLine("Connecting to {0} with username {1} on database {2}", servername,username,databasename);

			var azureSqlClient = SQLDatabaseUsageClient.CreateServerUsagesClient(servername,username,password);

		}
	}
}
