using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace SlidingCacheAop.WinApp
{
	public class CachedMethodInvocation
	{
		private IInvocation _innerInvocation;
		private CacheResultAttribute _cacheAttribute;

		public object ReturnValue
		{
			get { return _innerInvocation.ReturnValue; }
			set { _innerInvocation.ReturnValue = value; }
		}

		public string Signature { get; private set; }

		public TimeSpan CacheDuration
		{
            get { return _cacheAttribute.Duration; }
		}

		public CachedMethodInvocation(IInvocation invocation)
		{
			_innerInvocation = invocation;
			ComputeSignatureIfCacheAttributeExistOnMethod();
		}

		public void Proceed()
		{
			_innerInvocation.Proceed();
		}

		private void ComputeSignatureIfCacheAttributeExistOnMethod()
		{
			_cacheAttribute = GetCacheAttributeFromMethod();
			if (IsCacheEnabled())
				ComputeSignature();
		}

		private CacheResultAttribute GetCacheAttributeFromMethod()
		{
			var targetMethod = _innerInvocation.MethodInvocationTarget;
			var cacheAttribute = targetMethod.GetCustomAttribute<CacheResultAttribute>();

			return cacheAttribute;
		}

		public bool IsCacheEnabled()
		{
            return _cacheAttribute != null && _cacheAttribute.Duration.Ticks > 0;
		}

		private void ComputeSignature()
		{
            // TODO It is up to you to implement a more complex logic to generate a signature based on the arguments
            // Some complex argument cases: datetimes, enumerables, "should-not-be-serializable" objects (ex:Stream?)
			Signature = string.Format("{0}-{1}-{2}",
				_innerInvocation.TargetType.FullName,
				_innerInvocation.Method.Name,
				string.Join("-", _innerInvocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())
			);
		}
	}
}
