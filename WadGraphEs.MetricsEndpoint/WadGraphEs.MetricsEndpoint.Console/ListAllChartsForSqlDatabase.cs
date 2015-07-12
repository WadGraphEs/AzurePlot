using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;

namespace WadGraphEs.MetricsEndpoint.Console {
    using Console = System.Console;
    class ListAllChartsForSqlDatabase {
        private List<string> _args;
        
        public ListAllChartsForSqlDatabase(List<string> args) {
            _args = args;

        }

        internal void Print() {
            var charts = ChartsFacade.ListAllChartsForSqlDatabaseServer(_args[0],_args[1],_args[2]);
            
            foreach(var chart in charts) {
                Console.WriteLine("{0} {1}", chart.Name, chart.Uri);
            }
        }
    }
}
