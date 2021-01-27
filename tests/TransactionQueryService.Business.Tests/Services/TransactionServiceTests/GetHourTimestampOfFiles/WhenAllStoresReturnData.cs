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
    public class WhenAllStoresReturnData : TransactionServiceTestBase
    {
        private IAsyncEnumerable<string> _paths1;
        private IAsyncEnumerable<string> _paths2;
        private IAsyncEnumerable<DateTimeOffset> _output;

        [OneTimeSetUp]
        public void Setup()
        {
            base.SharedSetup();
            
            _paths1 = GetSomePaths(1);
            _paths2 = GetSomePaths(2);

            Share1.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths1);

            Share2.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths2);

            _output = ClassInTest.GetHourTimestampsOfFiles(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, CancellationToken.None);
        }

        [Test]
        public void Each_Store_Is_Searched()
        {
            Share1.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
            Share2.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Test]
        public async Task Correct_Response_Is_Returned()
        {
            var count = 0;

            await foreach (var _ in _output)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(10));
        }

        private static async IAsyncEnumerable<string> GetSomePaths(int store)
        {
            for (var index = 0; index < 5; index++)
            {
                yield return $"{MountingInfo.MountLocation}{store}/2020/12/1/21/{Guid.NewGuid()}";
            }

            await Task.CompletedTask;
        }
    }
}