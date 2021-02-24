using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Enums
{
    public static class RiskAssessment
    {
        public static Risk DetermineRisk(
            NcfsOutcome ncfsOutcome, 
            GwOutcome rebuildOutcome, 
            GwOutcome unmanagedFileTypeOutcome,
            GwOutcome blockedOutcome)
        {
            if (TryGetRiskFromRebuildOutcome(rebuildOutcome, out var risk))
            {
                if (rebuildOutcome == GwOutcome.Failed)
                {
                    if (TryGetRiskFromNcfsOutcome(ncfsOutcome, out risk))
                        return risk;

                    if (TryGetRiskFromActionOutcome(unmanagedFileTypeOutcome, out risk))
                        return risk;

                    if (TryGetRiskFromActionOutcome(blockedOutcome, out risk))
                        return risk;

                    return Risk.Unknown;
                }

                return risk;
            }

            if (TryGetRiskFromNcfsOutcome(ncfsOutcome, out risk))
                return risk;

            if (TryGetRiskFromActionOutcome(unmanagedFileTypeOutcome, out risk))
                return risk;

            return Risk.Unknown;
        }

        private static bool TryGetRiskFromRebuildOutcome(GwOutcome outcome, out Risk risk)
        {
            risk = outcome switch
            {
                GwOutcome.Replace => Risk.Safe,
                GwOutcome.Unmodified => Risk.Safe,
                GwOutcome.Failed => Risk.BlockedByPolicy,
                _ => Risk.Unknown
            };
            return risk != Risk.Unknown;
        }

        private static bool TryGetRiskFromActionOutcome(GwOutcome outcome, out Risk risk)
        {
            risk = outcome switch
            {
                GwOutcome.Unmodified => Risk.AllowedByPolicy,
                GwOutcome.Failed => Risk.BlockedByPolicy,
                _ => Risk.Unknown
            };
            return risk != Risk.Unknown;
        }

        private static bool TryGetRiskFromNcfsOutcome(NcfsOutcome outcome, out Risk risk)
        {
            risk = outcome switch
            {
                NcfsOutcome.Replace => Risk.Safe,
                NcfsOutcome.Block => Risk.BlockedByNCFS,
                NcfsOutcome.Relay => Risk.AllowedByNCFS,
                _ => Risk.Unknown
            };
            return risk != Risk.Unknown;
        }
    }
}