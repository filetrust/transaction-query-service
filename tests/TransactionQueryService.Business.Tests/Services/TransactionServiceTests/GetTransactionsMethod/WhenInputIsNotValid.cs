using System.Threading;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetTransactionsMethod
{
    [TestFixture]
    public class WhenInputIsNotValid : TransactionServiceTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            base.SharedSetup();
        }

        [Test]
        public void Throws_With_Invalid_Path()
        {
            Assert.That(() => ClassInTest.GetTransactionsAsync(null, CancellationToken.None), ThrowsArgumentNullException("request"));
        }
    }
}
