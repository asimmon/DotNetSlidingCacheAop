using System;
using SlidingCacheAop.WinApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlidingCacheAop.Tests
{
    [TestClass]
    public class CacheResultAttributeTests
    {
        private static readonly Random _random = new Random();
        private int _duration;

        [TestInitialize]
        public void SetUp()
        {
            _duration = _random.Next(10, 30);
        }

        [TestMethod]
        public void TestCacheAttributeMilliseconds()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Milliseconds);
            Assert.AreEqual(TimeSpan.FromMilliseconds(_duration), cacheAttr.Duration);
        }

        [TestMethod]
        public void TestCacheAttributeSeconds()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Seconds);
            Assert.AreEqual(TimeSpan.FromSeconds(_duration), cacheAttr.Duration);
        }

        [TestMethod]
        public void TestCacheAttributeMinutes()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Minutes);
            Assert.AreEqual(TimeSpan.FromMinutes(_duration), cacheAttr.Duration);
        }

        [TestMethod]
        public void TestCacheAttributeHours()
        {
            var cacheAttr = new CacheResultAttribute(_duration, CacheTimeUnit.Hours);
            Assert.AreEqual(TimeSpan.FromHours(_duration), cacheAttr.Duration);
        }
    }
}
