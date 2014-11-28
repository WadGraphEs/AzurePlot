using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class AzureWebsiteId {
		readonly string _websiteName;

		public string Name {
			get { return _websiteName; }
		} 

		readonly string _webspace;

		public AzureWebsiteId(string websiteName,string webspace) {
			_websiteName = websiteName;
			_webspace = webspace;
		}

		public string ResourceId { 
			get {
				return ResourceIdBuilder.BuildWebSiteResourceId(_webspace,_websiteName);
			}
		}

		internal GraphiteCounterName BuildGraphiteCounterName(WebsiteMetricsDataSource websiteMetricsDataSource,string metricName,string metricUnit) {
								//Azure.WebSites.<websitename>.<slot>.MetricsApi.<metricname>.<unit>.Average
			return new GraphiteCounterName("Azure","WebSites", _websiteName,"Production",GetSourceName(websiteMetricsDataSource),
						metricName,metricUnit);
						
		}

		
		internal GraphiteCounterName BuildGraphiteCounterNamePercentage(WebsiteMetricsDataSource websiteMetricsDataSource,string metricName) {
			return BuildGraphiteCounterName(websiteMetricsDataSource,metricName,"Percentage");
		}

		private string GetSourceName(WebsiteMetricsDataSource websiteMetricsDataSource) {
			switch(websiteMetricsDataSource) {
				case WebsiteMetricsDataSource.MetricsApi:
					return "MetricsApi";
				case WebsiteMetricsDataSource.WebsitesApi:
					return "WebsitesApi";
				default:
					return "UnknownSource";
			}
		}

	}
}
