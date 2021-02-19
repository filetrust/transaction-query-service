using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetTransactionsMethod
{
    [TestFixture]
    public class WhenAllStoresReturnData : TransactionServiceTestBase
    {
        private GetTransactionsRequestV1 _input;
        private IAsyncEnumerable<string> _paths1;
        private IAsyncEnumerable<string> _paths2;
        private Transactions _output;
        private TransactionAdapationEventMetadataFile _expectedMetadata;

        [OneTimeSetUp]
        public async Task Setup()
        {
            base.SharedSetup();

            _input = new GetTransactionsRequestV1
            {
                Filter = new FileStoreFilterV1
                {
                    TimestampRangeStart = DateTimeOffset.MinValue,
                    TimestampRangeEnd = DateTimeOffset.MaxValue
                }
            };
            
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
                    Events = new []
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

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);
        }

        [Test]
        public void Each_Store_Is_Searched()
        {
            Share1.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
            Share2.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Each_Store_Data_Is_Downloaded()
        {
            await foreach (var i in _paths1)
            {
                Share1.Verify(s => s.DownloadAsync(It.Is<string>(x => x == $"{i}/metadata.json"), It.IsAny<CancellationToken>()), Times.Once);
            }

            await foreach (var i in _paths2)
            {
                Share2.Verify(s => s.DownloadAsync(It.Is<string>(x => x == $"{i}/metadata.json"), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Test]
        public async Task Correct_Response_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);

            await foreach (var path in _paths1)
            {
                var obj = _output.Files.Single(s => s.Directory == path);
                
                Assert.That(obj, Is.Not.Null);
                Assert.That(obj.ActivePolicyId.ToString(), Is.EqualTo(_expectedMetadata.Events.EventOrDefault(EventId.NewDocument).Properties["PolicyId"]));
                Assert.That(obj.FileId.ToString(), Is.EqualTo(_expectedMetadata.Events.EventOrDefault(EventId.NewDocument).Properties["FileId"]));
                Assert.That((int)obj.DetectionFileType, Is.EqualTo(int.Parse(_expectedMetadata.Events.EventOrDefault(EventId.FileTypeDetected).Properties["FileType"])));
                Assert.That(obj.Risk, Is.EqualTo(Risk.BlockedByNCFS));
                Assert.That(obj.Timestamp, Is.EqualTo(DateTimeOffset.Parse(_expectedMetadata.Events.EventOrDefault(EventId.NewDocument).Properties["Timestamp"])));
            }

            await foreach (var path in _paths2)
            {
                Assert.That(_output.Files.Any(s => s.Directory == path));
            }
        }

        private static async IAsyncEnumerable<string> GetSomePaths(int store)
        {
            for (var index = 0; index < 5; index++)
            {
                yield return $"some/path/{store}/{index}";
            }

            await Task.CompletedTask;
        }
    }
}