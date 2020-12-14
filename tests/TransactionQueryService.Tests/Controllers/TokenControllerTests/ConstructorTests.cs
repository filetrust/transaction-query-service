using Glasswall.Administration.K8.TransactionQueryService.Authentication;
using Glasswall.Administration.K8.TransactionQueryService.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Tests.Controllers.TokenControllerTests
{
    [TestFixture]
    public class ConstructorTests : TokenControllerTestBase
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<TokenController>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<TokenController>();
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes()
        {
            var attributes = typeof(TokenController).GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(2).Items);
            Assert.That(attributes[0], Is.InstanceOf<ApiControllerAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<RouteAttribute>().With.Property(nameof(RouteAttribute.Template)).EqualTo("api/v1/token"));
        }
        
        [Test]
        public void Constructed_Class_Has_Correct_Attributes_For_GetTransactions_Method()
        {
            var attributes = typeof(TokenController).GetMethod(nameof(TokenController.GetToken))?.GetCustomAttributes(false);

            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes, Has.Exactly(2).Items);
            Assert.That(attributes[0], Is.InstanceOf<HttpGetAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<BasicAuthAttribute>());
        }
    }
}