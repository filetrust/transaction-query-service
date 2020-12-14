using Glasswall.Administration.K8.TransactionQueryService.Common.Configuration;
using Glasswall.Administration.K8.TransactionQueryService.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace TransactionQueryService.Tests.Controllers.TokenControllerTests
{
    public class TokenControllerTestBase : UnitTestBase<TokenController>
    {
        protected Mock<ILogger<TokenController>> Logger;
        protected TransactionQueryServiceConfiguration Config;

        public void OnetimeSetupShared()
        {
            Logger = new Mock<ILogger<TokenController>>();
            Config = new TransactionQueryServiceConfiguration();

            ClassInTest = new TokenController(Logger.Object, Config);
        }
    }
}
