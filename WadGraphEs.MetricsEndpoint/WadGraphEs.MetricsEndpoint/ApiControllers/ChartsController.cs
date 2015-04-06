using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class ChartsController : ApiController{
		[ActionName("list-all-charts")]
		[HttpGet]
		public async Task<ICollection<ChartInfo>> ListAllCharts(){
			return await AzureSubscriptions.ListAllCharts();
			//return new [] {
			//	new ChartInfo { 
			//		Id="subscription.{123123}.cloudservices.{123}.cpu",
			//		Name = "TransactionEngine CPU",
			//		Service = "Windows Azure MSDN - Visual Studio Ultimate",
			//		ServiceType ="Azure Subcription"
			//	},
			//	new ChartInfo { 
			//		Id="sql-database-server.{n4wyssobxk}.{wad_test}.dtus",
			//		Name = "wad_test DTUs",
			//		Service = "n4wyssobxk.database.windows.net",
			//		ServiceType ="Azure SQL Database"
			//	}
			//};
		}
	}
}