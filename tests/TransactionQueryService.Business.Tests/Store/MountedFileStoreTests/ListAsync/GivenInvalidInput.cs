using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ListAsync
{
    [TestFixture]
    public class GivenInvalidInput : MountedFileStoreTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.ListAsync(null, CancellationToken),
                ThrowsArgumentNullException("pathFilter"));
        }
    }
}
