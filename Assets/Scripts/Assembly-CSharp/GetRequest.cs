using UnityEngine;

public class GetRequest : WWWRequest
{
	public GetRequest(string _url, string _tag = "", bool _retryAfterFail = false, float _retryDelaySecs = 5f, bool _destroyAfterDone = true)
	{
		m_url = _url;
		m_tag = _tag;
		m_destroyAfterDone = _destroyAfterDone;
		m_WWW = WWWResult.Get(m_url);
		startRequest(_retryAfterFail, _retryDelaySecs);
	}
}
