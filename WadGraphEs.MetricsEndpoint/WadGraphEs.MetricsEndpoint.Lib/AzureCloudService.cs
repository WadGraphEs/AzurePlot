using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
    class AzureCloudService {
        public string ServiceId { get; set; }

        public List<CloudServiceInstanceId> Instances { get; set; }
    }
}
