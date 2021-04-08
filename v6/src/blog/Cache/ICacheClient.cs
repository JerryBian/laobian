using System;
using System.Threading.Tasks;

namespace Laobian.Blog.Cache
{
    public interface ICacheClient
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> func, TimeSpan? expireAfter = null);
    }
}