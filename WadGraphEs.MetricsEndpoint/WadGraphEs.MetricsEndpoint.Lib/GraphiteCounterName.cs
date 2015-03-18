using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WadGraphEs.MetricsEndpoint.Lib {
	public class GraphiteCounterName {
		readonly string _root;
		readonly string[] _parts;

		public GraphiteCounterName(string root,params string[] parts) {
			_root = root;
			_parts = parts;
		}

		public override string ToString() {
			var sb = new StringBuilder(_root);
			foreach(var part in _parts) {
				sb.AppendFormat(".{0}", NormalizePart(part));
			}

			return sb.ToString();
		}

		private static string NormalizePart(string p) {
			return p
				.Trim()
				.Trim('\\')
				.Replace('\\','.')
				.Replace(' ','_')
				.Replace("%", "Pct")
				.Replace(".", "_dot_")
				.Replace("/", "_")
				;
		}
	}
}
