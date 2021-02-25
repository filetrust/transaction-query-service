using System;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.Metrics.AnalyticalHourTests.Initial
{
    [TestFixture]
    public class WhenSettingInitialState : AnalyticalHourTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        [TestCase("2020-02-01T10:00:00")]
        [TestCase("2021-02-01T10:00:00")]
        [TestCase("2020-01-01T10:00:00")]
        [TestCase("2020-02-01T11:00:00")]
        [TestCase("2020-02-01T10:01:00")]
        [TestCase("2020-02-01T10:00:01")]
        public void Hour_TimeStamp_Is_Set(string timestampAsString)
        {
            var timestamp = DateTimeOffset.Parse(timestampAsString);

            var initial = AnalyticalHour.Initial(timestamp, ExampleGwOutcome, ExampleNcfsOutcome, ExampleUnmanagedFileTypeActionFlag,
                ExampleUnmanagedBlockActionFlag);

            Assert.That(initial.Date, Is.EqualTo(timestamp));
        }

        [Test]
        [TestCase(ExampleGwOutcome, 1)]
        [TestCase("Some random stringy", 1)]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        public void Processed_By_Outcome_Is_Incremented_With_GwOutcome_Set(string gwOutcome, int expectedCount)
        {
            var initial = AnalyticalHour.Initial(DateTimeOffset.Now, gwOutcome, ExampleNcfsOutcome, null, null);

            Assert.That(initial.Processed, Is.EqualTo(1));
            Assert.That(initial.ProcessedByOutcome, Has.Exactly(expectedCount).Items);
            if (expectedCount > 0) Assert.That(initial.ProcessedByOutcome.ContainsKey(gwOutcome));
        }

        [Test]
        [TestCase(ExampleUnmanagedFileTypeActionFlag, 1)]
        [TestCase("Some random stringy", 1)]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        public void Processed_By_Outcome_Is_Incremented_With_UnmanagedFileTypeActionFlag_Set(string unmanagedFileTypeActionFlag, int expectedCount)
        {
            var initial = AnalyticalHour.Initial(DateTimeOffset.Now, null, ExampleNcfsOutcome, unmanagedFileTypeActionFlag, null);

            Assert.That(initial.Processed, Is.EqualTo(1));
            Assert.That(initial.ProcessedByOutcome, Has.Exactly(expectedCount).Items);
            if (expectedCount > 0) Assert.That(initial.ProcessedByOutcome.ContainsKey(unmanagedFileTypeActionFlag));
        }

        [Test]
        [TestCase(ExampleUnmanagedBlockActionFlag, 1)]
        [TestCase("Some random stringy", 1)]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        public void Processed_By_Outcome_Is_Incremented_With_BlockedActionFlag_Set(string blocked, int expectedCount)
        {
            var initial = AnalyticalHour.Initial(DateTimeOffset.Now, null, ExampleNcfsOutcome, null, blocked);

            Assert.That(initial.Processed, Is.EqualTo(1));
            Assert.That(initial.ProcessedByOutcome, Has.Exactly(expectedCount).Items);
            if (expectedCount > 0) Assert.That(initial.ProcessedByOutcome.ContainsKey(blocked));
        }

        [Test]
        [TestCase(ExampleNcfsOutcome, 1)]
        [TestCase("Some random stringy", 1)]
        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        public void Processed_By_Ncfs_Is_Incremented_With_NCFSOutcome_Set(string ncfs, int expectedCount)
        {
            var initial = AnalyticalHour.Initial(DateTimeOffset.Now, null, ncfs, null, null);

            Assert.That(initial.Processed, Is.EqualTo(1));
            Assert.That(initial.SentToNcfs, Is.EqualTo(expectedCount));
            Assert.That(initial.ProcessedByNcfs, Has.Exactly(expectedCount).Items);
            if (expectedCount > 0) Assert.That(initial.ProcessedByNcfs.ContainsKey(ncfs));
        }
    }
}
