using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzurePlot.Lib.SQLDatabase {
	public interface ServerUsagesClient {
		ICollection<UsageObject> GetUsages(DateTime from);


        List<string> ListDatabases();
    }
}
