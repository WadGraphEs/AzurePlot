using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AzurePlot.Lib {
	class AzureCloudServiceInfoApiClient {
		readonly AzureManagementRestClient _client;

		public AzureCloudServiceInfoApiClient(AzureManagementRestClient client) {
			_client = client;
		}

		const string HostedServicesPath = "/services/hostedservices";

		//from: http://msdn.microsoft.com/en-us/library/azure/ee460781.aspx
		internal async System.Threading.Tasks.Task<ICollection<string>> ListCloudServices() {
			//todo: handle continuation token
			var xml = await GETXml(HostedServicesPath);
			
			return SelectElementsInXmlByXPath(xml,"/a:HostedServices/a:HostedService/a:ServiceName").Select(_=>_.Value).ToList();
			
		}
		

		public async System.Threading.Tasks.Task<ICollection<CloudServiceInstanceId>> ListInstances() {
			var services = await ListCloudServices();
			

			var deployments = await Task.WhenAll(services.Select(_=>GetInstancesForService(_)));

			return deployments.SelectMany(_=>_).ToList();
			//var deployments = services.Select(async _=>await ))
		}

		private async Task<ICollection<CloudServiceInstanceId>> GetInstancesForService(string service) {
			var serviceXml = await GETXml(string.Format("/services/hostedservices/{0}?embed-detail=true",service));
			var deployments = SelectElementsInXmlByXPath(serviceXml,"/a:HostedService/a:Deployments/a:Deployment");

			var res = new List<CloudServiceInstanceId>();

			foreach(var deployment in deployments) {
				foreach(var instance in deployment.Element(GetElementName("RoleInstanceList")).Elements(GetElementName("RoleInstance"))) {
					res.Add(new CloudServiceInstanceId(
                        _client.SubscriptionId,
						service,
						deployment.Element(GetElementName("Name")).Value,
						deployment.Element(GetElementName("DeploymentSlot")).Value,
						instance.Element(GetElementName("RoleName")).Value,
						instance.Element(GetElementName("InstanceName")).Value
					));
				}
			}

			return res;
		}

		const string ApiVersion = "2014-06-01";

		private async System.Threading.Tasks.Task<string> GETXml(string path) {
			return await _client.GETXml(path ,apiVersion :ApiVersion);
		}

		private string GETXmlSync(string path) {
			return _client.GetXmlSync(path,apiVersion:ApiVersion);
		}

		private static ICollection<XElement> SelectElementsInXmlByXPath(string xml,string selector) {
			var res = XDocument.Parse(xml);

			var nsManager = new XmlNamespaceManager(res.CreateReader().NameTable);

			nsManager.AddNamespace("a",XMLElementNamespace);

			return res.XPathSelectElements(selector,nsManager).ToList();
		}

		private const string XMLElementNamespace= "http://schemas.microsoft.com/windowsazure";
		
		private static XName GetElementName(string name) {
			return XName.Get(name,XMLElementNamespace);
		}

		internal void TestConnection() {
			var xml = GETXmlSync(SubscriptionPath);
		}

		const string SubscriptionPath = "";


		internal string GetSubscriptionNameSync() {
			var xml = GETXmlSync(SubscriptionPath);

			return SelectElementsInXmlByXPath(xml,"/a:Subscription/a:SubscriptionName").Select(_=>_.Value).FirstOrDefault()??"Unknown";
		}

        internal Task<ICollection<CloudServiceInstanceId>> ListInstancesForServiceName(string serviceName) {
            
            return GetInstancesForService(serviceName);
        }
    }
}
