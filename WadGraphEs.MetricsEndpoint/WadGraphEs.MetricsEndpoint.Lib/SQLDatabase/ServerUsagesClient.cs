using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib.SQLDatabase {
	public interface ServerUsagesClient {
		ICollection<UsageObject> GetUsages(DateTime from);
	}
}
