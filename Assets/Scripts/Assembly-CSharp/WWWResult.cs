using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class WWWResult
{
	private readonly WWW m_www;
	private readonly byte[] m_bytes;
	private readonly string m_error;
	private readonly float m_readyTime;

	private static OfflineBackend s_backend;
	private static string s_backendError;

	private WWWResult(WWW www)
	{
		m_www = www;
	}

	private WWWResult(OfflineResponse response)
	{
		m_bytes = response.Data;
		m_error = response.Error;
		m_readyTime = Time.realtimeSinceStartup + response.DelaySeconds;
	}

	public static WWWResult Get(string url)
	{
		if (IpConfig.IsOfflineUrl(url))
		{
			return Local("GET", url, null, null);
		}
		return new WWWResult(new WWW(url));
	}

	public static WWWResult Post(string url, byte[] data, Hashtable headers)
	{
		if (IpConfig.IsOfflineUrl(url))
		{
			string fileSizes = null;
			if (headers != null && headers.ContainsKey("FILE_SIZES"))
			{
				fileSizes = headers["FILE_SIZES"] as string;
			}
			return Local("POST", url, data, fileSizes);
		}
		if (headers == null)
		{
			return new WWWResult(new WWW(url, data));
		}
		return new WWWResult(new WWW(url, data, headers));
	}

	private static WWWResult Local(string method, string url, byte[] body, string fileSizes)
	{
		if (s_backend == null && s_backendError == null)
		{
			try
			{
				s_backend = new OfflineBackend(Application.persistentDataPath + "/OfflineServer");
				Debug.Log("Offline backend ready: " + s_backend.RootPath);
			}
			catch (Exception e)
			{
				s_backendError = e.Message;
				Debug.LogError("Offline backend failed to start: " + e);
			}
		}

		OfflineResponse response;
		if (s_backend != null)
		{
			response = s_backend.Handle(method, url, body, fileSizes);
		}
		else
		{
			response = new OfflineResponse(new byte[0]);
			response.Error = "Offline backend unavailable: " + s_backendError;
		}
		return new WWWResult(response);
	}

	public bool isDone
	{
		get { return m_www != null ? m_www.isDone : Time.realtimeSinceStartup >= m_readyTime; }
	}

	public string error
	{
		get { return m_www != null ? m_www.error : m_error; }
	}

	public byte[] bytes
	{
		get { return m_www != null ? m_www.bytes : m_bytes; }
	}

	public string text
	{
		get { return m_www != null ? m_www.text : Encoding.UTF8.GetString(m_bytes); }
	}

	public AssetBundle assetBundle
	{
		get { return m_www != null ? m_www.assetBundle : null; }
	}

	public Texture2D texture
	{
		get { return m_www != null ? m_www.texture : null; }
	}

	public void Dispose()
	{
		if (m_www != null)
		{
			m_www.Dispose();
		}
	}
}
