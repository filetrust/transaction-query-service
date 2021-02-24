using System.Collections.Generic;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics
{
    public class TransactionAnalytics
    {

        public long TotalProcessed { get; set; }

        public IEnumerable<AnalyticalHour> Data { get; set; }

    }
}