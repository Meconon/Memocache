using FakeItEasy;
using Meconon.Memocache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.Caching;

namespace Meconon.MemocacheTests
{
	[TestClass()]
	public class MemocacheTests
	{
		[TestMethod]
		public void CacheNamePersists()
		{
			var expectedName = "TestName";
			using (Memocache.Memocache sut = new Memocache.Memocache(expectedName, new CacheItemPolicy()))
			{
				Assert.AreEqual(expectedName, sut.Name);
			}
		}

		[TestMethod]
		public void CacheInjectionWorks()
		{
			var cache = A.Fake<ICache>();

			A.CallTo(() => cache.Add(string.Empty, 0)).WithAnyArguments().DoesNothing();

			Func<int, int, int> m;
			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", new CacheItemPolicy(), cache))
			{
				m = sut.Cache<int, int, int>(Multiply);
				var answer = m(2, 2);

				A.CallTo(() => cache.Add(string.Empty, 0)).WithAnyArguments().MustHaveHappened();
			}
		}

		[TestMethod]
		public void DefaultPolicyPersistsToCache()
		{
			var cache = A.Fake<ICache>();

			var cacheItemPolicy = new CacheItemPolicy()
			{
				SlidingExpiration = TimeSpan.FromMilliseconds(10)
			};

			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", cacheItemPolicy, cache))
			{
				Assert.AreEqual(TimeSpan.FromMilliseconds(10), cache.DefaultPolicy.SlidingExpiration);
			}
		}

		[TestMethod]
		public void CacheAllowsMultipleCachingOfSame()
		{
			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", new CacheItemPolicy()))
			{
				var m = sut.Cache<int, int, int>(Multiply);
				m = sut.Cache<int, int, int>(Multiply);

				var answer = m(2, 2);

				var secondCall = m(2, 2);

				var thirdcall = m(3, 3);

				Assert.AreEqual(4, answer);
				Assert.AreEqual(answer, secondCall);

				Assert.AreEqual(9, thirdcall);
			}
		}

		[TestMethod]
		public void SimpleCacheTest1ArgumentFunc()
		{
			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", new CacheItemPolicy()))
			{
				var m = sut.Cache<int, int>(Square);

				var answer = m(2);

				var secondCall = m(2);

				var thirdcall = m(3);

				Assert.AreEqual(4, answer);
				Assert.AreEqual(answer, secondCall);

				Assert.AreEqual(9, thirdcall);
			}
		}

		[TestMethod]
		public void SimpleCacheTest2ArgumentFunc()
		{
			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", new CacheItemPolicy()))
			{
				var m = sut.Cache<int, int, int>(Multiply);

				var answer = m(2, 2);

				var secondCall = m(2, 2);

				var thirdcall = m(3, 3);

				Assert.AreEqual(4, answer);
				Assert.AreEqual(answer, secondCall);

				Assert.AreEqual(9, thirdcall);
			}
		}

		[TestMethod]
		public void SimpleCacheTest3ArgumentFunc()
		{
			using (Memocache.Memocache sut = new Memocache.Memocache("cacheName", new CacheItemPolicy()))
			{
				var m = sut.Cache<int, int, int, int>(Multiply3);

				var answer = m(2, 2, 2);

				var secondCall = m(2, 2, 2);

				var thirdcall = m(3, 3, 3);

				Assert.AreEqual(8, answer);
				Assert.AreEqual(answer, secondCall);

				Assert.AreEqual(27, thirdcall);
			}
		}

		public int Square(int x)
		{
			return x * x;
		}

		public int Multiply(int x, int y)
		{
			return x * y;
		}

		public int Multiply3(int x, int y, int z)
		{
			return x * y * z;
		}
	}
}