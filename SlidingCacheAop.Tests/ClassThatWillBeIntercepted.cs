using System;
using SlidingCacheAop.WinApp;

namespace SlidingCacheAop.Tests
{
    public interface IDoThings
    {
        string DoSomething();
        string DoSomethingElse();
        string DoSomethingElse(string arg1, int arg2);
        string DoSomethingElse(object arg);
    }

    public class ClassThatWillBeIntercepted : IDoThings
    {
        public string DoSomething()
        {
            return Guid.NewGuid().ToString("N");
        }

        [CacheResult(10, CacheTimeUnit.Milliseconds)]
        public string DoSomethingElse() => DoSomething();

        [CacheResult(10, CacheTimeUnit.Milliseconds)]
        public string DoSomethingElse(string arg1, int arg2) => DoSomething();

        [CacheResult(10, CacheTimeUnit.Milliseconds)]
        public string DoSomethingElse(object arg) => DoSomething();
    }
}
