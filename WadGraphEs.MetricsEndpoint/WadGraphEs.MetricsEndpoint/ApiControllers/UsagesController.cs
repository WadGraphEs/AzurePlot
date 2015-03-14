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
using NLog;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class UsagesController:ApiController {
		readonly static Logger _logger = LogManager.GetCurrentClassLogger(); 
		public async Task<IEnumerable<UsageObject>> Get() {
			_logger.Info("Getting usages");

			try {
				var azureUsageService = new AzureUsageClient(Factories.MetricsConfigEndpointConfigurationFactory.New());

				var websiteUsage = azureUsageService.GetWebsitesUsage();
				var cloudServiceUsage = azureUsageService.GetCloudServiceUsages();

				return (await websiteUsage).Concat(await cloudServiceUsage);
			}
			catch(Exception e) {
				_logger.ErrorException("Error getting usages", e);
				throw;
			}
		}

	}
}