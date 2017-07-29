using System.Collections.Concurrent;
using Castle.DynamicProxy;

namespace SlidingCacheAop.WinApp
{
    internal class CacheResultHandler
    {
        private static readonly ConcurrentDictionary<string, object> _lockObjects =
            new ConcurrentDictionary<string, object>();

        private CachedMethodInvocation _invocation;
        private ISlidingCache _cache;
        private object _currentLockObject;

        static CacheResultHandler() { }

        public CacheResultHandler(ISlidingCache cache, IInvocation invocation)
        {
            _cache = cache;
            _invocation = new CachedMethodInvocation(invocation);
        }

        public void Intercept()
        {
            if (_invocation.IsCacheEnabled())
                InterceptWithCache();
            else
                InterceptWithoutCache();
        }

        private void InterceptWithoutCache()
        {
            _invocation.Proceed();
        }

        private void InterceptWithCache()
        {
            TrySetMethodResultFromCache();
            if (!IsMethodResultSet())
                ProceedInvocationWithCacheThreadSafe();
        }

        private void TrySetMethodResultFromCache()
        {
            var cachedValue = _cache.Get(_invocation.Signature);
            if (cachedValue != null)
                _invocation.ReturnValue = cachedValue;
        }

        private bool IsMethodResultSet()
        {
            return _invocation.ReturnValue != null;
        }

        private void ProceedInvocationWithCacheThreadSafe()
        {
            GetOrAddDynamicLockObjectBasedOnMethodInvocation();
            lock (_currentLockObject)
                ProceedInvocationWithCache();
        }

        private void GetOrAddDynamicLockObjectBasedOnMethodInvocation()
        {
            _currentLockObject = _lockObjects.GetOrAdd(_invocation.Signature, idx => new object());
        }

        private void ProceedInvocationWithCache()
        {
            try
            {
                DoubleCheckCacheThenProceedInvocationWithCache();
            }
            finally
            {
                ReleaseCurrentLockObject();
            }
        }

        private void DoubleCheckCacheThenProceedInvocationWithCache()
        {
            TrySetMethodResultFromCache();
            if (!IsMethodResultSet())
                ProceedInvocationThenCacheResult();
        }

        private void ProceedInvocationThenCacheResult()
        {
            ProceedInvocation();
            CacheResult();
        }

        private void ProceedInvocation()
        {
            _invocation.Proceed();
        }

        private void CacheResult()
        {
            if (IsMethodResultSet())
                _cache.Set(_invocation.Signature, _invocation.ReturnValue, _invocation.CacheDuration);
        }

        private void ReleaseCurrentLockObject()
        {
            _lockObjects.TryRemove(_invocation.Signature, out object removedLockObject);
            _currentLockObject = null;
        }
    }
}
