using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WadGraphEs.MetricsEndpoint.ApiControllers;

namespace WadGraphEs.MetricsEndpoint.Lib {
    public class ChartDataFacade {
        public ChartDataFacade(string forUri) {
            _uri = new Uri(forUri);
            SubscriptionCredentialsProvider = s=>{ throw new InvalidOperationException("set SubscriptionCredentialsProvider"); };
        }

        public Func<string,MetricsEndpointConfiguration> SubscriptionCredentialsProvider;
        private Uri _uri;

        public Task<ChartData> FetchChartData() {
            var interval = GetInterval(_uri);

            if(_uri.Host == "dummy") {
                return Dummy(interval);
            }

			var path = _uri.LocalPath.Split(new [] {'/'},StringSplitOptions.RemoveEmptyEntries);
			
            switch(path[0]) {
                case "websites":
                    return GetWebsiteChartData(interval,path);
                case "cloud-services":
                    return GetCloudServiceChartData(interval,path);
                default:
                    throw new Exception("don't know how to handle " + path[0]);
            }
        }

        private Task<ChartData> GetCloudServiceChartData(TimeSpan interval,string[] path) {
            var serviceId = AMDCloudServiceRoleId.FromUri(_uri);

            var counter = path[path.Length-1];

            switch(counter) {
                case "cpu":
                    return GetCloudServiceCPU(serviceId, interval);
                case "disk":
                    return GetCloudServiceDisk(serviceId,interval);
                default:
                    throw new ArgumentException("don't now how to get " + counter);
            }

        }

        private Task<ChartData> GetCloudServiceDisk(AMDCloudServiceRoleId serviceRoleId,TimeSpan history) {
            return GetCloudServiceMetrics(serviceRoleId,history, 
                label: "Disk performance",
                regex:"Disk",
                formatSeriesLabel:(instanceName,metricName)=>string.Format("{0} {1}", instanceName, metricName)
            );
        }

        private string GetSubscriptionId() {
            return GetCredentials().SubscriptionId;
        }

        private Task<ChartData> GetCloudServiceCPU(AMDCloudServiceRoleId serviceRoleId, TimeSpan history) {
            return GetCloudServiceMetrics(serviceRoleId,history, label: "CPU",regex:"CPU",formatSeriesLabel:(instanceName,metricname)=>instanceName);
        }

        private async Task<ChartData> GetCloudServiceMetrics(AMDCloudServiceRoleId serviceRoleId, TimeSpan history, string label,string regex,Func<string,string,string> formatSeriesLabel) {
            var client = new AzureCloudServicesClient(new AzureManagementRestClient(GetCredentials().GetCertificateCloudCredentials()),GetCredentials().GetCertificateCloudCredentials());

            var instances = await client.ListInstancesForServiceRole(serviceRoleId);

            var usages = await client.GetUsage(instances,history,MetricsFilter.FromRegexes(regex));

            var usagesPartitioned = PartitionByInstanceNameAndMetric(usages);

            return new ChartData {
                Name = string.Format("{0} {1}", serviceRoleId.DisplayName,label),
                Series = usagesPartitioned.Keys.SelectMany(
                    instance=>usagesPartitioned[instance].Keys.Select(metricName=>new SeriesData {
                        Name = formatSeriesLabel(instance,metricName),
                        DataPoints = usagesPartitioned[instance][metricName].Select(uo=>new DataPoint { Timestamp = uo.Timestamp, Value = uo.Value }).ToList()
                    })).ToList()
            };
                
                
                //usages.Select(_=> new SeriesData{
                //    Name = formatInstanceName(_.Key.InstanceName),
                //    DataPoints = _.Value.Select(uo=>new DataPoint { Timestamp = uo.Timestamp, Value = uo.Value }).ToList()
                //}).ToList()
            
        }

        private Dictionary<string,Dictionary<string,List<UsageObject>>> PartitionByInstanceNameAndMetric(ICollection<UsageObject> usages) {
            //fmt: Azure.CloudServices.MetricsApi.<servicename>.<slot>.<role>.<metricname>.<unit>.<aggregation>.<instancename>
            const int instanceNameIndex = 9;
            const int metricNameIndex = 6;
            return usages.GroupBy(_=>_.GraphiteCounterName.Split('.')[instanceNameIndex])
                .ToDictionary(
                    _=>_.Key,
                    _=>_.GroupBy(x=>x.GraphiteCounterName.Split('.')[metricNameIndex]).ToDictionary(y=>y.Key,y=>y.ToList()));
        }

        private Task<ChartData> GetWebsiteChartData(TimeSpan interval,string[] path) {
            var webspace = path[1];
            var websiteName = path[2];
            var counter = path[3];
            switch(counter) {
                case "requests":
                    return GetWebsiteRequests(webspace,websiteName,interval);
                case "cpu":
                    return GetWebsiteCPU(webspace,websiteName,interval);
                case "memory":
                    return GetWebsiteMemory(webspace,websiteName,interval);
                case "traffic":
                    return GetWebsiteTraffic(webspace,websiteName,interval);
                case "response-times":
                    return GetWebsiteResponseTimes(webspace,websiteName,interval);
                default:
                    throw new Exception("Don't know how to get " + counter);
            }
        }


