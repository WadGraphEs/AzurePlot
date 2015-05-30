using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.DataAccess;

namespace WadGraphEs.MetricsEndpoint.Setup {
	public class Dashboard {
		internal static void AddChart(string uri) {
			var ctx = GetDataContext();	

			ctx.DashboardCharts.Add(new DashboardChart {
				Uri = uri
			});

			ctx.SaveChanges();
		}

		private static DataContext GetDataContext() {
			return new DataContext();
		}

		internal static ICollection<string> GetCharts() {
			return GetDataContext().DashboardCharts.Select(_=>_.Uri).ToList();
		}
	}
}