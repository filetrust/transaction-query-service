using System;
using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Glasswall.Administration.K8.TransactionQueryService.Common.Models.V1;
using Glasswall.Administration.K8.TransactionQueryService.Common.Services;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Store.DatePathFilterTests.DecideAction
{
    [TestFixture]
    public class WhenSpanningMultipleYears : UnitTestBase<IPathFilter>
    {
        private FileStoreFilterV1 _input;

        [OneTimeSetUp]
        public void Setup()
        {
            _input = new FileStoreFilterV1
            {
                TimestampRangeStart = new DateTimeOffset(2019, 2, 1, 0, 0, 0, 0, TimeSpan.Zero),
                TimestampRangeEnd = new DateTimeOffset(2021, 1, 1, 0, 0, 0, 0, TimeSpan.Zero)
            };

            ClassInTest = new DatePathFilter(_input);
        }

        [Test]
        [TestCase("2020", PathAction.Recurse)]
        [TestCase("2019", PathAction.Recurse)]
        [TestCase("2021", PathAction.Recurse)]
        [TestCase("2020/5", PathAction.Recurse)]
        [TestCase("2019/5", PathAction.Recurse)]
        [TestCase("2021/1", PathAction.Recurse)]
        [TestCase("2020/5/1", PathAction.Recurse)]
        [TestCase("2019/5/1", PathAction.Recurse)]
        [TestCase("2021/1/1", PathAction.Recurse)]
        [TestCase("2020/5/1/0", PathAction.Recurse)]
        [TestCase("2019/5/1/0", PathAction.Recurse)]
        [TestCase("2021/1/1/0", PathAction.Recurse)]
        [TestCase(null, PathAction.Stop)]
        [TestCase("", PathAction.Stop)]
        [TestCase(" ", PathAction.Stop)]
        [TestCase("2018", PathAction.Stop)]
        [TestCase("2022", PathAction.Stop)]
        [TestCase("2021/2", PathAction.Stop)]
        [TestCase("2021/1/2", PathAction.Stop)]
        [TestCase("2021/1/1/1", PathAction.Stop)]
        [TestCase("gobildygoop", PathAction.Stop)]
        [TestCase("gobildygoop/1", PathAction.Stop)]
        [TestCase("gobildygoop/1/1", PathAction.Stop)]
        [TestCase("gobildygoop/1/1/1", PathAction.Stop)]
        [TestCase("gob/il/dy/goop", PathAction.Stop)]
        [TestCase("2020/il", PathAction.Stop)]
        [TestCase("2020/il/1", PathAction.Stop)]
        [TestCase("2020/il/1/1", PathAction.Stop)]
        [TestCase("2020/1/dy", PathAction.Stop)]
        [TestCase("2020/1/dy/1", PathAction.Stop)]
        [TestCase("2020/1/1/goop", PathAction.Stop)]
        [TestCase("2021/1/1/0/950f7eca-7869-412f-8629-2006ef348ea4", PathAction.Collect)]
        [TestCase("2021/1/1/0/deadbeef-7869-412f-8629-2006ef348ea4", PathAction.Collect)]
        [TestCase("2021/1/1/0/feedbeef-7869-412f-8629-2006ef348ea4", PathAction.Collect)]
        [TestCase("2021/1/1/0/feeddead-7869-412f-8629-2006ef348ea4", PathAction.Collect)]
        [TestCase("2021/1/1/0/deddedde-7869-412f-8629-2006ef348ea4", PathAction.Collect)]
        [TestCase("2021/1/1/1/1/1", PathAction.Collect)]
        public void Action_Is_Correct(string path, PathAction expectedAction)
        {
            var output = ClassInTest.DecideAction(path);
             
            Assert.That(output, Is.EqualTo(expectedAction), $"{path} - {expectedAction}");
        }
    }
}
