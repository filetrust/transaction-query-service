using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using TestCommon;

namespace TransactionQueryService.Tests.ValidateModelTests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ValidateModelTests : UnitTestBase<ValidateModelAttribute>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new ValidateModelAttribute();
        }

        [Test]
        public async Task BadRequest_Set_When_Model_Errors_Occur()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            await ClassInTest.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            Assert.That(actionExecutingContext.Result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(((BadRequestObjectResult)actionExecutingContext.Result).Value, Is.InstanceOf<List<string>>());
            CollectionAssert.AreEqual((List<string>)((BadRequestObjectResult)actionExecutingContext.Result).Value, modelState.Values.SelectMany(s => s.Errors).Select(e => e.ErrorMessage).ToList());
        }

        [Test]
        public async Task BadRequest_Not_Set_When_Model_Errors_Do_Not_Occur()
        {
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            await ClassInTest.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            Assert.That(actionExecutingContext.Result, Is.Not.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void Exception_Thrown_With_Null_Context()
        {
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            Assert.That(() => ClassInTest.OnActionExecutionAsync(null, async () => await Task.FromResult(context)), Throws.ArgumentNullException);
        }


        [Test]
        public void Exception_Thrown_With_Null_Next()
        {
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };

            Assert.That(() => ClassInTest.OnActionExecutionAsync(actionExecutingContext, null), Throws.ArgumentNullException);
        }
    }
}
