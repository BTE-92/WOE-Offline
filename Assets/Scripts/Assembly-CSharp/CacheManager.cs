using System.Collections.Generic;

public static class CacheManager
{
	private static List<ICache> m_caches = new List<ICache>();

	private static float m_nextUpdate;

	private static float m_updateInterval = 1f;

	public static ICache AddCache(ICache _cache)
	{
		m_caches.Add(_cache);
		return _cache;
	}

	public static ICache GetCache(string _key)
	{
		for (int num = m_caches.Count - 1; num > -1; num--)
		{
			if (m_caches[num].m_key == _key)
			{
				return m_caches[num];
			}
		}
		return null;
	}

	public static void RemoveCache(string _key)
	{
		for (int num = m_caches.Count - 1; num > -1; num--)
		{
			if (m_caches[num].m_key == _key)
			{
				m_caches.RemoveAt(num);
				break;
			}
		}
	}

	public static bool IsCacheExpired(string _key)
	{
		ICache cache = GetCache(_key);
		if (cache != null)
		{
			return cache.IsExpired();
		}
		return true;
	}

	public static void RemoveAll()
	{
		while (m_caches.Count > 0)
		{
			m_caches.RemoveAt(m_caches.Count - 1);
		}
	}

	public static void Update()
	{
		if (!(m_nextUpdate < Main.m_gameTime))
		{
			return;
		}
		for (int num = m_caches.Count - 1; num > -1; num--)
		{
			if (m_caches[num].IsExpired())
			{
				m_caches.RemoveAt(num);
			}
		}
		m_nextUpdate += m_updateInterval;
	}
}
