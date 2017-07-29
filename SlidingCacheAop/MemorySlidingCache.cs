using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace SlidingCacheAop.WinApp
{
	public class MemorySlidingCache : ISlidingCache
	{
		private readonly Lazy<MemoryCache> DefaultMemoryCacheFactory = new Lazy<MemoryCache>(() =>
		{
			var options = Options.Create(new MemoryCacheOptions());
			return new MemoryCache(options);
		});

		private MemoryCache DefaultMemoryCache
		{
			get { return DefaultMemoryCacheFactory.Value; }
		}

		public object Get(string key)
		{
			return DefaultMemoryCache.Get(key);
		}

        public void Set(string key, object valueToCache, TimeSpan slidingExpiration)
		{
            if (slidingExpiration.Ticks <= 0)
				throw new ArgumentException("SlidingExpiration cannot be less or equal to zero", nameof(slidingExpiration));

			var cachePolicy = new MemoryCacheEntryOptions
			{
                AbsoluteExpiration = DateTime.Now.Add(slidingExpiration)
			};

			DefaultMemoryCache.Set(key, valueToCache, cachePolicy);
		}
	}
}
