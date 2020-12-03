using System;
using System.Collections.Generic;
using System.IO;
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
    public class WhenEventIsMalformed : TransactionServiceTestBase
    {
        private GetTransactionsRequestV1 _input;

        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();

            _input = new GetTransactionsRequestV1
            {
                Filter = new FileStoreFilterV1
                {
                    TimestampRangeStart = DateTimeOffset.MinValue,
                    TimestampRangeEnd = DateTimeOffset.MaxValue
                }
            };

            Share1.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetSomePaths(1));

            Share2.Setup(s => s.ListAsync(It.IsAny<IPathFilter>(), It.IsAny<CancellationToken>()))
                .Returns(GetNoPaths());
        }

        [Test]
        public async Task No_Exception_Is_Thrown_With_Missing_Keys()
        {
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        new TransactionAdaptionEventModel()
                    }
                });

            await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);
        }

        [Test]
        public async Task No_Exception_Is_Thrown_With_Missing_Enum()
        {
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        new TransactionAdaptionEventModel
                        {
                            Properties = new Dictionary<string, string> {
                                ["EventId"] = "2",
                            }
                        }
                    }
                });

            await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);
        }

        [Test]
        public async Task No_Exception_Is_Thrown_With_Malformed_Enum()
        {
            JsonSerialiser.Setup(s => s.Deserialize<TransactionAdapationEventMetadataFile>(It.IsAny<MemoryStream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(new TransactionAdapationEventMetadataFile
                {
                    Events = new[]
                    {
                        new TransactionAdaptionEventModel
                        {
                            Properties = new Dictionary<string, string> {
                                ["EventId"] = "2",
                                ["FileType"] = "banana"
                            }
                        }
                    }
                });

            await ClassInTest.GetTransactionsAsync(_input, CancellationToken.None);
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