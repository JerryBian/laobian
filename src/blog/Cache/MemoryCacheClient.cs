using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Laobian.Blog.Cache
{
    public class MemoryCacheClient : ICacheClient, IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheClient()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> func, TimeSpan? expireAfter = null)
        {
            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                var val = await func();
                cacheEntry.Value = val;
                cacheEntry.AbsoluteExpirationRelativeToNow = expireAfter;
                cacheEntry.ExpirationTokens.Add(new CacheChangeToken());

                return val;
            });
        }

        public void Dispose()
        {
            _memoryCache?.Dispose();
        }
    }
}