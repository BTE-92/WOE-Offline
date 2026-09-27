public interface ICache
{
	string m_key { get; set; }

	float m_created { get; set; }

	float m_updated { get; set; }

	float m_expires { get; set; }

	float m_expireDuration { get; set; }

	int GetCacheLength();

	bool IsExpired();

	void Clear();
}
