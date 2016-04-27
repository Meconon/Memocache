using System;
using System.Diagnostics.Contracts;
using System.Runtime.Caching;

namespace Meconon.Memocache
{
	public class CacheImplementation : ICache
	{
		private readonly MemoryCache _cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheImplementation"/> class.
		/// </summary>
		/// <param name="name">The optional cache namename.</param>
		public CacheImplementation(string name = null)
		{
			Contract.Requires(String.Equals(name, "default", StringComparison.OrdinalIgnoreCase) == false, "Do not use the name 'default' as that is the default name");
			_cache = string.IsNullOrEmpty(name) == false ? new MemoryCache(name) : MemoryCache.Default;

			DefaultPolicy = new CacheItemPolicy();
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="CacheImplementation"/> class.
		/// </summary>
		~CacheImplementation()
		{
			Dispose(false);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_cache != null, "Must have an iCache instance.");
		}

		/// <summary>
		/// Gets or sets the default policy.
		/// </summary>
		/// <value>
		/// The default policy.
		/// </value>
		public CacheItemPolicy DefaultPolicy { get; set; }

		/// <summary>
		/// Retrieves the cached result for the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The cached object, or null if none exists.</returns>
		public T? Retrieve<T>(string key) where T : struct
		{
			if (_cache.Contains(key))
			{
				object o = _cache[key];

				return (T?)o;
			}

			return null;
		}

		/// <summary>
		/// Adds the specified value with the specified key and cache item policy.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="policy">The policy.</param>
		public void Add<T>(string key, T value, CacheItemPolicy policy)
		{
			if (_cache.Contains(key))
			{
				_cache[key] = value;
				return;
			}
			// end of potentially redundant

			_cache.Add(key, value, policy);
		}

		/// <summary>
		/// Adds the specified value with the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add<T>(string key, T value)
		{
			Add(key, value, DefaultPolicy);
		}

		/// <summary>
		/// Gets the name assigned to the cache.
		/// </summary>
		/// <value>
		/// The name of the cache.
		/// </value>
		public string Name
		{
			get { return _cache.Name; }
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