using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.DownloadAsync
{
    [TestFixture]
    public class GivenValidFile : MountedFileStoreTestBase
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

            if (!File.Exists(fullPath))
            {
                await File.WriteAllTextAsync(fullPath, "some text", CancellationToken);
            }

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
            using (_output)
            {
                var str = Encoding.UTF8.GetString(_output.ToArray());

                Assert.That(str, Is.EqualTo("some text"));
            }
        }
    }
}
