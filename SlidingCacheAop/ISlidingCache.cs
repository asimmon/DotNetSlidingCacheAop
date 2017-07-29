using System;

namespace SlidingCacheAop.WinApp
{
    public interface ISlidingCache
    {
        object Get(string key);

        void Set(string key, object valueToCache, TimeSpan slidingExpiration);
    }
}