        private Task<ChartData> GetWebsiteCPU(string webspace,string websiteName, TimeSpan interval) {
			return GetWebsiteUsages(webspace,websiteName,x=>x,string.Format("{0} (website) CPU",websiteName), interval,"^CpuTime");
		}

		private Task<ChartData> GetWebsiteRequests(string webspace,string websiteName, TimeSpan interval) {
			return GetWebsiteUsages(webspace,websiteName,x=>x.Replace(".Count",""),string.Format("{0} (website) requests", websiteName),interval,"^Http", "^Requests");
		}

        
        private Task<ChartData> GetWebsiteMemory(string webspace,string websiteName, TimeSpan interval) {
            return GetWebsiteUsages(webspace,websiteName,x=>x.Replace(".Bytes",""),string.Format("{0} (website) memory usage (bytes)", websiteName),interval,"MemoryWorkingSet");
        }

        private Task<ChartData> GetWebsiteTraffic(string webspace,string websiteName, TimeSpan interval) {
            return GetWebsiteUsages(webspace,websiteName,x=>x.Replace(".Bytes",""),string.Format("{0} (website) traffic (bytes)", websiteName),interval,"(^BytesSent|^BytesReceived)");
        }

        private Task<ChartData> GetWebsiteResponseTimes(string webspace,string websiteName, TimeSpan interval) {
            return GetWebsiteUsages(webspace,websiteName,x=>x.Replace(".Milliseconds",""),string.Format("{0} (website) response times (ms)", websiteName),interval,"^AverageResponseTime");
        }

		private async Task<ChartData> GetWebsiteUsages(string webspace,string websiteName,Func<string,string> formatSeries,string charttitle, TimeSpan interval,params string[] filters) {
			var usageClient = new AzureUsageClient(GetCredentials());
			var usages = await usageClient.GetWebsitesUsageForWebsite(webspace,websiteName,interval,filters);
			return new ChartData {
				Name = charttitle,
				Series = usages.GroupBy(_ => _.GraphiteCounterName).Select(_ =>
					new SeriesData {
						Name = formatSeries(_.Key),
						DataPoints = _.Select(dp => new DataPoint { Timestamp = dp.Timestamp,Value = dp.Value }).ToList()
					}
				).ToList()
			};
		}

        private MetricsEndpointConfiguration GetCredentials() {
            return SubscriptionCredentialsProvider(_uri.Host);
        }

         private static TimeSpan GetInterval(Uri uri) {

             var qs = ParseQueryString(uri);

            var unit = GetUnit(qs, TimeSpan.FromHours(1));
            return GetInterval(qs, unit, 1);
        }

         private static NameValueCollection ParseQueryString(Uri uri) {
             //todo: add decent implementation without relying on system.web
             if(string.IsNullOrEmpty(uri.Query)) {
                return new NameValueCollection();
             }

             var query = uri.Query;

             query = query.Substring(1);

             var spl = query.Split('&');

             var pairs  = spl.Select(_ => _.Split('=')).ToDictionary(p => p[0],p => p[1]);

             var qs = new NameValueCollection();
             foreach(var pair in pairs) {
                 qs.Add(pair.Key,pair.Value);
             }
             return qs;
         }

        private static TimeSpan GetInterval(System.Collections.Specialized.NameValueCollection qs,TimeSpan unit,int @defaultValue) {
            var value = @defaultValue;
            if(!string.IsNullOrEmpty(qs["interval"])) {
                value = int.Parse(qs["interval"]);
            }
            if(value<=0) {
                throw new ArgumentException("Cannot have negative interval");
            }
            return TimeSpan.FromSeconds(unit.TotalSeconds * value);            
        }

        private static TimeSpan GetUnit(System.Collections.Specialized.NameValueCollection qs,TimeSpan @default) {
            if(string.IsNullOrEmpty(qs["unit"])) {
                return @default;
            }
            switch(qs["unit"]) {
                case "minutes": return TimeSpan.FromMinutes(1);
                case "hours": return TimeSpan.FromHours(1);
            }
            throw new ArgumentOutOfRangeException("Not a valid unit");
        }

        

        private static Task<ChartData> Dummy(TimeSpan interval) {
            var data = new ChartData { 
                Name = "Dummy",
                Series = new List<SeriesData> {
                    new SeriesData {
                        Name = "200",
                        DataPoints = GenerateData(interval,40)
                    },
                    new SeriesData {
                        Name = "all",
                        DataPoints = GenerateData(interval,50)
                    },
                }
            };

            return Task.FromResult(data);
        }
        
      

		private static List<DataPoint> GenerateData(TimeSpan period,double magnitude) {
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
                     //var usages = GetUsageClient()
            //    .GetWebsitesUsageForWebsite(GetWebspace(), _website,_history, _filters.ToArray())
            //    .Result;
        //private string GetWebspace() {
        //    return AzureWebsitesInfoApiClientFacade.FindWebspace(_config,_website);
        //}

        //private AzureUsageClient GetUsageClient() {
        //    return new AzureUsageClient(_config);
        //}
    }
}
