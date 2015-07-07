using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace WadGraphEs.MetricsEndpoint.Console {
    using System.Reflection;
    using Console = System.Console;
    class ServicePointMonitor {
        
        internal static void Start(TimeSpan interval) {
            var thread = new Thread(()=> {
                while(true) {
                    foreach(var serviceEndpoint in ListServicePoints()) {
                        PrintServicePointConnections(serviceEndpoint);
                    }

                    Thread.Sleep(interval);
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        private static IEnumerable<ServicePoint> ListServicePoints() {
            var tableField = typeof(ServicePointManager).GetField("s_ServicePointTable", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var table = (Hashtable)tableField.GetValue(null);
            var keys = table.Keys.Cast<object>().ToList();

            Console.WriteLine("ServicePoint count: {0}", keys.Count);
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

        private static void PrintServicePointConnections(ServicePoint sp) {
            var spType = sp.GetType();
            var privateOrPublicInstanceField = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var connectionGroupField = spType.GetField("m_ConnectionGroupList",privateOrPublicInstanceField);
            var value = (Hashtable)connectionGroupField.GetValue(sp);
            var connectionGroups = value.Keys.Cast<object>().ToList();
            var totalConnections = 0;
            Console.WriteLine("ServicePoint: {0}", sp.Address);
            foreach(var key in connectionGroups) {
                var connectionGroup = value[key];
                var groupType = connectionGroup.GetType();
                var listField = groupType.GetField("m_ConnectionList",privateOrPublicInstanceField);
                var listValue = (ArrayList)listField.GetValue(connectionGroup);
                //Console.WriteLine("{3} {0}\nConnectionGroup: {1} Count: {2}",sp.Address, key,listValue.Count, DateTime.Now);
                Console.WriteLine("{0}", key);
                totalConnections+=listValue.Count;
            }
            
            Console.WriteLine("ConnectionGroupCount: {0}, Total Connections: {1}", connectionGroups.Count, totalConnections);
        }
    }
}
