using Castle.DynamicProxy;

namespace SlidingCacheAop
{
    public class CacheResultInterceptor : IInterceptor
    {
        private readonly ISlidingCache _cache;

        public CacheResultInterceptor(ISlidingCache cache)
        {
            _cache = cache;
        }

        public void Intercept(IInvocation invocation)
        {
            new CacheResultHandler(_cache, invocation).Intercept();
        }
    }
}
