using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.AnalysisReport;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1
{
    public class TransactionDetail
    {
        public DetailStatus Status { get; set; }
        public GWallInfo AnalysisReport { get; set; }
    }
}