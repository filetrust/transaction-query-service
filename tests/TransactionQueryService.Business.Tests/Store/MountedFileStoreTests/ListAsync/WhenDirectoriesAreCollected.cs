using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ListAsync
{
    [TestFixture]
    public class WhenDirectoriesAreCollected : MountedFileStoreTestBase
    {
        private List<string> _paths;
        private string _fullPath1;
        private string _fullPath2;
        private string _subPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            Directory.CreateDirectory(RootPath);
            _subPath = $"{RootPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
            Directory.CreateDirectory(_subPath);
            _fullPath1 = $"{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            _fullPath2 = $"{_subPath}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.txt";
            await File.WriteAllTextAsync(_fullPath1, "some text", CancellationToken);
            await File.WriteAllTextAsync(_fullPath2, "some text", CancellationToken);

            await foreach (var val in ClassInTest.ListAsync(new RecurseAllFilter(), CancellationToken))
                _paths.Add(val);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Has.One.EqualTo(_subPath));
        }

        public class RecurseAllFilter : IPathFilter
        {
            public PathAction DecideAction(string path)
            {
                return PathAction.Collect;
            }
        }
    }
}
