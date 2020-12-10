using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests.ListAsync
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WhenCancellationRequested : MountedFileStoreTestBase
    {
        private List<string> _paths;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            await foreach (var path in ClassInTest.ListAsync(new RecurseAllFilter(), new CancellationToken(true)))
            {
                _paths.Add(path);
            }
        }
        
        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Is.Empty);
        }

        private class RecurseAllFilter : IPathFilter
        {
            public PathAction DecideAction(string path)
            {
                return PathAction.Collect;
            }
        }
    }
}
