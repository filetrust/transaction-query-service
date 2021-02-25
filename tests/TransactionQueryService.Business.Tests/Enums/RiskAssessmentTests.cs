using System;
using System.Collections.Generic;
using Glasswall.Administration.K8.TransactionQueryService.Business.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Enums
{
    [TestFixture]
    // Uncovered edge cases
    // - cases when NCFS is not set and policy is refer                                              
    // - cases when NCFS is not set and policy is not to refer
    public class RiskAssessmentTests
    {
        private const NcfsOutcome NcfsNotSet = NcfsOutcome.Unknown;
        private const GwOutcome OutcomeNotSet = GwOutcome.Unknown;

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsUnknown))]
        public void When_FileType_Is_Unknown_And_Ncfs_Is_Not_Set(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected) =>
            Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsUnknownAndNcfsIsSet))]
        public void When_FileType_Is_Unknown_And_Ncfs_Is_Set(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected) =>
            Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildReplacesContent))]
        public void When_FileType_Is_Known_And_Rebuild_Replaces_Content(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected) 
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildDoesNotModifyContent))]
        public void When_FileType_Is_Known_And_Rebuild_Does_Not_Modify_Content(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsRelay))]
        public void When_FileType_Is_Known_And_Rebuild_Fails_And_NcfsOutcome_Is_Relay(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsReplace))]
        public void When_FileType_Is_Known_And_Rebuild_Fails_And_Ncfs_Outcome_Is_Relay(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsBlock))]
        public void When_FileType_Is_Known_And_Rebuild_Fails_And_Ncfs_Outcome_Is_Block(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsNotSet))]
        public void When_FileType_Is_Known_And_Rebuild_Fails_And_NcfsOutcome_Is_Not_Set(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        [Test]
        [TestCaseSource(nameof(WhenFileTypeIsKnownAndRebuildFailsButNoNcfsActionIsFound))]
        public void When_FileType_Is_Known_And_Rebuild_Fails_But_No_NcfsAction_Is_Found(NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
            => Correct_Risk_Is_Identified(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction, expected);

        private void Correct_Risk_Is_Identified(
            NcfsOutcome ncfsOutcome, GwOutcome rebuildOutcome, GwOutcome unmanagedAction, GwOutcome blockedFileAction, Risk expected)
        {
            var actual = RiskAssessment.DetermineRisk(ncfsOutcome, rebuildOutcome, unmanagedAction, blockedFileAction);
            Assert.That(actual, Is.EqualTo(expected));
        }

        private static object[] WhenFileTypeIsUnknown()
        {
            return new object[]
            {
                new object[] {NcfsNotSet, OutcomeNotSet, GwOutcome.Failed, OutcomeNotSet, Risk.BlockedByPolicy},
                new object[] {NcfsNotSet, OutcomeNotSet, GwOutcome.Replace, OutcomeNotSet, Risk.Unknown},
                new object[] {NcfsNotSet, OutcomeNotSet, GwOutcome.Unmodified, GwOutcome.Replace, Risk.AllowedByPolicy}
            };
        }

        private static object[] WhenFileTypeIsUnknownAndNcfsIsSet()
        {
            return new object[]
            {
                new object[] {NcfsOutcome.Block, OutcomeNotSet, GwOutcome.Failed, OutcomeNotSet, Risk.BlockedByNCFS},
                new object[] {NcfsOutcome.Replace, OutcomeNotSet, GwOutcome.Replace, OutcomeNotSet, Risk.Safe},
                new object[] {NcfsOutcome.Relay, OutcomeNotSet, GwOutcome.Unmodified, GwOutcome.Replace, Risk.AllowedByNCFS}
            };
        }

        private static object[] WhenFileTypeIsKnownAndRebuildReplacesContent()
        {
            var objs = new List<object>();

            foreach (var ncfsOutcome in Enum.GetValues(typeof(NcfsOutcome)))
                foreach (var unmanagedFileTypeAction in Enum.GetValues(typeof(GwOutcome)))
                    foreach (var blockedFileAction in Enum.GetValues(typeof(GwOutcome)))
                        objs.Add(new[] { ncfsOutcome, GwOutcome.Replace, unmanagedFileTypeAction, blockedFileAction, Risk.Safe });

            return objs.ToArray();
        }

        private static object[] WhenFileTypeIsKnownAndRebuildDoesNotModifyContent()
        {
            var objs = new List<object>();

            foreach (var ncfsOutcome in Enum.GetValues(typeof(NcfsOutcome)))
                foreach (var unmanagedFileTypeAction in Enum.GetValues(typeof(GwOutcome)))
                    foreach (var blockedFileAction in Enum.GetValues(typeof(GwOutcome)))
                        objs.Add(new[] { ncfsOutcome, GwOutcome.Unmodified, unmanagedFileTypeAction, blockedFileAction, Risk.Safe });

            return objs.ToArray();
        }

        private static object[] WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsNotSet()
        {
            return new object[]
            {
                new object[] {NcfsNotSet, GwOutcome.Failed, OutcomeNotSet, GwOutcome.Unmodified, Risk.AllowedByPolicy},
                new object[] {NcfsNotSet, GwOutcome.Failed, OutcomeNotSet, GwOutcome.Failed, Risk.BlockedByPolicy}
            };
        }

        private static object[] WhenFileTypeIsKnownAndRebuildFailsButNoNcfsActionIsFound()
        {
            return new object[]
            {
                new object[] {NcfsOutcome.Unknown, GwOutcome.Failed, OutcomeNotSet, OutcomeNotSet, Risk.Unknown}
            };
        }

        private static object[] WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsRelay()
        {
            var objs = new List<object>();

            foreach (var unmanagedFileTypeAction in Enum.GetValues(typeof(GwOutcome)))
                foreach (var blockedFileAction in Enum.GetValues(typeof(GwOutcome)))
                    objs.Add(new[] { NcfsOutcome.Relay, GwOutcome.Failed, unmanagedFileTypeAction, blockedFileAction, Risk.AllowedByNCFS });

            return objs.ToArray();
        }

        private static object[] WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsBlock()
        {
            var objs = new List<object>();

            foreach (var unmanagedFileTypeAction in Enum.GetValues(typeof(GwOutcome)))
                foreach (var blockedFileAction in Enum.GetValues(typeof(GwOutcome)))
                    objs.Add(new[] { NcfsOutcome.Block, GwOutcome.Failed, unmanagedFileTypeAction, blockedFileAction, Risk.BlockedByNCFS });

            return objs.ToArray();
        }

        private static object[] WhenFileTypeIsKnownAndRebuildFailsAndNcfsOutcomeIsReplace()
        {
            var objs = new List<object>();

            foreach (var unmanagedFileTypeAction in Enum.GetValues(typeof(GwOutcome)))
                foreach (var blockedFileAction in Enum.GetValues(typeof(GwOutcome)))
                    objs.Add(new[] { NcfsOutcome.Replace, GwOutcome.Failed, unmanagedFileTypeAction, blockedFileAction, Risk.Safe });

            return objs.ToArray();
        }
    }
}
