using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Services
{
    public interface ITransactionService
    {
        Task<Transactions> GetTransactionsAsync(GetTransactionsRequestV1 request, CancellationToken cancellationToken);
        Task<TransactionDetail> GetDetailAsync(string fileDirectory, CancellationToken cancellationToken);
        Task<TransactionAnalytics> GetTransactionAnalyticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken1);
    }

    public class TransactionAnalytics
    {
        public long TotalProcessed { get; set; }

        public IEnumerable<AnalyticalHour> Data { get; set; }

    }

    public class AnalyticalHour
    {
        public long Processed { get; set; }

        // public long Pending { get; set; }

        public long SentToNcfs { get; set; }

        public DateTimeOffset Date { get; set; }

        public Dictionary<string, long> ProcessedByOutcome { get; set; }

        public Dictionary<string, long> ProcessedByNcfs { get; set; }
    }
}