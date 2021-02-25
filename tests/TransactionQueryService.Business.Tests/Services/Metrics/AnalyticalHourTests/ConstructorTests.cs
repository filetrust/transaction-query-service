using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Services.Metrics.AnalyticalHourTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<AnalyticalHour>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<AnalyticalHour>();
        }
    }
}
