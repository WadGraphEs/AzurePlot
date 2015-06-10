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
			

	//		CREATE LOGIN WAD_MonitorUser
	//WITH PASSWORD=N'passwd'
            if(!args.Any()) {
                Console.WriteLine("Must provide args");
                Environment.Exit(1);
            }

            switch(args[0]) {
                case "gen-certificate":
                    GenerateCertificate();
                    return;
                case "get-website-stats":
                    //get-website-stats <endpointconfig> <website-name> 24:00:00 ^Http
                    new GetWebsiteStats(args.Skip(1).ToList()).PrintStats();
                    return;
                case "display-chart-data":
                    //display-chart-data uri 24:00:00 [optional-arguments]
                    new DisplayChartData(args.Skip(1).ToList()).PrintData();
                    return;
                case "list-all-charts":
                    new ListAllCharts(args.Skip(1).ToList()).Print();
                    return;
                default:
                    Console.WriteLine("Unknown operation {0}", args[0]);
                    Environment.Exit(1);
                    return;
            }
		}


        private static void GenerateCertificate() {
            var certName = ReadStringOrDefault("Output certificate?", "certificate.cer");
            var pfxName = ReadStringOrDefault("Output PFX?", "private.pfx");
            var cn = ReadStringOrDefault("Certificate Common Name?", "Azure Management");

            string password = ReadPassword();

            var pfx = X509Tools.GenerateCertificate.GeneratePfx(cn,password);
            var cer = X509Tools.GenerateCertificate.GetCertificateForBytes(pfx,password);

            Console.WriteLine("Writing pfx to {0}", pfxName);
            File.WriteAllBytes(pfxName,pfx);
            Console.WriteLine("Writing cert to {0}", certName);
            File.WriteAllBytes(certName,cer);
        }

        private static string ReadPassword() {
            Console.WriteLine("Password?");
            string password;

            while(true) {
                password = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(password)) {
                    Console.WriteLine("Invalid password");
                    continue;
                }
                break;
            }
            return password;
        }

        private static string ReadStringOrDefault(string label, string @default) {
            Console.WriteLine("{0}? ({1})?", label, @default);
            var read = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(read)) {
                return @default;
            }

            return read;
        }

        private static void GetSQL() {    
             //var username = args[0];
            //var password = args[1];
            //var servername = args[2];
            //var databasename = "master";

            //Console.WriteLine("Connecting to {0} with username {1} on database {2}", servername,username,databasename);

            //var serverUsagesClient = SQLDatabaseUsageClient.CreateServerUsagesClient(servername,username,password);

            //var result = serverUsagesClient.TestConnection();

            //if(result.Failed) {
            //    Console.WriteLine("Error\n{0}\n{1}", result.Message,result.Exception);
            //}

            //var usages = serverUsagesClient.GetUsages(DateTime.Today.ToUniversalTime());

            //foreach(var usage in usages) {
            //    Console.WriteLine("{0}\t{1}\t{2}", usage.GraphiteCounterName,usage.Timestamp,usage.Value);
            //}
        }
	}
}

    