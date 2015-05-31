using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class Dashboard {
		internal static DashboardChart AddChart(string uri) {
			var ctx = GetDataContext();	


            var chart = new DashboardChart {
				Uri = uri
			};
			ctx.DashboardCharts.Add(chart);

			ctx.SaveChanges();

            return chart;
		}

		private static DataContext GetDataContext() {
			return new DataContext();
		}

		internal static ICollection<DashboardChart> GetDashboardCharts() {
			return GetDataContext().DashboardCharts.OrderBy(_=>_.Id).ToList();
		}

        internal static void RemoveChart(int chartId) {
            var ctx = GetDataContext();
            var chart = ctx.DashboardCharts.Find(chartId);
            ctx.DashboardCharts.Remove(chart);
            ctx.SaveChanges();
        }

        internal static DashboardChart GetChartById(int id) {
            return GetDataContext().DashboardCharts.Find(id);
        }
    }
}