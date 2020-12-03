using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Business.Serialisation;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using NUnit.Framework;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Serialisation.XmlSerialiserTests
{
    [TestFixture]
    public class XmlSerialiserTests : UnitTestBase<ISerialiser>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new XmlSerialiser();
        }

        [Test]
        public void Null_Input_Throws_For_Serialise()
        {
            Assert.That(() => ClassInTest.Serialize<object>(null), Throws.Exception.InstanceOf<NotImplementedException>());
        }

        [Test]
        public void Null_Encoding_Throws_For_Deserialise()
        {
            Assert.That(() => ClassInTest.Deserialize<object>(new MemoryStream(), null), ThrowsArgumentNullException("encoding"));
        }

        [Test]
        public void Null_Input_Throws_For_Deserialise_Stream()
        {
            Assert.That(() => ClassInTest.Deserialize<JsonSerialiserTests.JsonSerialiserTests>(null, Encoding.UTF8), ThrowsArgumentNullException("input"));
        }


        [Test]
        public void Null_Input_Throws_For_Deserialise_String()
        {
            Assert.That(() => ClassInTest.Deserialize<JsonSerialiserTests.JsonSerialiserTests>(null), ThrowsArgumentNullException("input"));
        }

        [Test]
        public async Task Deserialise_String_Is_Correct()
        {
            var obj = await ClassInTest.Deserialize<xml>("<xml><TestProp>Test</TestProp></xml>");

            Assert.That(obj.TestProp, Is.EqualTo("Test"));
        }

        [Test]
        public async Task Deserialise_Stream_Is_Correct()
        {
            await using var ms = new MemoryStream(Encoding.UTF8.GetBytes("<xml><TestProp>Test</TestProp></xml>"));

            var obj = await ClassInTest.Deserialize<xml>(ms, Encoding.UTF8);

            Assert.That(obj.TestProp, Is.EqualTo("Test"));
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class xml
    {
        private string testPropField;

        /// <remarks/>
        public string TestProp
        {
            get => this.testPropField;
            set => this.testPropField = value;
        }
    }


}