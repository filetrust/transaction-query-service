using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ListAsync
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WhenActionIsStop : MountedFileStoreTestBase
    {
        private List<string> _paths;
        private string _subPath;
        private string _fullPath1;
        private string _fullPath2;

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

            await foreach (var path in ClassInTest.ListAsync(new StopAllFilter(), new CancellationToken(false)))
            {
                _paths.Add(path);
            }
        }
        
        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Is.Empty);
        }

        private class StopAllFilter : IPathFilter
        {
            public PathAction DecideAction(string path)
            {
                return PathAction.Stop;
            }
        }
    }
}
