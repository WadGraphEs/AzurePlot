using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AzurePlot.Web.DataAccess;

namespace AzurePlot.Web.Setup {
	public class Dashboard {
		internal static DashboardChart AddChart(string uri) {
			return DataContext.Do(ctx=>{
                var chart = new DashboardChart {
				    Uri = uri
			    };
			    ctx.DashboardCharts.Add(chart);

			    ctx.SaveChanges();

                return chart;
            });
		}


		internal static ICollection<DashboardChart> GetDashboardCharts() {
			return DataContext.Do(ctx=>ctx.DashboardCharts.OrderBy(_=>_.Id).ToList());
		}

        internal static void RemoveChart(int chartId) {
            DataContext.Do(ctx=>{
                var chart = ctx.DashboardCharts.Find(chartId);
                ctx.DashboardCharts.Remove(chart);
                ctx.SaveChanges();
            });
        }

        internal static DashboardChart GetChartById(int id) {
            return DataContext.Do(ctx=>ctx.DashboardCharts.Find(id));
        }
    }
}