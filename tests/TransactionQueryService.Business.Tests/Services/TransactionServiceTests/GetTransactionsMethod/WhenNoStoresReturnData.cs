using System;
using System.Collections.Generic;
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
    public class WhenNoStoresReturnData : TransactionServiceTestBase
    {
        private GetTransactionsRequestV1 _input;
        private Transactions _output;

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
                .Returns(GetSomePaths());

            Share2.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetSomePaths());

            _output = await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);
        }

        [Test]
        public void Each_Store_Is_Searched()
        {
            Share1.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
            Share2.Verify(s => s.ListAsync(It.IsAny<DatePathFilter>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void No_Data_Is_Downloaded()
        {
            Share1.Verify(f => f.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            Share2.Verify(f => f.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void No_Data_Is_Deserialised()
        {
            JsonSerialiser.VerifyNoOtherCalls();
        }

        [Test]
        public void Correct_Response_Is_Returned()
        {
            Assert.That(_output.Files, Is.Empty);
        }

        private static async IAsyncEnumerable<string> GetSomePaths()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}