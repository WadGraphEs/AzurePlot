using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class ChartsController : ApiController{
		[ActionName("list-all-charts")]
		[HttpGet]
		public Task<ICollection<ChartInfo>> ListAllCharts(){
			return AzureSubscriptions.ListAllCharts();
		}

		[ActionName("get-chart-data")]
		[HttpGet]
		public Task<ChartData> GetChartData(string uri){
            return AzureSubscriptions.GetChartData(uri);
			
		}

	}
}