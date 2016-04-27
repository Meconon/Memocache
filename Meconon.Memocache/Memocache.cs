using Meconon.Memocache.Logging;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Caching;

namespace Meconon.Memocache
{
	/// <summary>
	/// Memocache -- a memoizer using extended caching policies
	/// </summary>
	public class Memocache : IDisposable
	{
		private readonly ICache _cache;
		private readonly IMemocacheLog _log;

		/// <summary>
		/// The name of the cache (optional)
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// IoC Constructor - initializes a new instance of the <see cref="Memocache"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="logger">The logger.</param>
		public Memocache(ICache cache, IMemocacheLog logger)
		{
			Contract.Requires(cache != null, "An ICache instance is required.");
			Contract.Requires(logger != null, "An IMemocacheLog instance is required.");

			_log = logger;

			_log.CreatingMemoCache(cache.Name);

			_cache = cache;

			_log.CreatedMemoCache();
		}

		/// <summary>
		/// Creates a new memocache instance with optional name and cache policy
		/// </summary>
		/// <param name="name">The name to assign to the memocache. Do <em>not</em> use the name 'default'</param>
		/// <param name="defaultPolicy">The <see cref="CacheItemPolicy"/> to use as default for new entries</param>
		/// <param name="cache">Injected cache implementation if default <see cref="MemoryCache"/> not wanted.</param>
		/// <param name="logger">The <see cref="IMemocacheLog"/> instance to use for logging.</param>
		public Memocache(string name, CacheItemPolicy defaultPolicy, ICache cache = null, IMemocacheLog logger = null)
		{
			Contract.Requires(String.Equals(name, "default", StringComparison.OrdinalIgnoreCase) == false, "Do not use the name 'default' as that is the default name");
			Contract.Requires(name == null || name.Length > 0);

			_log = logger ?? new NullLog();

			_log.CreatingMemoCache(name);

			Name = name;

			_cache = cache ?? new CacheImplementation(Name);

			if (defaultPolicy != null)
			{
				_cache.DefaultPolicy = defaultPolicy;
			}

			_log.CreatedMemoCache();
		}

		~Memocache()
		{
			Dispose(false);
		}

		/// <summary>
		/// Gets or sets the default policy.
		/// </summary>
		/// <value>
		/// The default policy.
		/// </value>
		public CacheItemPolicy DefaultPolicy
		{
			get { return _cache.DefaultPolicy; }
			set { _cache.DefaultPolicy = value; }
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_log != null, "Must have an IMemocacheLog instance.");
			Contract.Invariant(_cache != null, "Must have an ICache instance.");
		}

		/// <summary>
		/// Memoize and cache the given function.
		/// </summary>
		/// <typeparam name="TResult">The return type of the function.</typeparam>
		/// <typeparam name="T1">The type of the first parameter.</typeparam>
		/// <param name="func">The function to memoize</param>
		/// <returns>A memoized function that will use cached results when available.</returns>
		public Func<T1, TResult> Cache<TResult, T1>(Func<T1, TResult> func) where TResult : struct
		{
			Contract.Requires(func != null, "Cannot memoize a NULL function.");

			_log.Caching(func.Method.Name);

			return x1 =>
			{
				if (func.Method.DeclaringType == null)
				{
					_log.CachingDeclaringTypeIllegal(func.Method);

					return default(TResult);
				}

				string key = $"{func.Method.DeclaringType.FullName}_{func.Method.Name}_{x1}";

				TResult? cachedValue = IsInCache<TResult>(key);
				if (cachedValue.HasValue)
				{
					return cachedValue.Value;
				}

				TResult result = func.Invoke(x1);

				AddResult(key, result);

				return result;
			};
		}

		/// <summary>
		/// Memoize and cache the given function.
		/// </summary>
		/// <typeparam name="TResult">The return type of the function.</typeparam>
		/// <typeparam name="T1">The type of the first parameter.</typeparam>
		/// <typeparam name="T2">The type of the second parameter</typeparam>
		/// <param name="func">The function to memoize</param>
		/// <returns>A memoized function that will use cached results when available.</returns>
		public Func<T1, T2, TResult> Cache<TResult, T1, T2>(Func<T1, T2, TResult> func) where TResult : struct
		{
			Contract.Requires(func != null, "Cannot memoize a NULL function.");

			_log.Caching(func.Method.Name);

			return (x1, y1) =>
			{
				if (func.Method.DeclaringType == null)
				{
					_log.CachingDeclaringTypeIllegal(func.Method);

					return default(TResult);
				}

				string key = $"{func.Method.DeclaringType.FullName}_{func.Method.Name}_{x1}_{y1}";
				TResult? cachedValue = IsInCache<TResult>(key);
				if (cachedValue.HasValue)
				{
					return cachedValue.Value;
				}
				TResult result = func.Invoke(x1, y1);

				AddResult(key, result);

				return result;
			};
		}

		/// <summary>
		/// Memoize and cache the given function.
		/// </summary>
		/// <typeparam name="TResult">The return type of the function.</typeparam>
		/// <typeparam name="T1">The type of the first parameter.</typeparam>
		/// <typeparam name="T2">The type of the second parameter</typeparam>
		/// <typeparam name="T3">The type of the thirs parameter</typeparam>
		/// <param name="func">The function to memoize</param>
		/// <returns>A memoized function that will use cached results when available.</returns>
		public Func<T1, T2, T3, TResult> Cache<TResult, T1, T2, T3>(Func<T1, T2, T3, TResult> func) where TResult : struct
		{
			Contract.Requires(func != null, "Cannot memoize a NULL function.");

			_log.Caching(func.Method.Name);

			return (x1, y1, z1) =>
			{
				if (func.Method.DeclaringType == null)
				{
					_log.CachingDeclaringTypeIllegal(func.Method);

					return default(TResult);
				}

				string key = $"{func.Method.DeclaringType.FullName}_{func.Method.Name}_{x1}_{y1}_{z1}";
				TResult? cachedValue = IsInCache<TResult>(key);
				if (cachedValue.HasValue)
				{
					return cachedValue.Value;
				}

				TResult result = func.Invoke(x1, y1, z1);

				AddResult(key, result);

				return result;
			};
		}

		private TResult? IsInCache<TResult>(string key) where TResult : struct
		{
			TResult? cachedResult = _cache.Retrieve<TResult>(key);

			_log.CachingFirstHit(key, cachedResult);

			if (!cachedResult.HasValue)
			{
				return null;
			}

			_log.CacheReadSuccessful(key);

			return cachedResult;
		}

		private void AddResult<TResult>(string key, TResult result) where TResult : struct
		{
			_log.AddingToCache(key, result);

			_cache.Add(key, result);

			_log.AddedToCache(key, result);
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
				_log.Dispose();
			}
		}
	}
}