using System;
using System.Collections.Generic;
using UnityEngine;

public static class HighScores
{
	public static string SendHighscore(int _score, int _starts, string _gameId, bool _returnScores = false, Action<WWWRequest> _successEventHandler = null, Action<WWWRequest> _failEventHandler = null)
	{
		if (_gameId != null)
		{
			if (UserLogin.m_userLoginState == UserLoginState.LOGGED_IN)
			{
				KeyValuePair keyValuePair = ClientTools.GenerateKeyValuePair(_score.ToString(), new string[3]
				{
					_gameId,
					PlayerPrefsX.GetUserId(),
					_score.ToString()
				});
				string text = IpConfig.ServerUrl + "/v1/highscore/send";
				text = text + "?gameId=" + _gameId;
				text = text + "&playerId=" + PlayerPrefsX.GetUserId();
				text = text + "&starts=" + _starts;
				text = text + "&time=" + keyValuePair.value;
				text = text + "&hash=" + keyValuePair.key;
				text = text + "&name=" + WWW.EscapeURL(PlayerPrefsX.GetUserName());
				string text2 = ((!_returnScores) ? "false" : "true");
				text = text + "&returnScores=" + text2;
				PostRequestQueue.AddToQueue(text, _successEventHandler, _failEventHandler);
				return text;
			}
			Debug.LogError("User not logged in!");
		}
		else
		{
			Debug.Log("No level id. Won't send score");
		}
		return null;
	}

	public static string TicksToTime(int _ticks)
	{
		float seconds = (float)_ticks / 60f;
		return ToolBox.getTimeStringFromSeconds(seconds);
	}

	public static string ScoreToTime(int _score)
	{
		float seconds = (float)_score / 1000f / 60f;
		return ToolBox.getTimeStringFromSeconds(seconds);
	}

	public static int TicksToScore(int _ticks, bool _generateRandomFractions)
	{
		UnityEngine.Random.seed = (int)(Time.realtimeSinceStartup * 10f);
		return _ticks * 1000 + (_generateRandomFractions ? UnityEngine.Random.Range(0, 900) : 0);
	}

	public static HighscoreData[] ParseHighscoreJSON(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		HighscoreData[] array = new HighscoreData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			array[i].name = (string)dictionary["n"];
			array[i].score = (int)(long)dictionary["t"];
		}
		return array;
	}
}
