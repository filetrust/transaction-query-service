using System.Collections.Generic;
using System.Linq;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class Transactions
    {
        public int Count => Files.Count();

        public IEnumerable<TransactionFile> Files { get; set; }
    }
}