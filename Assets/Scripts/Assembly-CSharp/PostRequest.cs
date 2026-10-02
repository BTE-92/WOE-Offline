using System.Collections;
using System.Text;
using UnityEngine;

public class PostRequest : WWWRequest
{
	public PostRequest(string _url, string _tag = "", bool _retryAfterFail = false, float _retryDelaySecs = 5f, bool _destroyAfterDone = true)
	{
		m_url = _url;
		m_destroyAfterDone = _destroyAfterDone;
		m_tag = _tag;
		m_WWW = WWWResult.Post(m_url, new byte[1], null);
		startRequest(_retryAfterFail, _retryDelaySecs);
	}

	public PostRequest(string _url, string _JSON, Hashtable _postHeader = null, string _tag = "", bool _retryAfterFail = false, float _retryDelaySecs = 5f, bool _destroyAfterDone = true)
	{
		m_url = _url;
		m_destroyAfterDone = _destroyAfterDone;
		m_tag = _tag;
		if (_postHeader == null)
		{
			_postHeader = defaultHeaders("application/json");
		}
		m_WWW = WWWResult.Post(m_url, Encoding.UTF8.GetBytes(_JSON), _postHeader);
		startRequest(_retryAfterFail, _retryDelaySecs);
	}

	public PostRequest(string _url, byte[] _postData, Hashtable _postHeader = null, string _tag = "", bool _retryAfterFail = false, float _retryDelaySecs = 5f, bool _destroyAfterDone = true)
	{
		m_url = _url;
		m_destroyAfterDone = _destroyAfterDone;
		m_tag = _tag;
		if (_postHeader == null)
		{
			_postHeader = defaultHeaders("application/octet-stream");
		}
		m_WWW = WWWResult.Post(m_url, _postData, _postHeader);
		startRequest(_retryAfterFail, _retryDelaySecs);
	}

	private Hashtable defaultHeaders(string _contentType)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Content-Type", _contentType);
		return hashtable;
	}
}
