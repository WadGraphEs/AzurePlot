using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.Lib.SQLDatabase;

namespace WadGraphEs.MetricsEndpoint.Console {
    class ListAllChartsForSqlDatabase {
        private List<string> _args;
        
        public ListAllChartsForSqlDatabase(List<string> args) {
            _args = args;

        }

        internal void Print() {
            var charts = ChartsFacade.ListAllChartsForSqlDatabaseServer(_args[0],_args[1],_args[2]);


        }
    }
}
