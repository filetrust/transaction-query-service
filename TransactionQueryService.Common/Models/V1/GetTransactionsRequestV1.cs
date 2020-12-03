using System.ComponentModel.DataAnnotations;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class GetTransactionsRequestV1
    {
        [Required]
        public FileStoreFilterV1 Filter { get; set; }
    }
}