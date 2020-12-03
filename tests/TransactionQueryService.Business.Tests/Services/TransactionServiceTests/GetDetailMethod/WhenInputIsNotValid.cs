using System;
using System.Threading;
using NUnit.Framework;

namespace TransactionQueryService.Business.Tests.Services.TransactionServiceTests.GetDetailMethod
{
    [TestFixture]
    public class WhenInputIsNotValid : TransactionServiceTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            base.SharedSetup();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Throws_With_Invalid_Path(string path)
        {
            Assert.That(() => ClassInTest.GetDetailAsync(path, CancellationToken.None), 
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName))
                    .EqualTo("fileDirectory")
                    .And
                    .With.Property(nameof(ArgumentException.Message))
                    .StartWith("Value must not be null or whitespace"));
        }
    }
}
