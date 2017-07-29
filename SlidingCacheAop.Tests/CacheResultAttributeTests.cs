using System;
using Xunit;

namespace SlidingCacheAop.Tests
{
    public class CacheResultAttributeTests
    {
        private static readonly Random NumberGenerator = new Random();
        private readonly int _duration;

        public CacheResultAttributeTests()
        {
            _duration = NumberGenerator.Next(10, 30);
        }

        [Fact]
        public void TestCacheAttributeMilliseconds()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Milliseconds);
            Assert.Equal(TimeSpan.FromMilliseconds(_duration), cacheAttr.Duration);
        }

        [Fact]
        public void TestCacheAttributeSeconds()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Seconds);
            Assert.Equal(TimeSpan.FromSeconds(_duration), cacheAttr.Duration);
        }

        [Fact]
        public void TestCacheAttributeMinutes()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Minutes);
            Assert.Equal(TimeSpan.FromMinutes(_duration), cacheAttr.Duration);
        }

        [Fact]
        public void TestCacheAttributeHours()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Hours);
            Assert.Equal(TimeSpan.FromHours(_duration), cacheAttr.Duration);
        }
    }
}
