using System;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Store.DatePathFilterTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<DatePathFilter>
    {
        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            Assert.That(() => new DatePathFilter(
                    DateTimeOffset.MinValue,
                    DateTimeOffset.MaxValue
                ), Throws.Nothing);
        }

        [Test]
        public void Constructor_Throws_With_Null_Start()
        {
            Assert.That(() => new DatePathFilter(
                    null,
                    DateTimeOffset.MaxValue), ThrowsArgumentNullException("start"));
        }

        [Test]
        public void Constructor_Throws_With_Null_End()
        {
            Assert.That(() => new DatePathFilter(
                    DateTimeOffset.MinValue,
                    null
                ), ThrowsArgumentNullException("end"));
        }
    }
}
