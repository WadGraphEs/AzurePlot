using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace AzurePlot.Lib {
    using System.Reflection;
    using Console = System.Console;
    public class ServicePointMonitor {
        private Action<string> _output;

        public ServicePointMonitor(Action<string> output) {
            _output = output;
        }
        
        private void Print() {
            foreach(var serviceEndpoint in ListServicePoints()) {
                PrintServicePointConnections(serviceEndpoint);
            }
        }

        private IEnumerable<ServicePoint> ListServicePoints(){
            var tableField = typeof(ServicePointManager).GetField("s_ServicePointTable", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var table = (Hashtable)tableField.GetValue(null);
            var keys = table.Keys.Cast<object>().ToList();

            Output("ServicePoint count: {0}, DefaultConnectionLimit: {1}", keys.Count, ServicePointManager.DefaultConnectionLimit);
            foreach(var key in keys) {
                var val = ((WeakReference)table[key]);

                if(val == null) {
                    continue;
                }
                var target = val.Target;
                if(target==null) { 
                    continue;
                }
                yield return target as ServicePoint;
            }
            
        }

        private void Output(string fmt,params object[] args)
        {
            _output(string.Format(fmt,args));
        }

        private void PrintServicePointConnections(ServicePoint sp) {
            var spType = sp.GetType();
            var privateOrPublicInstanceField = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var connectionGroupField = spType.GetField("m_ConnectionGroupList",privateOrPublicInstanceField);
            var value = (Hashtable)connectionGroupField.GetValue(sp);
            var connectionGroups = value.Keys.Cast<object>().ToList();
            var totalConnections = 0;
            Output("ServicePoint: {0} (Connection Limit: {1}, Reported connections: {2})", sp.Address,sp.ConnectionLimit,sp.CurrentConnections);
            foreach(var key in connectionGroups) {
                var connectionGroup = value[key];
                var groupType = connectionGroup.GetType();
                var listField = groupType.GetField("m_ConnectionList",privateOrPublicInstanceField);
                var listValue = (ArrayList)listField.GetValue(connectionGroup);
                //Console.WriteLine("{3} {0}\nConnectionGroup: {1} Count: {2}",sp.Address, key,listValue.Count, DateTime.Now);
                Output("{0}", key);
                totalConnections+=listValue.Count;
            }
            
            Output("ConnectionGroupCount: {0}, Total Connections: {1}", connectionGroups.Count, totalConnections);
        }

        public static void Start(TimeSpan interval) {
            Start(interval,Console.WriteLine);
        }

        public static void Start(TimeSpan interval, Action<string> output) {
            var thread = new Thread(()=> {
                while(true) {
                    var monitor = new ServicePointMonitor(output);
                    monitor.Print();
                    

                    Thread.Sleep(interval);
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }
    }
}
