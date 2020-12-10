using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<MountedFileStore>
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            Assert.That(() => new MountedFileStore(null, ""), ThrowsArgumentNullException("logger"));
            Assert.That(() => new MountedFileStore(Mock.Of<ILogger<MountedFileStore>>(), null),
                ThrowsArgumentNullException("rootPath"));
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            Assert.That(() => new MountedFileStore(Mock.Of<ILogger<MountedFileStore>>(), ""), Throws.Nothing);
        }
    }
}
