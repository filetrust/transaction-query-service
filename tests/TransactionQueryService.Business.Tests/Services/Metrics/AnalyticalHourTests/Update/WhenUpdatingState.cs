using System;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.Metrics.AnalyticalHourTests.Update
{
    [TestFixture]
    public class WhenUpdatingState : AnalyticalHourTestBase
    {
        [SetUp]
        public void Setup()
        {
            SharedSetup();

            ClassInTest = AnalyticalHour.Initial(DateTimeOffset.Now, ExampleGwOutcome, ExampleNcfsOutcome, null, null);
        }

        [Test]
        [TestCase(ExampleGwOutcome, 2)]
        [TestCase("Some random stringy", 2)]
        [TestCase(null, 1)]
        [TestCase("", 1)]
        [TestCase(" ", 1)]
        public void Processed_By_Outcome_Is_Incremented_With_GwOutcome_Set(string gwOutcome, int expectedCount)
        {
            ClassInTest.Update(gwOutcome, null, null, null);

            Assert.That(ClassInTest.Processed, Is.EqualTo(2));
            Assert.That(ClassInTest.ProcessedByOutcome.Sum(f => f.Value), Is.EqualTo(expectedCount));
            if (expectedCount > 1) Assert.That(ClassInTest.ProcessedByOutcome.ContainsKey(gwOutcome));
        }

        [Test]
        [TestCase(ExampleUnmanagedFileTypeActionFlag, 2)]
        [TestCase("Some random stringy", 2)]
        [TestCase(null, 1)]
        [TestCase("", 1)]
        [TestCase(" ", 1)]
        public void Processed_By_Outcome_Is_Incremented_With_UnmanagedFileTypeActionFlag_Set(string unmanagedFileTypeActionFlag, int expectedCount)
        {
            ClassInTest.Update(null, null, unmanagedFileTypeActionFlag, null);

            Assert.That(ClassInTest.Processed, Is.EqualTo(2));
            Assert.That(ClassInTest.ProcessedByOutcome.Sum(f => f.Value), Is.EqualTo(expectedCount));
            if (expectedCount > 1) Assert.That(ClassInTest.ProcessedByOutcome.ContainsKey(unmanagedFileTypeActionFlag));
        }

        [Test]
        [TestCase(ExampleUnmanagedBlockActionFlag, 2)]
        [TestCase("Some random stringy", 2)]
        [TestCase(null, 1)]
        [TestCase("", 1)]
        [TestCase(" ", 1)]
        public void Processed_By_Outcome_Is_Incremented_With_BlockedActionFlag_Set(string blocked, int expectedCount)
        {
            ClassInTest.Update(null, null, null, blocked);

            Assert.That(ClassInTest.Processed, Is.EqualTo(2));
            Assert.That(ClassInTest.ProcessedByOutcome.Sum(f => f.Value), Is.EqualTo(expectedCount));
            if (expectedCount > 1) Assert.That(ClassInTest.ProcessedByOutcome.ContainsKey(blocked));
        }

        [Test]
        [TestCase(ExampleNcfsOutcome, 2)]
        [TestCase("Some random stringy", 2)]
        [TestCase(null, 1)]
        [TestCase("", 1)]
        [TestCase(" ", 1)]
        public void Processed_By_Ncfs_Is_Incremented_With_NCFSOutcome_Set(string ncfs, int expectedCount)
        {
            ClassInTest.Update(null, ncfs, null, null);

            Assert.That(ClassInTest.Processed, Is.EqualTo(2));
            Assert.That(ClassInTest.SentToNcfs, Is.EqualTo(expectedCount));
            if (expectedCount > 1) Assert.That(ClassInTest.ProcessedByNcfs.ContainsKey(ncfs));
        }
    }
}
