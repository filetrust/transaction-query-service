using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Serialisation.JsonSerialiserTests
{
    [TestFixture]
    public class JsonSerialiserTests : UnitTestBase<ISerialiser>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new JsonSerialiser();
        }

        [Test]
        public void Null_Input_Throws_For_Serialise()
        {
            Assert.That(() => ClassInTest.Serialize<object>(null), ThrowsArgumentNullException("input"));
        }


        [Test]
        public void Null_Input_Throws_For_Deserialise_Stream()
        {
            Assert.That(() => ClassInTest.Deserialize<JsonSerialiserTests>(null, Encoding.UTF8), ThrowsArgumentNullException("input"));
        }


        [Test]
        public void Null_Input_Throws_For_Deserialise_String()
        {
            Assert.That(() => ClassInTest.Deserialize<JsonSerialiserTests>(null), ThrowsArgumentNullException("input"));
        }

        [Test]
        public async Task Serialise_Is_Correct()
        {
            var str = await ClassInTest.Serialize(new 
            {
                TestProp = "Test"
            });

            Assert.That(str, Is.EqualTo("{\"TestProp\":\"Test\"}"));
        }

        [Test]
        public async Task Deserialise_String_Is_Correct()
        {
            var obj = await ClassInTest.Deserialize<Dictionary<string, string>>("{\"TestProp\":\"Test\"}");

            Assert.That(obj["TestProp"], Is.EqualTo("Test"));
        }

        [Test]
        public async Task Deserialise_Stream_Is_Correct()
        {
            await using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("{\"TestProp\":\"Test\"}")))
            {
                var obj = await ClassInTest.Deserialize<Dictionary<string, string>>(ms, Encoding.UTF8);
                Assert.That(obj["TestProp"], Is.EqualTo("Test"));
            }
        }
    }
}