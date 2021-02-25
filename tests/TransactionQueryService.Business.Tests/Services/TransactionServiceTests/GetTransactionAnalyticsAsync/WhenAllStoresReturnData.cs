using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.Metrics;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetTransactionAnalyticsAsync
{
    [TestFixture]
    public class WhenAllStoresReturnData : TransactionServiceTestBase
    {
        private IAsyncEnumerable<string> _paths1;
        private IAsyncEnumerable<string> _paths2;
        private TransactionAnalytics _output;
        private TransactionAdapationEventMetadataFile _expectedMetadata;

        [OneTimeSetUp]
        public async Task Setup()
        {
            base.SharedSetup();

            var fileId = Guid.NewGuid();

            _paths1 = GetSomePaths(1);
            _paths2 = GetSomePaths(2);

            Share1.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths1);

            Share2.Setup(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths2);

            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        TransactionAdaptionEventModel.AnalysisCompletedEvent(fileId),
                        TransactionAdaptionEventModel.FileTypeDetectedEvent(FileType.Bmp, fileId),
                        TransactionAdaptionEventModel.NcfsCompletedEvent(NcfsOutcome.Block, fileId),
                        TransactionAdaptionEventModel.NcfsStartedEvent(fileId),
                        TransactionAdaptionEventModel.NewDocumentEvent(fileId),
                        TransactionAdaptionEventModel.RebuildCompletedEvent(GwOutcome.Failed, fileId),
                        TransactionAdaptionEventModel.RebuildEventStarting(fileId),
                    }
                });

            _output = await ClassInTest.GetTransactionAnalyticsAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, CancellationToken.None);
        }

        [Test]
        public void Each_Store_Is_Searched()
        {
            Share1.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
            Share2.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Test]
        public void Correct_Response_Is_Returned()
        {
            Assert.That(_output.TotalProcessed, Is.EqualTo(10));
            Assert.That(_output.Data, Has.One.Items);
            Assert.That(_output.Data.ElementAt(0).Processed, Is.EqualTo(10));
            Assert.That(_output.Data.ElementAt(0).SentToNcfs, Is.EqualTo(10));
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