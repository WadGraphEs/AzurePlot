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
}
