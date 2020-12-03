using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation;
using Newtonsoft.Json;

namespace Glasswall.Administration.K8.TransactionQueryService.Business.Serialisation
{
    public class JsonSerialiser : IJsonSerialiser
    {
        public Task<TInput> Deserialize<TInput>(Stream input, Encoding encoding)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return InternalDeserializeStreamAsync<TInput>(input, encoding);
        }

        public Task<TOutput> Deserialize<TOutput>(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(JsonConvert.DeserializeObject<TOutput>(input));
        }

        public Task<string> Serialize<TInput>(TInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(JsonConvert.SerializeObject(input));
        }

        private static async Task<TInput> InternalDeserializeStreamAsync<TInput>(Stream input, Encoding encoding)
        {
            var bytes = new byte[input.Length];
            input.Position = 0;
            await input.ReadAsync(bytes, 0, (int)input.Length);
            return JsonConvert.DeserializeObject<TInput>(encoding.GetString(bytes));
        }
    }
}