using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Tests.Controllers.TransactionControllerTests.GetMetricsMethod
{
    [TestFixture]
    public class WhenRequestIsValid : TransactionControllerTestBase
    {
        private IActionResult _result;
        private IAsyncEnumerable<DateTimeOffset> _expected;
        private DateTimeOffset _input1;
        private DateTimeOffset _input2;

        [OneTimeSetUp]
        public async Task OnetimeSetup()
        {
            base.OnetimeSetupShared();

            _expected = AsAsyncEnumerable(new[]
            {
                new DateTimeOffset(new DateTime(2020, 1, 1, 1, 0, 0)),
                new DateTimeOffset(new DateTime(2020, 1, 1, 1, 0, 0))
            });

            Service.Setup(s => s.GetHourTimestampsOfFiles(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .Returns(_expected);

            _result = await ClassInTest.GetMetrics(_input1 = DateTimeOffset.UtcNow, _input2 = DateTimeOffset.UtcNow, CancellationToken.None);
        }

        [Test]
        public void Messages_Are_Logged()
        {
            Logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.Is<EventId>(s => s == 0),
                    It.Is<object>(s => s.ToString() == "Beginning get metrics request"),
                    It.IsAny<Exception>(), 
                    (Func<object, Exception, string>) It.IsAny<object>()));

            Logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.Is<EventId>(s => s == 0),
                    It.Is<object>(s => s.ToString() == "Finished get metrics request"),
                    It.IsAny<Exception>(),
                    (Func<object, Exception, string>)It.IsAny<object>()));
        }

        [Test]
        public void Transactions_Are_Retrieved()
        {
            Service.Verify(
                s => s.GetHourTimestampsOfFiles(
                    It.Is<DateTimeOffset>(x => x == _input1),
                    It.Is<DateTimeOffset>(x => x == _input2),
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_result, Is.InstanceOf<OkObjectResult>());

            Assert.That(_result, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("totalProcessed", 2));
            Assert.That(_result, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .With.Property("data").With.One.Items.WithPropEqual("processed", 2));
        }

        private static async IAsyncEnumerable<DateTimeOffset> AsAsyncEnumerable(IEnumerable<DateTimeOffset> dates)
        {
            foreach (var date in dates)
            {
                yield return date;
            }
        }
    }
}