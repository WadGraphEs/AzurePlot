using Microsoft.WindowsAzure.Management.Monitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	class CloudServiceInstanceId {
		readonly string _instanceName;
		readonly string _rolename;
		readonly string _deploymentSlot;
		readonly string _deploymentName;
		readonly string _serviceName;

		public CloudServiceInstanceId(string serviceName, string deploymentName, string deploymentSlot, string rolename, string instanceName) {
			_serviceName=serviceName;
			_deploymentName=deploymentName;
			_deploymentSlot=deploymentSlot;
			_rolename=  rolename;
			_instanceName = instanceName;
		}
		//public string ServiceName{get;set;}
		//public string DeploymentName{get;set;}
		//public string DeploymentSlot{get;set;}

		//public string RoleName { get; set; }

		//public string InstanceName { get; set; }

		public string ResourceId { 
			get {
				return ResourceIdBuilder.BuildCloudServiceResourceId(_serviceName,_deploymentName,_rolename,_instanceName);
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
