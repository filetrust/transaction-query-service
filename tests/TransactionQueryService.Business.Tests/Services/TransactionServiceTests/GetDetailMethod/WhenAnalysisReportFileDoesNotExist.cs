using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetDetailMethod
{
    [TestFixture]
    public class WhenAnalysisReportFileDoesNotExist : TransactionServiceTestBase
    {
        private TransactionDetail _output;
        private const string Input = "some/file/path";

        [OneTimeSetUp]
        public async Task Setup()
        {
            base.SharedSetup();

            Share1.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            Share1.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as MemoryStream);

            _output = await ClassInTest.GetDetailAsync(Input, CancellationToken.None);
        }

        [Test]
        public void Correct_Status_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.Status, Is.EqualTo(DetailStatus.AnalysisReportNotFound));
        }

        [Test]
        public void AnalysisReport_Is_Not_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.AnalysisReport, Is.Null);
        }

        [Test]
        public void Download_Is_Attempted()
        {
            Share1.Verify(s => s.DownloadAsync(It.Is<string>(x => x == $"{Input}/report.xml"), It.IsAny<CancellationToken>()), Times.Once);
            Share2.VerifyNoOtherCalls();
        }

        [Test]
        public void Directory_Is_Checked_For_Existence()
        {
            Share1.Verify(s => s.ExistsAsync(It.Is<string>(x => x == Input), It.IsAny<CancellationToken>()), Times.Once);
            Share2.VerifyNoOtherCalls();
        }
    }
}