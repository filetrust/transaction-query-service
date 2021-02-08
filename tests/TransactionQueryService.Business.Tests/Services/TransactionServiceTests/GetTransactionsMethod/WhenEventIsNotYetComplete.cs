using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetTransactionsMethod
{
    [TestFixture]
    public class WhenEventIsNotYetComplete : TransactionServiceTestBase
    {
        private GetTransactionsRequestV1 _input;
        private IAsyncEnumerable<string> _paths1;
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

            Share1.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(_paths1 = GetSomePaths(1));

            Share2.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetNoPaths());

            var fileId = Guid.NewGuid();

            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_expectedMetadata = new TransactionAdapationEventMetadataFile
                {
                    Events = new []
                    {
                        TransactionAdaptionEventModel.NewDocumentEvent(fileId)
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
        }

        [Test]
        public async Task Correct_Response_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);

            await foreach (var path in _paths1)
            {
                Assert.That(_output.Files.Any(s => s.Directory == path));
            }
        }

        private static async IAsyncEnumerable<string> GetSomePaths(int store)
        {
            for (var index = 0; index < 1; index++)
            {
                yield return $"some/path/{store}/{index}";
            }

            await Task.CompletedTask;
        }

        private static async IAsyncEnumerable<string> GetNoPaths()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}