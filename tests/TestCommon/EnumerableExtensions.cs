using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCommon
{
    public static class EnumerableExtensions
    {
        public static async IAsyncEnumerator<TItem> AsAsyncEnumerator<TItem>(this IEnumerable<TItem> items)
        {
            foreach (var item in items)
                yield return item;

            await Task.CompletedTask;
        }

        public static async Task<IEnumerable<TItem>> AsEnumerableAsync<TItem>(this IAsyncEnumerable<TItem> items)
        {
            var ret = new List<TItem>();

            await foreach (var item in items)
            {
                ret.Add(item);
            }

            return ret;
        }
    }
}