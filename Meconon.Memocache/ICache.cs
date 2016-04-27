using System;
using System.Runtime.Caching;

namespace Meconon.Memocache
{
	public interface ICache : IDisposable
	{
		/// <summary>
		/// Gets or sets the default policy.
		/// </summary>
		/// <value>
		/// The default policy.
		/// </value>
		CacheItemPolicy DefaultPolicy { get; set; }

		/// <summary>
		/// Retrieves the cached result for the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The cached object, or null if none exists.</returns>
		T? Retrieve<T>(string key) where T : struct;

		/// <summary>
		/// Adds the specified value with the specified key and cache item policy.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="policy">The policy.</param>
		void Add<T>(string key, T value, CacheItemPolicy policy);

		/// <summary>
		/// Adds the specified value with the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Add<T>(string key, T value);

		/// <summary>
		/// Gets the name assigned to the cache.
		/// </summary>
		/// <value>
		/// The name of the cache.
		/// </value>
		string Name { get; }
	}
}