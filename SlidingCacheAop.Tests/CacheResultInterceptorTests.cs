using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SlidingCacheAop.WinApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlidingCacheAop.Tests
{
    [TestClass]
    public class CacheResultInterceptorTests
    {
        private CacheResultInterceptor _interceptor;
        private IDoThings _interceptedObject;

        [TestInitialize]
        public void SetUp()
        {
            CreateInterceptor();
            CreateInterceptedObject();
        }

        [TestMethod]
        public void CallNonInterceptedMethodTwiceShouldReturnDifferentResults()
        {
            var firstResult = _interceptedObject.DoSomething();
            var secondResult = _interceptedObject.DoSomething();

            Assert.AreNotEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void CallInterceptedMethodTwiceShouldReturnTheSameResult()
        {
            var firstResult = _interceptedObject.DoSomethingElse();
            var secondResult = _interceptedObject.DoSomethingElse();

            Assert.AreEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void CallInterceptedMethodAgainAfterCacheExpirationShouldReturnDifferentResult()
        {
            var firstResult = _interceptedObject.DoSomethingElse();
            Thread.Sleep(11);
            var secondResult = _interceptedObject.DoSomethingElse();

            Assert.AreNotEqual(firstResult, secondResult);
        }

        [TestMethod]
        public void CallInterceptedMethodFromMultipleTasksShouldReturnTheSameResult()
        {
            var tasks = CreateTasksOfInterceptedMethods();

            Task.WaitAll(tasks);

            for (int i = 1; i < tasks.Length; i++)
                Assert.AreEqual(tasks[0].Result, tasks[i].Result);
        }

        [TestMethod]
        public void CallInterceptedMethodWithDifferentArgumentsShouldReturnDifferentResults()
        {
            var firstResult = _interceptedObject.DoSomethingElse("a", 0);
            var secondResult = _interceptedObject.DoSomethingElse("b", 0);

            Assert.AreNotEqual(firstResult, secondResult);
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
                new Type[] { typeof(IDoThings) },
                _interceptor
            );
        }

        private Task<string>[] CreateTasksOfInterceptedMethods()
        {
            return new Task<string>[]
            {
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse()),
                Task.Run(() => _interceptedObject.DoSomethingElse())
            };
        }
    }
}
