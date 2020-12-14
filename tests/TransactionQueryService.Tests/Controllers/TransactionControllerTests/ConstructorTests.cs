using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Glasswall.Administration.K8.TransactionQueryService;
using Glasswall.Administration.K8.TransactionQueryService.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Tests.Controllers.TransactionControllerTests
{
    [TestFixture]
    public class ConstructorTests : TransactionControllerTestBase
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<TransactionController>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<TransactionController>();
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes()
        {
            var attributes = typeof(TransactionController).GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(3).Items);
            Assert.That(attributes[0], Is.InstanceOf<ApiControllerAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<RouteAttribute>().With.Property(nameof(RouteAttribute.Template)).EqualTo("api/v1/transactions"));
        }
        
        [Test]
        public void Constructed_Class_Has_Correct_Attributes_For_GetTransactions_Method()
        {
            var attributes = typeof(TransactionController).GetMethod(nameof(TransactionController.GetTransactions))?.GetCustomAttributes(false);

            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes[0], Is.InstanceOf<AsyncStateMachineAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<DebuggerStepThroughAttribute>());
            Assert.That(attributes[2], Is.InstanceOf<HttpPostAttribute>());
            Assert.That(attributes[3], Is.InstanceOf<ValidateModelAttribute>());
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes_For_GetTransactions_Method_Args()
        {
            var methodArgParamAttributes = typeof(TransactionController).GetMethod(nameof(TransactionController.GetTransactions))?.GetParameters()[0]?.GetCustomAttributes(false);

            Assert.That(methodArgParamAttributes, Is.Not.Null);
            Assert.That(methodArgParamAttributes, Has.Exactly(2).Items);
            Assert.That(methodArgParamAttributes[0], Is.InstanceOf<RequiredAttribute>());
            Assert.That(methodArgParamAttributes[1], Is.InstanceOf<FromBodyAttribute>());
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes_For_GetDetail_Method()
        {
            var attributes = typeof(TransactionController).GetMethod(nameof(TransactionController.GetDetail))?.GetCustomAttributes(false);

            Assert.That(attributes, Is.Not.Null);
            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes[0], Is.InstanceOf<AsyncStateMachineAttribute>());
            Assert.That(attributes[1], Is.InstanceOf<DebuggerStepThroughAttribute>());
            Assert.That(attributes[2], Is.InstanceOf<HttpGetAttribute>());
            Assert.That(attributes[3], Is.InstanceOf<ValidateModelAttribute>());
        }

        [Test]
        public void Constructed_Class_Has_Correct_Attributes_For_GetDetail_Method_Args()
        {
            var methodArgParamAttributes = typeof(TransactionController).GetMethod(nameof(TransactionController.GetDetail))?.GetParameters()[0]?.GetCustomAttributes(false);

            Assert.That(methodArgParamAttributes, Is.Not.Null);
            Assert.That(methodArgParamAttributes, Has.Exactly(2).Items);
            Assert.That(methodArgParamAttributes[0], Is.InstanceOf<RequiredAttribute>());
            Assert.That(methodArgParamAttributes[1], Is.InstanceOf<FromQueryAttribute>());
        }

    }
}