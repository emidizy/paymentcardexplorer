using AppCore.Shared.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Shared.Services
{
    public class HttpCache : IHttpCache
    {
        /// <summary>
        /// Given the scope of this implementation, I used IMemoryCache. Although this is quite volatile, it achieves the requirements. 
        /// For production purpose, a distributed cache like redis is highly recommended as IMemorycache may not survive app restart.
        /// </summary>
        private readonly IMemoryCache _cache;

        public HttpCache(IMemoryCache memCache)
        {
            _cache = memCache;
        }

        /// <summary>
        /// Save record to in-memory
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="secondInterval"></param>
        public void SaveToCache(string key, object data, int secondInterval)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(secondInterval));
            _cache.Set(key, data, cacheEntryOptions);

        }

        /// <summary>
        /// Retrieve data from the cache using the key that was used to save it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T LoadFromCache<T>(string key) where T : new()
        {
            var result = (T)_cache.Get(key);
            return result;
        }

        /// <summary>
        /// Use the Key to remove item from the cache
        /// </summary>
        /// <param name="key"></param>
        public void RemoveFromCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
