using Glasswall.Administration.K8.TransactionQueryService.Business.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Enums
{
    [TestFixture]
    public class RiskAssessmentTests
    {
        [Test]
        [TestCase(NcfsOutcome.Relayed, GwOutcome.Unknown, Risk.AllowedByNCFS)]
        [TestCase(NcfsOutcome.Relayed, GwOutcome.Replace, Risk.AllowedByNCFS)]
        [TestCase(NcfsOutcome.Relayed, GwOutcome.Unmodified, Risk.AllowedByNCFS)]
        [TestCase(NcfsOutcome.Relayed, GwOutcome.Failed, Risk.AllowedByNCFS)]
        [TestCase(NcfsOutcome.Replaced, GwOutcome.Unknown, Risk.Safe)]
        [TestCase(NcfsOutcome.Replaced, GwOutcome.Replace, Risk.Safe)]
        [TestCase(NcfsOutcome.Replaced, GwOutcome.Unmodified, Risk.Safe)]
        [TestCase(NcfsOutcome.Replaced, GwOutcome.Failed, Risk.Safe)]
        [TestCase(NcfsOutcome.Blocked, GwOutcome.Unknown, Risk.BlockedByNCFS)]
        [TestCase(NcfsOutcome.Blocked, GwOutcome.Replace, Risk.BlockedByNCFS)]
        [TestCase(NcfsOutcome.Blocked, GwOutcome.Unmodified, Risk.BlockedByNCFS)]
        [TestCase(NcfsOutcome.Blocked, GwOutcome.Failed, Risk.BlockedByNCFS)]
        [TestCase(NcfsOutcome.Unknown, GwOutcome.Unknown, Risk.Unknown)]
        [TestCase(NcfsOutcome.Unknown, GwOutcome.Replace, Risk.Safe)]
        [TestCase(NcfsOutcome.Unknown, GwOutcome.Unmodified, Risk.AllowedByPolicy)]
        [TestCase(NcfsOutcome.Unknown, GwOutcome.Failed, Risk.BlockedByPolicy)]
        public void Correct_Risk_Is_Identified(NcfsOutcome ncfsOutcome, GwOutcome gwOutcome, Risk expected)
        {
            var actual = RiskAssessment.DetermineRisk(ncfsOutcome, gwOutcome);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
