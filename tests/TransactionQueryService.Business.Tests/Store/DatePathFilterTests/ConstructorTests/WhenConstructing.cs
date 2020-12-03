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
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<DatePathFilter>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            Assert.That(() => new DatePathFilter(
                new FileStoreFilterV1
                {
                    TimestampRangeStart = DateTimeOffset.MinValue,
                    TimestampRangeEnd = DateTimeOffset.MaxValue
                }), Throws.Nothing);
        }

        [Test]
        public void Constructor_Throws_With_Null_Start()
        {
            Assert.That(() => new DatePathFilter(
                new FileStoreFilterV1
                {
                    TimestampRangeStart = null,
                    TimestampRangeEnd = DateTimeOffset.MaxValue
                }), ThrowsArgumentException("filter", "Start was null"));
        }

        [Test]
        public void Constructor_Throws_With_Null_End()
        {
            Assert.That(() => new DatePathFilter(
                new FileStoreFilterV1
                {
                    TimestampRangeStart = DateTimeOffset.MinValue,
                    TimestampRangeEnd = null
                }), ThrowsArgumentException("filter", "End was null"));
        }
    }
}
