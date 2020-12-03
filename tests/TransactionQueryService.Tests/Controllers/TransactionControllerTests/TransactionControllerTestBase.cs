using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Glasswall.Administration.K8.TransactionQueryService.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace TransactionQueryService.Tests.Controllers.TransactionControllerTests
{
    public class TransactionControllerTestBase : UnitTestBase<TransactionController>
    {
        protected Mock<ILogger<TransactionController>> Logger;
        protected Mock<ITransactionService> Service;

        public void OnetimeSetupShared()
        {
            Logger = new Mock<ILogger<TransactionController>>();
            Service = new Mock<ITransactionService>();

            ClassInTest = new TransactionController(Logger.Object, Service.Object);
        }
    }
}
