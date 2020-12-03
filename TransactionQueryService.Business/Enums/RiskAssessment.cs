using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Enums
{
    public static class RiskAssessment
    {
        public static Risk DetermineRisk(NcfsOutcome ncfsOutcome, GwOutcome gwOutcome)
        {
            Risk risk;
            switch (ncfsOutcome)
            {
                case NcfsOutcome.Relayed:
                    risk = Risk.AllowedByNCFS;
                    break;
                case NcfsOutcome.Replaced:
                    risk = Risk.Safe;
                    break;
                case NcfsOutcome.Blocked:
                    risk = Risk.BlockedByNCFS;
                    break;
                default:
                    risk = Risk.Unknown;
                    break;
            }

            if (risk != Risk.Unknown) return risk;

            switch (gwOutcome)
            {
                case GwOutcome.Replace:
                    risk = Risk.Safe;
                    break;
                case GwOutcome.Unmodified:
                    risk = Risk.AllowedByPolicy;
                    break;
                case GwOutcome.Failed:
                    risk = Risk.BlockedByPolicy;
                    break;
                default:
                    risk = Risk.Unknown;
                    break;
            }

            return risk;
        }
    }
}