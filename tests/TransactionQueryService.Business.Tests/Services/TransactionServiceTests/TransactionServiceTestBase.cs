using Glasswall.Administration.K8.TransactionQueryService.Business.Services;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests
{
    public class TransactionServiceTestBase : UnitTestBase<TransactionService>
    {
        protected Mock<ILogger<ITransactionService>> Logger;
        protected Mock<IJsonSerialiser> JsonSerialiser;
        protected Mock<IXmlSerialiser> XmlSerialiser;

        protected Mock<IFileStore> Share1;
        protected Mock<IFileStore> Share2;

        public void SharedSetup()
        {
            Logger = new Mock<ILogger<ITransactionService>>();
            JsonSerialiser = new Mock<IJsonSerialiser>();
            XmlSerialiser = new Mock<IXmlSerialiser>();
            Share1 = new Mock<IFileStore>();
            Share2 = new Mock<IFileStore>();

            ClassInTest = new TransactionService(Logger.Object, new [] { Share1.Object, Share2.Object }, JsonSerialiser.Object, XmlSerialiser.Object);
        }
    }
}