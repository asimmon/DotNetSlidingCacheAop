using System;
using System.IO;
using System.Text;
using Castle.DynamicProxy;
using Xunit;

namespace SlidingCacheAop.Tests
{
    public class InvocationCatcherInterceptor : IInterceptor
    {
        public Action<IInvocation> ActionAgainstInvocation { get; set; }

        public void Intercept(IInvocation invocation)
        {
            if (ActionAgainstInvocation != null)
                ActionAgainstInvocation(invocation);
        }
    }

    public class CachedMethodInvocationTests
    {
        private InvocationCatcherInterceptor _interceptor;
        private IDoThings _interceptedObject;
        private string _expectedSignature;

        public CachedMethodInvocationTests()
        {
            CreateInterceptor();
            CreateInterceptedObject();
        }

        [Fact]
        public void TestCachedMethodInvocationSignatureWithoutArgs()
        {
            _expectedSignature = "SlidingCacheAop.Tests.ClassThatWillBeIntercepted-DoSomethingElse-";
            _interceptedObject.DoSomethingElse();
        }

        [Fact]
        public void TestCachedMethodInvocationSignatureWithSimpleArgs()
        {
            _expectedSignature = "SlidingCacheAop.Tests.ClassThatWillBeIntercepted-DoSomethingElse-a-1";
            _interceptedObject.DoSomethingElse("a", 1);
        }

        [Fact]
        public void TestCachedMethodInvocationSignatureWithSerializableArg()
        {
            var sb = new StringBuilder("foo");

            _expectedSignature = "SlidingCacheAop.Tests.ClassThatWillBeIntercepted-DoSomethingElse-foo";
            _interceptedObject.DoSomethingElse(sb);
        }

        [Fact]
        public void TestCachedMethodInvocationSignatureWithTooComplexArg()
        {
            // the test pass but the logic is biased due to the nature of the complex argument
            // which is not ToString-able as this implementation of the signature computation
            // requires it
            var ms = new MemoryStream();

            _expectedSignature = "SlidingCacheAop.Tests.ClassThatWillBeIntercepted-DoSomethingElse-System.IO.MemoryStream";
            _interceptedObject.DoSomethingElse(ms);
        }

        private void CreateInterceptor()
        {
            _interceptor = new InvocationCatcherInterceptor();
        }

        private void CreateInterceptedObject()
        {
            var proxyGenerator = new ProxyGenerator();
            _interceptedObject = (IDoThings)proxyGenerator.CreateClassProxy(
                typeof(ClassThatWillBeIntercepted),
                new[] { typeof(IDoThings) },
                _interceptor
            );

            _interceptor.ActionAgainstInvocation = (invocation) =>
            {
                var cachedInvocation = new CachedMethodInvocation(invocation);
                Assert.Equal(_expectedSignature, cachedInvocation.Signature);
            };
        }
    }
}
