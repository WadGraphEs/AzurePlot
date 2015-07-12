using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.Lib {
	class CloudServiceInstanceId {
        readonly string _instanceName;

        public string InstanceName {
            get { return _instanceName; }
        } 

		readonly string _rolename;
		readonly string _deploymentSlot;
		readonly string _deploymentName;
        readonly string _serviceName;
        readonly string _subscriptionId;

        public string ServiceName {
            get { return _serviceName; }
        }

        public AMDCloudServiceRoleId RoleId {
            get {
                return new AMDCloudServiceRoleId {  
                     CloudServiceName = _serviceName,
                     Role = _rolename,
                     Slot = _deploymentSlot,
                     SubscriptionId = _subscriptionId
                };
            }
        }


		public CloudServiceInstanceId(string subscriptionId, string serviceName, string deploymentName, string deploymentSlot, string rolename, string instanceName) {
			_serviceName=serviceName;
			_deploymentName=deploymentName;
			_deploymentSlot=deploymentSlot;
			_rolename=  rolename;
			_instanceName = instanceName;
            _subscriptionId = subscriptionId;
		}
	
		public string ResourceId { 
			get {
				return ResourceIdBuilder.BuildCloudServiceResourceId(_serviceName,_deploymentName,_rolename,_instanceName);
			}
		}

        public string CloudServiceId {
            get {
                return ResourceIdBuilder.BuildCloudServiceResourceId(_serviceName,_deploymentName,_rolename);
            }
        }

		//Azure.CloudServices.MetricsApi.<servicename>.<deploymentslot>.<rolename>.<metricname>.<unit>.<typeofvalue>.<instancename>


		internal GraphiteCounterName BuildGraphiteCounterName(string metricname,string unit,string typeOfValueName) {
			return new GraphiteCounterName(
				"Azure","CloudServices","MetricsApi",
				_serviceName,
				_deploymentSlot,
				_rolename,
				metricname,
				unit,
				typeOfValueName,
				_instanceName
			);
		}





        
    }
}
