using System.Collections.Generic;
using System.Linq;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class GetTransactionsResponseV1
    {
        public int Count => Files.Count();

        public IEnumerable<GetTransactionsResponseV1File> Files { get; set; }
    }
}