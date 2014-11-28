using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class WebsiteApiMetricsData {
		public double CurrentValue { get; set; }
		public string Name { get; set; }
		public string ComputeMode { get; set; }
		public string DisplayName { get; set; }
		public double Limit { get; set; }
		public string Unit { get; set; }

		internal ICollection<UsageObject> ToUsageObjects(AzureWebsiteId forWebsiteId) {
			return FromWebsiteUsageObjectEnumerable(forWebsiteId).ToList();
		}

		//Azure.WebSites.<websitename>.<slot>.WebsiteApi.<metricname>.<unit>
		//Azure.WebSites.<websitename>.<slot>.WebsiteApi.<metricname>.Percentage

		private IEnumerable<UsageObject> FromWebsiteUsageObjectEnumerable(AzureWebsiteId website) {
			yield return new UsageObject() {
				GraphiteCounterName = website.BuildGraphiteCounterName(WebsiteMetricsDataSource.WebsitesApi,Name,Unit).ToString(),
				Timestamp = DateTime.UtcNow.ToString("o"),
				Value = CurrentValue
			};

			if(Limit <= 0) {
				yield break;
			}

			yield return new UsageObject() {
				GraphiteCounterName = website.BuildGraphiteCounterNamePercentage(WebsiteMetricsDataSource.WebsitesApi,Name).ToString(),
				Timestamp = DateTime.UtcNow.ToString("o"),
				Value = CurrentValue * 100 / Limit
			};
		}
	}
}
