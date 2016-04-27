using System;
using System.Runtime.Caching;
using System.Threading;
using Memocache = Meconon.Memocache.Memocache;

namespace Memocached.Sample
{
	/// <summary>
	/// A simple class demonstrating an internally memoised function.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public class SampleClass : IDisposable
	{
		public SampleClass()
		{
			_cachedimplementation = _cache.Cache<long, int, int>((x, y) => InternalImplementation(x, y));
		}

		~SampleClass()
		{
			Dispose(false);
		}

		// We create the cache as a private field
		private readonly Memocache _cache = new Memocache("ComplexCalculationCache", defaultPolicy: new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(15) });

		// The memoized accessor to the function.
		private readonly Func<int, int, long> _cachedimplementation;

		/// <summary>
		/// The actual implementation of the lengthy calculation that we want to cach.
		/// </summary>
		/// <param name="x">The x value</param>
		/// <param name="y">The y value</param>
		/// <returns>A <see cref="long"/> containing the calculated answer</returns>
		/// <remarks>We keep the implementation private as we don't actually want callers to call it directly.
		/// We will expose a wrapper method that actually calls the cached accessor to this method.</remarks>
		private long InternalImplementation(int x, int y)
		{
			// Sleep to simulate lengthy calculation/data retrieval.
			Thread.Sleep(1000);
			return x * (y ^ 5);
		}

		/// <summary>
		/// The public wrapper for the lengthy calculation we will cache
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>A <see cref="long"/> containing the calculated/cached  answer</returns>
		public long ComplexCalculation(int x, int y)
		{
			return _cachedimplementation(x, y);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool releaseManagedResources)
		{
			if (releaseManagedResources)
			{
				_cache.Dispose();
			}
		}
	}
}