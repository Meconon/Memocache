using System;
using System.Reflection;

namespace Meconon.Memocache.Logging
{
	public interface IMemocacheLog : IDisposable
	{
		/// <summary>
		/// Logs creation of the memo cache with a given name.
		/// </summary>
		/// <param name="name">The name.</param>
		void CreatingMemoCache(string name);

		/// <summary>
		/// Logs creation of the memo cache
		/// </summary>
		void CreatedMemoCache();

		/// <summary>
		/// Logs the caching of a function.
		/// </summary>
		/// <param name="name">The name.</param>
		void Caching(string name);

		/// <summary>
		/// Logs the attempted caching of an illegal declaring type.
		/// </summary>
		/// <param name="methodInfo">The method information.</param>
		void CachingDeclaringTypeIllegal(MethodInfo methodInfo);

		/// <summary>
		/// Logs the cache's first hit when checking for a cached value
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void CachingFirstHit(string key, object value);

		/// <summary>
		/// Logs the start of an addition to the chache
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="result">The result.</param>
		void AddingToCache(string key, ValueType result);

		/// <summary>
		/// Logs the successful addition to the cache
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="valueType">Type of the value.</param>
		void AddedToCache(string key, ValueType valueType);

		/// <summary>
		/// Logs a successful read from the cache
		/// </summary>
		/// <param name="key">The key.</param>
		void CacheReadSuccessful(string key);
	}
}