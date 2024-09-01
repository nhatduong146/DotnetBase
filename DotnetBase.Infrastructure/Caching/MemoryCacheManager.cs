using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace DotnetBase.Infrastructure.Caching
{
    /// <summary>
    /// Represents a MemoryCacheCache
    /// </summary>
    public partial class MemoryCacheManager : ICacheManager
    {
        public MemoryCacheManager()
        {

        }

        public MemoryCacheManager(IMemoryCache cache) : this()
        {
            _cache = cache;
        }

        protected static IMemoryCache _cache;
        protected static IMemoryCache Cache
        {
            get
            {
                return _cache;
            }
            private set
            {
                _cache = value;
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(params string[] keys)
        {
            if (keys == null)
                return;
            foreach (var key in keys)
            {
                Cache.Remove(key);
            }
        }

        public async Task<T> GetAndSetAsync<T>(string key, int cacheDays, Func<Task<T>> func)
        {
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromDays(cacheDays));

            if (!_cache.TryGetValue<T>(key, out var t))
            {
                t = await func();

                // Save data in cache.
                _cache.Set(key, t, cacheEntryOptions);
            }
            return t;
        }
    }
}
