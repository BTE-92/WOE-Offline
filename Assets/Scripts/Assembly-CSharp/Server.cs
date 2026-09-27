using System;
using System.Collections.Generic;
using UnityEngine;

public static class Server
{
	private class Parser<T>
	{
		private Action<T> m_okCallback;

		private Func<WWWRequest, T> m_parser;

		public Parser(Action<T> _okCallback, Func<WWWRequest, T> _parser)
		{
			m_okCallback = _okCallback;
			m_parser = _parser;
		}

		public void RequestOk(WWWRequest req)
		{
			m_okCallback(m_parser(req));
		}
	}

	public static void DeleteMiniGame(string _gameId, Action _okCallback = null, Action<WWWRequest> _failureCallback = null)
	{
		Debug.Log("Delete minigame " + _gameId);
		string text = IpConfig.ServerUrl + "/v1/minigame/delete";
		text = text + "?id=" + _gameId;
		int second = DateTime.Now.Second;
		text = text + "&time=" + second;
		text = text + "&hash=" + ClientTools.GenerateHash(_gameId + second);
		new PostRequest(text, string.Empty, false, 5f);
	}

	public static void SaveLike(string _gameId)
	{
		string url = IpConfig.ServerUrl + "/v1/minigame/like/save?gameId=" + _gameId + "&playerId=" + PlayerPrefsX.GetUserId();
		new PostRequest(url, string.Empty, false, 5f);
	}

	public static void GetLikes(Action<List<string>> _okCallback)
	{
		Parser<List<string>> parser = new Parser<List<string>>(_okCallback, ClientTools.ParseLikes);
		string url = IpConfig.ServerUrl + "/v1/minigame/like/find?playerId=" + PlayerPrefsX.GetUserId();
		WWWRequest wWWRequest = new GetRequest(url, string.Empty, false, 5f);
		wWWRequest.requestComplete += parser.RequestOk;
	}

	public static void SaveScreenshot(string gameId, byte[] data, Action<WWWRequest> _okCallback, Action<WWWRequest> _failureCallback = null)
	{
		WWWRequest wWWRequest = new PostRequest(IpConfig.ServerUrl + "/v1/minigame/screenshot/save?gameId=" + gameId, data, null, string.Empty, false, 5f);
		wWWRequest.requestComplete += _okCallback;
		wWWRequest.requestFailed += _failureCallback;
	}

	public static void GetScreenshot(string gameId, Action<byte[]> _okCallback, Action<WWWRequest> _failureCallback)
	{
		Parser<byte[]> parser = new Parser<byte[]>(_okCallback, ClientTools.ParseByteArray);
		WWWRequest wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/minigame/screenshot/find?gameId=" + gameId, string.Empty, false, 5f);
		wWWRequest.requestComplete += parser.RequestOk;
		wWWRequest.requestFailed += _failureCallback;
	}

	public static void SaveComment(string gameId, string comment, Action<WWWRequest> _okCallback, Action<WWWRequest> _failureCallback = null)
	{
		string text = IpConfig.ServerUrl + "/v1/minigame/comment/save";
		text = text + "?gameId=" + gameId;
		text = text + "&playerId=" + PlayerPrefsX.GetUserId();
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			text = text + "&facebookId=" + PlayerPrefsX.GetFacebookId();
		}
		if (PlayerPrefsX.GetGameCenterId() != null)
		{
			text = text + "&gameCenterId=" + PlayerPrefsX.GetGameCenterId();
		}
		text = text + "&comment=" + WWW.EscapeURL(comment);
		text = text + "&hash=" + ClientTools.GenerateHash(gameId + PlayerPrefsX.GetUserId() + comment);
		WWWRequest wWWRequest = new PostRequest(text, string.Empty, false, 5f);
		wWWRequest.requestComplete += _okCallback;
		wWWRequest.requestFailed += _failureCallback;
	}

	public static void GetComments(string gameId, Action<CommentData[]> _okCallback, Action<WWWRequest> _failureCallback, int limit = 0)
	{
		Parser<CommentData[]> parser = new Parser<CommentData[]>(_okCallback, ClientTools.ParseComments);
		string text = IpConfig.ServerUrl + "/v1/minigame/comment/find?gameId=" + gameId;
		if (limit != 0)
		{
			text = text + "&limit=" + limit;
		}
		WWWRequest wWWRequest = new GetRequest(text, string.Empty, false, 5f);
		wWWRequest.requestComplete += parser.RequestOk;
		wWWRequest.requestFailed += _failureCallback;
	}

	public static void SendQuitData(int _starts, string _gameId)
	{
		string text = IpConfig.ServerUrl + "/v1/highscore/quit";
		text = text + "?gameId" + _gameId;
		text = text + "&playerId=" + PlayerPrefsX.GetUserId();
		text = text + "&starts=" + _starts;
		text = text + "&hash=" + ClientTools.GenerateHash(_gameId + PlayerPrefsX.GetUserId() + _starts);
		new PostRequest(text, string.Empty, false, 5f);
	}
}
