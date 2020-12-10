using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ExistsAsync
{
    [TestFixture]
    public class WithExistingFile : MountedFileStoreTestBase
    {
        private bool _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            var fullPath = $"{RootPath}/{Guid.NewGuid()}.txt";

            if (!File.Exists(fullPath))
            {
                await File.WriteAllTextAsync(fullPath, "some text", CancellationToken);
            }

            _output = await ClassInTest.ExistsAsync(fullPath, CancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void True_Is_Returned()
        {
            Assert.That(_output);
        }
    }
}
