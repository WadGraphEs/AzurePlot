using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
    class AzureCloudService {
        public string ServiceResourceId { get; set; }

        public string ServiceName {get;set;}

        public string SubscriptionId{get;set;}

        public List<CloudServiceInstanceId> Instances { get; set; }

        public string FriendlyName {
            get {
                return ServiceName;
            }
        }
    }

    struct AMDCloudServiceRoleId {
        public string CloudServiceName{get;set;}
        public string Slot{get;set;}
        public string Role{get;set;}

        public string SubscriptionId{get;set;}

        Uri Uri {
            get {
                return new Uri(string.Format("wadgraphes://{0}/cloud-services/{1}/{2}/{3}", SubscriptionId, CloudServiceName, Slot,Role));
            }
        }

        public string DisplayName { 
            get {
                return string.Format("{0}.{1} ({2})", CloudServiceName,Role,Slot);
            }
        }

        internal static AMDCloudServiceRoleId FromUri(Uri uri) {
            var pathSegments = uri.LocalPath.Split(new [] { '/' },StringSplitOptions.RemoveEmptyEntries);
            return new AMDCloudServiceRoleId { 
                CloudServiceName = pathSegments[1],
                Slot = pathSegments[2],
                Role = pathSegments[3],
                SubscriptionId = uri.Host
            };
        }

        internal Uri AppendToUri(string path) {
            return new Uri(Uri.ToString()+path);
        }
    }
}
