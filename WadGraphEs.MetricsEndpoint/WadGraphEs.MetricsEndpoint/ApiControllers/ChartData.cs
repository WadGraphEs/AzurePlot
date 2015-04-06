using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class ChartData {
		public string Name{get;set;}
		public List<SeriesData> Series{get;set;}
	}
}
