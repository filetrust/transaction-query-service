using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetTransactionsMethod
{
    [TestFixture]
    public class WhenFilterDoesNotMatch : TransactionServiceTestBase
    {
        private GetTransactionsRequestV1 _input;
        private IAsyncEnumerable<string> _paths1;
        private IAsyncEnumerable<string> _paths2;
        private Transactions _output;
        private TransactionAdapationEventMetadataFile _expectedMetadata;
        private Guid _fileId;

        [SetUp]
        public void Setup()
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

            Share1.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths1 = GetSomePaths(1));

            Share2.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths2 = GetSomePaths(2));

            _fileId = Guid.NewGuid();

            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new []
                    {
                        TransactionAdaptionEventModel.AnalysisCompletedEvent(_fileId),
                        TransactionAdaptionEventModel.FileTypeDetectedEvent(FileType.Bmp, _fileId),
                        TransactionAdaptionEventModel.NcfsCompletedEvent(NcfsOutcome.Block, _fileId),
                        TransactionAdaptionEventModel.NcfsStartedEvent(_fileId),
                        TransactionAdaptionEventModel.NewDocumentEvent(fileId: _fileId),
                        TransactionAdaptionEventModel.RebuildCompletedEvent(GwOutcome.Failed, _fileId),
                        TransactionAdaptionEventModel.RebuildEventStarting(_fileId),
                    }
                });
        }


        [Test]
        public async Task Bad_FileId_Is_FilteredOut_Filter()
        {
            var badEvent = TransactionAdaptionEventModel.NewDocumentEvent();
            badEvent.Properties["FileId"] = "Rgsjrjgkisjghr";
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        TransactionAdaptionEventModel.AnalysisCompletedEvent(_fileId),
                        TransactionAdaptionEventModel.FileTypeDetectedEvent(FileType.Bmp, _fileId),
                        TransactionAdaptionEventModel.NcfsCompletedEvent(NcfsOutcome.Block, _fileId),
                        TransactionAdaptionEventModel.NcfsStartedEvent(_fileId),
                        badEvent,
                        TransactionAdaptionEventModel.RebuildCompletedEvent(GwOutcome.Failed, _fileId),
                        TransactionAdaptionEventModel.RebuildEventStarting(_fileId),
                    }
                });

            _input.Filter.FileIds = new List<Guid> {
            {
                Guid.NewGuid()
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Bad_FileType_Is_FilteredOut_Filter()
        {
            var badEvent = TransactionAdaptionEventModel.FileTypeDetectedEvent(FileType.Coff);
            badEvent.Properties["FileType"] = "Rgsjrjgkisjghr";
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        TransactionAdaptionEventModel.AnalysisCompletedEvent(_fileId),
                        badEvent,
                        TransactionAdaptionEventModel.NcfsCompletedEvent(NcfsOutcome.Block, _fileId),
                        TransactionAdaptionEventModel.NcfsStartedEvent(_fileId),
                        TransactionAdaptionEventModel.NewDocumentEvent(),
                        TransactionAdaptionEventModel.RebuildCompletedEvent(GwOutcome.Failed, _fileId),
                        TransactionAdaptionEventModel.RebuildEventStarting(_fileId),
                    }
                });

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task NoneExistent_FileType_Is_FilteredOut_Filter()
        {
            var badEvent = TransactionAdaptionEventModel.FileTypeDetectedEvent(FileType.Coff);
            badEvent.Properties.Remove("FileType");
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        TransactionAdaptionEventModel.AnalysisCompletedEvent(_fileId),
                        badEvent,
                        TransactionAdaptionEventModel.NcfsCompletedEvent(NcfsOutcome.Block, _fileId),
                        TransactionAdaptionEventModel.NcfsStartedEvent(_fileId),
                        TransactionAdaptionEventModel.NewDocumentEvent(),
                        TransactionAdaptionEventModel.RebuildCompletedEvent(GwOutcome.Failed, _fileId),
                        TransactionAdaptionEventModel.RebuildEventStarting(_fileId),
                    }
                });

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task No_Event_Selected_With_Wrong_FileId_Filter()
        {
            _input.Filter.FileIds = new List<Guid> {
            {
                Guid.NewGuid()
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task No_Event_Selected_With_Correct_FileId_Filter()
        {
            _input.Filter.FileIds = new List<Guid> {
            {
                _fileId
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task No_Event_Selected_With_Wrong_FileType_Filter()
        {
            _input.Filter.FileTypes = new List<FileType> {
            {
                FileType.Coff
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task No_Event_Selected_With_Wrong_Risk_Filter()
        {
            _input.Filter.Risks = new List<Risk> {
            {
                Risk.Unknown
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task No_Event_Selected_With_Wrong_Policy_Filter()
        {
            _input.Filter.PolicyIds = new List<Guid> {
            {
                Guid.Empty
            }};

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);

            Assert.That(_output.Count, Is.EqualTo(0));
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