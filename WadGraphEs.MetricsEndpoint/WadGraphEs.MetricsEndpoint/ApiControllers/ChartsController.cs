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
		public ChartData GetChartData(string uri){
			var data = new ChartData { 
				Name = "azurewebsites requests",
				Series = new List<SeriesData> {
					new SeriesData {
						Name = "200",
						DataPoints = GenerateData(TimeSpan.FromHours(12),40)
					},
					new SeriesData {
						Name = "all",
						DataPoints = GenerateData(TimeSpan.FromHours(12),50)
					},
				}
			};

			return data;
		}

		private List<DataPoint> GenerateData(TimeSpan period,double magnitude) {
			var res = new List<DataPoint>();
			var end = DateTime.Now;
			var start = end.Add(period.Negate());

			var rand = new Random();
			
			for(var i = start; i<=end; i = i.AddMinutes(5)) {
				res.Add(new DataPoint {
					Timestamp = i.ToUniversalTime().ToString("o"),
					Value = magnitude * (1 + rand.NextDouble() + Math.Sin(2*Math.PI * (i-start).TotalMinutes / period.TotalMinutes))
				});
			}

			return res;
		}
	}
}