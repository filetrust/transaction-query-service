using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Serialisation
{
    public class XmlSerialiser : IXmlSerialiser
    {
        public Task<TResult> Deserialize<TResult>(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var serializer = new XmlSerializer(typeof(TResult));
            var reader = new StreamReader(ms);
            var obj = (TResult) serializer.Deserialize(reader);
            reader.Close();

            return Task.FromResult(obj);
        }

        public Task<TResult> Deserialize<TResult>(Stream input, Encoding encoding)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            var serializer = new XmlSerializer(typeof(TResult));
            input.Position = 0;
            var obj = (TResult)serializer.Deserialize(input);
            return Task.FromResult(obj);
        }

        public Task<string> Serialize<TInput>(TInput input)
        {
            throw new NotImplementedException();
        }
    }
}