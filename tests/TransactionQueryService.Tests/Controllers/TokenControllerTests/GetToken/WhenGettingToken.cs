using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace TransactionQueryService.Tests.Controllers.TokenControllerTests.GetToken
{
    [TestFixture]
    public class WhenGettingToken : TokenControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public void Setup()
        {
            OnetimeSetupShared();

            Config.Username = string.Concat(Enumerable.Repeat("t", 255).ToArray());
            Config.Password = string.Concat(Enumerable.Repeat("p", 255).ToArray());
            Config.TokenSecret = string.Concat(Enumerable.Repeat("s", 255).ToArray());

            _output = ClassInTest.GetToken();
        }

        [Test]
        public void OK_IsReturned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>());

            var ok = (OkObjectResult) _output;

            Assert.That(ok.Value, Is.InstanceOf<string>());
        }
    }
}
