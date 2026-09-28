using System;
using UnityEngine;

public abstract class WWWRequest
{
	public WWWRequestThread m_thread;

	public WWW m_WWW;

	public string m_url;

	public string m_tag;

	public bool m_destroyAfterDone;

	private GameObject m_gameObject;

	public event Action<WWWRequest> requestComplete;

	public event Action<WWWRequest> requestFailed;

	protected void startRequest(bool _retryAfterFail, float _retryDelaySecs)
	{
		m_gameObject = new GameObject("Download: " + m_url);
		m_thread = m_gameObject.AddComponent<WWWRequestThread>() as WWWRequestThread;
		m_thread.init(this, _retryAfterFail, _retryDelaySecs);
		Debug.Log("Starting WWWRequest: " + m_url);
	}

	public void Destroy()
	{
		if (m_gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(m_gameObject);
		}
		m_WWW.Dispose();
		m_WWW = null;
		requestComplete = null;
		requestFailed = null;
		m_gameObject = null;
		m_thread = null;
	}

	public void DComplete()
	{
		if (requestComplete != null)
		{
			requestComplete(this);
		}
		if (m_destroyAfterDone)
		{
			Destroy();
		}
	}

	public void DFailed()
	{
		if (requestFailed != null)
		{
			requestFailed(this);
		}
		if (m_destroyAfterDone && !m_thread.m_retryWhenFails)
		{
			Destroy();
		}
	}
}
