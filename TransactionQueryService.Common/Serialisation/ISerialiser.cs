using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Glasswall.Administration.K8.TransactionQueryService.Common.Serialisation
{
    public interface ISerialiser
    {
        Task<TResult> Deserialize<TResult>(string input);
        Task<TResult> Deserialize<TResult>(Stream input, Encoding encoding);
        Task<string> Serialize<TInput>(TInput input);
    }
}
