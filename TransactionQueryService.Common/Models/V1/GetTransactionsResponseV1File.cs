using System;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class GetTransactionsResponseV1File
    {
        public DateTimeOffset Timestamp { get; set; }
        public Guid FileId { get; set; }
        public FileType DetectionFileType { get; set; }
        public Risk Risk { get; set; }
        public Guid ActivePolicyId { get; set; }
        public string Directory { get; set; }
    }
}