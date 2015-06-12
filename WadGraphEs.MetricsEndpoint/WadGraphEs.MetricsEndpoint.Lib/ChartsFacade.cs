using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WadGraphEs.MetricsEndpoint.Lib {
    public class ChartsFacade {
        public static async Task<ICollection<ChartInfo>> ListAllChartsForSubscription(MetricsEndpointConfiguration subscriptionConfig, string serviceName) {
            var infoClient = new AzureSubscriptionInfoClient(subscriptionConfig);

			var websites = infoClient.ListWebsites();
            var cloudservices = infoClient.ListCloudServiceRoles();

			return 
                (await websites).SelectMany(_=>GetWebsiteCharts(serviceName,_))
                .Concat(
                    (await cloudservices ).SelectMany(_=>GetCloudServiceCharts(serviceName,_)))
                .ToList();
        }

        private static ICollection<ChartInfo> GetCloudServiceCharts(string serviceName, AMDCloudServiceRoleId cs) {
            Func<string,Uri,ChartInfo> buildChartInfo = (counter,uri)=>
                new ChartInfo {
                    ResourceName = cs.DisplayName,
                    ResourceType = "cloud service",
                    ServiceName = serviceName,
                    ServiceType = "Azure Subscription",
                    Name = string.Format("{0} (cloud service) {1}", cs.DisplayName,counter),
                    Uri = uri.ToString()
                };

            return new ChartInfo[] {
                buildChartInfo("CPU", cs.AppendToUri("/cpu")),
                buildChartInfo("Network", cs.AppendToUri("/network")),
                buildChartInfo("Disk Performance", cs.AppendToUri("/disk"))
            };
        }

        private static ICollection<ChartInfo> GetWebsiteCharts(string serviceName, AzureWebsite website) {
			Func<string,string,ChartInfo> initChartInfo = (name,uri)=>
				new ChartInfo {
					ResourceName = website.Name,
					ResourceType = "website",
					ServiceName = serviceName,
					ServiceType = "Azure Subscription",
					Name = string.Format("{0} (website) {1}", website.Name,  name),
					Uri = uri
				};
			return new ChartInfo[] {
				initChartInfo("Requests", website.Uri.ToString()+"/requests"),
				initChartInfo("Memory", website.Uri.ToString()+"/memory"),
				initChartInfo("CPU", website.Uri.ToString()+"/cpu"),
				initChartInfo("Traffic", website.Uri.ToString()+"/traffic"),
                initChartInfo("Response Times", website.Uri.ToString()+"/response-times"),
			};
		}
    }
}
