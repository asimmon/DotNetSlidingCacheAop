using System;

namespace SlidingCacheAop
{
    public interface ISlidingCache
    {
        object Get(string key);

        void Set(string key, object valueToCache, TimeSpan slidingExpiration);
    }
}