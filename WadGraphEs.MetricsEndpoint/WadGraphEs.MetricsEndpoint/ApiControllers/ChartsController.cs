using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using WadGraphEs.MetricsEndpoint.Setup;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class ChartsController : ApiController{
		[ActionName("list-all-charts")]
		[HttpGet]
		public async Task<ICollection<ChartInfo>> ListAllCharts(){
			var subscriptionCharts = AzureSubscriptions.ListAllCharts();
            var sqlCharts = AzureSQLDatabases.ListAllCharts();

             

            return (await subscriptionCharts).Concat(sqlCharts)
                 .Concat(new [] { new ChartInfo {
                    Name = "Dummy",
                    ResourceName = "DummyResourceName",
                    ResourceType ="DummyResourceType",
                    ServiceName = "Dummy Service",
                    ServiceType = "DummyService",
                    Uri = "wadgraphes://dummy"
                }})            
                .ToList();
		}

		[ActionName("get-chart-data")]
		[HttpGet]
		public Task<ChartData> GetChartData(string uri){
            //return AzureSubscriptions.GetChartData(uri);
            var facade = new ChartDataFacade(uri);

            facade.SqlCredentialsProvider = AzureSQLDatabases.GetCredentials;
            facade.SubscriptionCredentialsProvider = AzureSubscriptions.GetCredentials;

            return facade.FetchChartData();
			
		}

        

	}
}