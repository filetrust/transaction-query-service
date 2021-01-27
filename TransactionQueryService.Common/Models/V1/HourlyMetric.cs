using System;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class HourlyMetric
    {
        public DateTimeOffset Date { get; set; }

        public int Processed { get; set; }
    }
}