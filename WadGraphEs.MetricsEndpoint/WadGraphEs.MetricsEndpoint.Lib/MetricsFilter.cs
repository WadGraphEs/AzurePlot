using Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WadGraphEs.MetricsEndpoint.Lib {
    class MetricsFilter {
        public static readonly MetricsFilter None = new MetricsFilter(m=>true);

        readonly Func<MetricDefinition,bool> _filter;

        MetricsFilter(Func<MetricDefinition,bool> filter) {
            _filter = filter;
        }

        internal ICollection<MetricDefinition> FilterMetrics(List<Microsoft.WindowsAzure.Management.Monitoring.Metrics.Models.MetricDefinition> metrics) {
            return metrics.Where(_filter).ToList();
        }

        internal static MetricsFilter FromRegexes(string[] filters) {
            if(!filters.Any()) {
                return MetricsFilter.None;
            }

            var regexes = filters.Select(_=>new Regex(_,RegexOptions.IgnoreCase)).ToList();

            return new MetricsFilter(m=>regexes.Any(_=>_.IsMatch(m.Name)));// .Where(_=>filtersRegex.Any(f=>f.IsMatch(_.GraphiteCounterName))).ToList()

        }
    }
}
