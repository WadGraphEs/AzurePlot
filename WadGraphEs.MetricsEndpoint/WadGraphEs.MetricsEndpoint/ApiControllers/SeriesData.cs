using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.ApiControllers {
	public class SeriesData {
		public string Name{get;set;}
		public List<DataPoint> DataPoints{get;set;}
	}
}
