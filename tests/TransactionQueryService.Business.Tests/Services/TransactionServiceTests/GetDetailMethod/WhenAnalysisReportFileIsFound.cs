using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Enums;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.AnalysisReport;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Moq;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetDetailMethod
{
    [TestFixture]
    public class WhenAnalysisReportFileIsFound : TransactionServiceTestBase
    {
        private TransactionDetail _output;
        private MemoryStream _memoryStream;
        private GWallInfo _analysisReportDeserialised;
        private const string Input = "some/file/path";

        [OneTimeSetUp]
        public async Task Setup()
        {
            base.SharedSetup();

            Share1.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            Share1.Setup(s => s.DownloadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream = new MemoryStream());

            XmlSerialiser.Setup(s => s.Deserialize<GWallInfo>(It.IsAny<Stream>(), It.IsAny<Encoding>()))
                .ReturnsAsync(_analysisReportDeserialised = new GWallInfo());

            _output = await ClassInTest.GetDetailAsync(Input, CancellationToken.None);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _memoryStream.DisposeAsync();
        }

        [Test]
        public void Correct_Status_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.Status, Is.EqualTo(DetailStatus.Success));
        }

        [Test]
        public void AnalysisReport_Is_Deserialised()
        {
            XmlSerialiser.Verify(x => x. Deserialize<GWallInfo>(It.Is<Stream>(
                input => input == _memoryStream), It.Is<Encoding>(input => Equals(input, Encoding.UTF8))));
        }

        [Test]
        public void AnalysisReport_Is_Returned()
        {
            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.AnalysisReport, Is.EqualTo(_analysisReportDeserialised));
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