using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Xunit;

namespace SlidingCacheAop.Tests
{
    public class CacheResultInterceptorTests
    {
        private CacheResultInterceptor _interceptor;
        private IDoThings _interceptedObject;

        public CacheResultInterceptorTests()
        {
            CreateInterceptor();
            CreateInterceptedObject();
        }

        [Fact]
        public void CallNonInterceptedMethodTwiceShouldReturnDifferentResults()
        {
            var firstResult = _interceptedObject.DoSomething();
            var secondResult = _interceptedObject.DoSomething();

            Assert.NotEqual(firstResult, secondResult);
        }

        [Fact]
        public void CallInterceptedMethodTwiceShouldReturnTheSameResult()
        {
            var firstResult = _interceptedObject.DoSomethingElse();
            var secondResult = _interceptedObject.DoSomethingElse();

            Assert.Equal(firstResult, secondResult);
        }

        [Fact]
        public void CallInterceptedMethodAgainAfterCacheExpirationShouldReturnDifferentResult()
        {
            var firstResult = _interceptedObject.DoSomethingElse();
            Thread.Sleep(11);
            var secondResult = _interceptedObject.DoSomethingElse();

            Assert.NotEqual(firstResult, secondResult);
        }

        [Fact]
        public void CallInterceptedMethodFromMultipleTasksShouldReturnTheSameResult()
        {
            var tasks = CreateTasksOfInterceptedMethods();

            Task.WaitAll(tasks);

            for (int i = 1; i < tasks.Length; i++)
                Assert.Equal(tasks[0].Result, tasks[i].Result);
        }

        [Fact]
        public void CallInterceptedMethodWithDifferentArgumentsShouldReturnDifferentResults()
        {
            var firstResult = _interceptedObject.DoSomethingElse("a", 0);
            var secondResult = _interceptedObject.DoSomethingElse("b", 0);

            Assert.NotEqual(firstResult, secondResult);
        }

        private void CreateInterceptor()
        {
            var cache = new MemorySlidingCache();
            _interceptor = new CacheResultInterceptor(cache);
        }

        private void CreateInterceptedObject()
        {
            var proxyGenerator = new ProxyGenerator();
            _interceptedObject = (IDoThings)proxyGenerator.CreateClassProxy(
                typeof(ClassThatWillBeIntercepted),
                new[] { typeof(IDoThings) },
                _interceptor
            );
        }

        private Task<string>[] CreateTasksOfInterceptedMethods()
        {
            return new[]
            {
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse())
            };
        }
    }
}
