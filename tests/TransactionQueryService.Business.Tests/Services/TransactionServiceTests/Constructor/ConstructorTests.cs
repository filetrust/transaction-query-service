using Glasswall.Administration.K8.TransactionQueryService.Business.Services;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.Constructor
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<TransactionService>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<TransactionService>();
        }
    }
}
