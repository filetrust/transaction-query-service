using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.DownloadAsync
{
    [TestFixture]
    public class WhenFileDoesNotExist : MountedFileStoreTestBase
    {
        private MemoryStream _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            var fullPath = $"{RootPath}/{Guid.NewGuid()}.txt";

            _output = await ClassInTest.DownloadAsync(fullPath, CancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void Output_Is_File_Contents()
        {
            Assert.That(_output, Is.Null);
        }
    }
}
