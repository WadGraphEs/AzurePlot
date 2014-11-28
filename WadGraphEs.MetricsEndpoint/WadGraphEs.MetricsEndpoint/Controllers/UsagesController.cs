using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Xml;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Controllers {
	public class UsagesController:ApiController {
		public async Task<IEnumerable<UsageObject>> Get() {
			var azureUsageService = new AzureUsageClient(Factories.MetricsConfigEndpointConfigurationFactory.New());

			var websiteUsage = azureUsageService.GetWebsitesUsage();
			var cloudServiceUsage = azureUsageService.GetCloudServiceUsages();

			return (await websiteUsage).Concat(await cloudServiceUsage);
		}

	}
}