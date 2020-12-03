using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Services
{
    public interface ITransactionService
    {
        Task<GetTransactionsResponseV1> GetTransactionsAsync(GetTransactionsRequestV1 request, CancellationToken cancellationToken);
        Task<GetDetailResponseV1> GetDetailAsync(string fileDirectory, CancellationToken cancellationToken);
    }
}