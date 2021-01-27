using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetHourTimestampOfFiles
{
    [TestFixture]
    public class WhenPathIsInvalid : TransactionServiceTestBase
    {
        private IAsyncEnumerable<string> _paths1;
        private IAsyncEnumerable<string> _paths2;
        private IAsyncEnumerable<DateTimeOffset> _output;

        [OneTimeSetUp]
        public void Setup()
        {
            base.SharedSetup();
        }

        [Test]
        [TestCase("/rgolskg/12/1/21/")]
        [TestCase("/2020/3osjt/1/21/")]
        [TestCase("/2020/12/serlgmjslkm/21/")]
        [TestCase("/2020/12/1/epsgk/")]
        public async Task Each_Store_Is_Searched(string path)
        {
            Share1.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetSomePaths(1, path));

            Share2.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetSomePaths(2, path));

            _output = ClassInTest.GetHourTimestampsOfFiles(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, CancellationToken.None);

            var count = 0;

            await foreach (var _ in _output)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(0));
        }

        private static async IAsyncEnumerable<string> GetSomePaths(int store, string path)
        {
            for (var index = 0; index < 5; index++)
            {
                yield return $"{MountingInfo.MountLocation}{store}{path}{Guid.NewGuid()}";
            }

            await Task.CompletedTask;
        }
    }
}