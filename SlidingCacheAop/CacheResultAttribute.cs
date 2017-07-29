using System;

namespace SlidingCacheAop.WinApp
{
    public class CacheResultAttribute : Attribute
    {
        private readonly int _duration;
        private readonly CacheTimeUnit _timeUnit;

        public CacheResultAttribute(int duration, CacheTimeUnit timeUnit)
        {
            _duration = duration;
            _timeUnit = timeUnit;
        }

        public TimeSpan Duration
        {
            get
            {
                var timeUnitInMilliseconds = (int)_timeUnit;
                var durationInMilliseconds = _duration * timeUnitInMilliseconds;
                return TimeSpan.FromMilliseconds(durationInMilliseconds);
            }
        }
    }
}
