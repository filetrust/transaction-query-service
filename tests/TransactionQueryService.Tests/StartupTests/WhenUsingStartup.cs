using System.Collections.Generic;
using System.Linq;
using Glasswall.Administration.K8.TransactionQueryService;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using TestCommon;

namespace TransactionQueryService.Tests.StartupTests
{
    [TestFixture]
    public class WhenUsingStartup : UnitTestBase<Startup>
    {
        //[OneTimeSetUp]
        //public void Setup()
        //{
        //    ClassInTest = new Startup(Mock.Of<IConfiguration>());

        //    Directory.CreateDirectory("/mnt/stores/store1");
        //}

        //[OneTimeTearDown]
        //public void Teardown()
        //{
        //    Directory.Delete("/mnt/stores/store1");
        //}

        //[Test]
        //public void Can_Resolve_Transaction_Service()
        //{

        //    var services = new ServiceCollection();

        //    Environment.SetEnvironmentVariable("username", "username");
        //    Environment.SetEnvironmentVariable("password", "password");

        //    ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

        //    ClassInTest.ConfigureServices(services);

        //    Assert.That(services.Any(s =>
        //        s.ServiceType == typeof(IEnumerable<IFileStore>)), "No file store was added");

        //    services.BuildServiceProvider().GetRequiredService<ITransactionService>();
        //}
        
        //[Test]
        //public void Constructor_Constructs_With_Mocked_Params()
        //{
        //    ConstructorAssertions.ConstructsWithMockedParameters<Startup>();
        //}

        //[Test]
        //public void Constructor_Is_Guarded_Against_Null()
        //{
        //    ConstructorAssertions.ClassIsGuardedAgainstNull<Startup>();
        //}
    }
}
