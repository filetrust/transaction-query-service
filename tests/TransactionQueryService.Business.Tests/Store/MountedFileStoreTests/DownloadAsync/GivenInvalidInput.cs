using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.DownloadAsync
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Exception_Is_Thrown(string input)
        {
            Assert.That(async () => await ClassInTest.DownloadAsync(input, CancellationToken),
                ThrowsArgumentException("path", "Value must not be null or whitespace"));
        }
    }
}
