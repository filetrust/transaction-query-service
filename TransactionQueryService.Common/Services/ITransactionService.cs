using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Services
{
    public interface ITransactionService
    {
        Task<Transactions> GetTransactionsAsync(GetTransactionsRequestV1 request, CancellationToken cancellationToken);
        Task<TransactionDetail> GetDetailAsync(string fileDirectory, CancellationToken cancellationToken);
        Task<TransactionAnalytics> GetTransactionAnalyticsAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken1);
    }
}