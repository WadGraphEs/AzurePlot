using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.ApiControllers {
	public class ChartData {
		public string Name{get;set;}
		public List<SeriesData> Series{get;set;}
        public string IntervalFrom { get; set; }
        public string IntervalTill { get; set; }
        public TimeSpan Interval {
            set {
                var now = DateTime.UtcNow;
                IntervalFrom = now.Add(value.Negate()).ToString("o");
                IntervalTill = now.ToString("o");
            }
        }
    }
}
