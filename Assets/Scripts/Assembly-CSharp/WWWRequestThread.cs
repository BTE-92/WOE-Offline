using UnityEngine;

public class WWWRequestThread : MonoBehaviour
{
	public bool m_retryWhenFails;

	public WWWRequest m_downloader;

	private bool m_fetchingData;

	private bool m_offline;

	private float m_retryDelaySecs;

	private float m_fetchStartTime;

	public void init(WWWRequest _downloader, bool _retryWhenFails = false, float _retryDelaySecs = 5f)
	{
		m_downloader = _downloader;
		m_fetchingData = true;
		m_retryWhenFails = _retryWhenFails;
		m_retryDelaySecs = _retryDelaySecs;
		m_fetchStartTime = Time.realtimeSinceStartup;
		m_offline = false;
	}

	public void Update()
	{
		if (m_fetchingData)
		{
			if (m_downloader.m_WWW != null && m_downloader.m_WWW.isDone)
			{
				if (m_downloader.m_WWW.error != null)
				{
					Debug.LogError("WWWRequest error " + m_downloader.m_WWW.error + " from " + m_downloader.m_url);
					m_downloader.DFailed();
					m_offline = true;
				}
				else
				{
					Debug.Log("WWWRequest complete from " + m_downloader.m_url);
					m_downloader.DComplete();
					m_offline = false;
				}
				m_fetchingData = false;
			}
		}
		else if (m_retryWhenFails && m_offline && Time.realtimeSinceStartup >= m_fetchStartTime + m_retryDelaySecs)
		{
			m_downloader.m_WWW = WWWResult.Get(m_downloader.m_url);
			m_fetchingData = true;
			m_fetchStartTime = Time.realtimeSinceStartup;
			Debug.Log("WWWRequest retry " + m_downloader.m_url);
		}
	}
}
