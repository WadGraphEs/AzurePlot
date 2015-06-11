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

        public string CpuUri {
            get {
                return string.Format("wadgraphes://{0}/cloud-services/{1}/{2}/{3}/cpu", SubscriptionId, CloudServiceName, Slot,Role);
            }
        }

        public string DisplayName { 
            get {
                return string.Format("{0}.{1} ({2})", CloudServiceName,Role,Slot);
            }
        }
    }
}
