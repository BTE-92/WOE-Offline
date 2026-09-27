using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

public static class ClientTools
{
	public static string gpw(string s)
	{
		UnityEngine.Random.seed = 97139634;
		char[] array = s.ToCharArray();
		char[] value = ToolBox.shuffleArray(array);
		return new string(value);
	}

	public static KeyValuePair GenerateKeyValuePair(string _value, string[] _keys)
	{
		KeyValuePair result = new KeyValuePair
		{
			value = _value,
			key = string.Empty
		};
		for (int i = 0; i < _keys.Length; i++)
		{
			result.key += _keys[i];
		}
		result.key += gpw(ServerConfig.GetHPW());
		result.key = ToolBox.Md5Sum(result.key);
		return result;
	}

	public static string GenerateHash(string _string)
	{
		string strToEncrypt = _string + gpw(ServerConfig.GetHPW());
		return ToolBox.Md5Sum(strToEncrypt);
	}

	public static Dictionary<string, object> ParseServerResponse(string _response)
	{
		if (!_response.StartsWith("{"))
		{
			Debug.LogError("Server response not valid JSON: " + _response);
			return null;
		}
		return Json.Deserialize(_response) as Dictionary<string, object>;
	}

	public static bool ServerResponseOk(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			Debug.LogError("NULL DICTIONARY!!!");
			return false;
		}
		string text = (string)dict["status"];
		if (text.Equals("OK"))
		{
			return true;
		}
		return false;
	}

	public static byte[] ParseByteArray(WWWRequest _request)
	{
		return _request.m_WWW.bytes;
	}

	public static FBUserData ParseFBUserData(string _FBUserDataString)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_FBUserDataString);
		return new FBUserData
		{
			id = (string)dictionary["id"],
			username = (string)dictionary["username"]
		};
	}

	public static string[] ParseFBFriendIds(string _FBFriendString)
	{
		Debug.Log(_FBFriendString);
		Dictionary<string, object> dictionary = ParseServerResponse(_FBFriendString);
		List<object> list = dictionary["data"] as List<object>;
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
			if (dictionary2.ContainsKey("installed"))
			{
				list2.Add((string)dictionary2["id"]);
			}
		}
		return list2.ToArray();
	}

	public static CommentData[] ParseComments(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_request.m_WWW.text);
		List<object> list = dictionary["data"] as List<object>;
		CommentData[] array = new CommentData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
			array[i].playerId = (string)dictionary2["playerId"];
			array[i].comment = (string)dictionary2["comment"];
			if (dictionary2.ContainsKey("facebookId"))
			{
				array[i].facebookId = (string)dictionary2["facebookId"];
			}
			if (dictionary2.ContainsKey("gameCenterId"))
			{
				array[i].gameCenterId = (string)dictionary2["gameCenterId"];
			}
		}
		return array;
	}

	public static List<string> ParseLikes(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_request.m_WWW.text);
		object[] array = (dictionary["data"] as List<object>).ToArray();
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			list.Add((string)array[i]);
		}
		return list;
	}

	public static FeedData[] ParseGameFeedList(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		FeedData[] array = new FeedData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			string text = (string)dictionary["type"];
			array[i].targetId = (string)dictionary["targetId"];
			array[i].type = text;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["ts"];
			array[i].timestamp = new DateTime((long)dictionary2["$date"]);
			array[i].message = (string)dictionary["message"];
			if (!text.Equals("game"))
			{
				continue;
			}
			if (dictionary.ContainsKey("friendCreator"))
			{
				array[i].friendCreator = (string)dictionary["friendCreator"];
			}
			if (dictionary.ContainsKey("friendPlayers"))
			{
				object[] array2 = (dictionary["friendPlayers"] as List<object>).ToArray();
				array[i].friendPlayers = new string[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array[i].friendPlayers[j] = (string)array2[j];
				}
			}
		}
		return array;
	}

	public static MinigameMetaData[] ParseMinigameList(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		MinigameMetaData[] array = new MinigameMetaData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			array[i].name = (string)dictionary["name"];
			array[i].id = (string)dictionary["id"];
			if (dictionary.ContainsKey("creatorId"))
			{
				array[i].creatorId = (string)dictionary["creatorId"];
			}
			else
			{
				array[i].creatorId = "??";
			}
			if (dictionary.ContainsKey("creatorFacebookId"))
			{
				array[i].creatorFacebookId = (string)dictionary["creatorFacebookId"];
			}
			if (dictionary.ContainsKey("creatorGameCenterId"))
			{
				array[i].creatorGameCenterId = (string)dictionary["creatorGameCenterId"];
			}
			if (dictionary.ContainsKey("creatorName"))
			{
				array[i].creatorName = (string)dictionary["creatorName"];
			}
			else
			{
				array[i].creatorName = "??";
			}
			if (dictionary.ContainsKey("publishTime"))
			{
				array[i].published = true;
			}
			else
			{
				array[i].published = false;
			}
			if (dictionary.ContainsKey("description"))
			{
				array[i].description = (string)dictionary["description"];
			}
			if (dictionary.ContainsKey("timesFinished"))
			{
				array[i].timesFinished = (int)(long)dictionary["timesFinished"];
			}
			if (dictionary.ContainsKey("timesLiked"))
			{
				array[i].timesLiked = (int)(long)dictionary["timesLiked"];
			}
			if (dictionary.ContainsKey("timesPlayed"))
			{
				array[i].timesPlayed = (int)(long)dictionary["timesPlayed"];
			}
			else
			{
				array[i].timesPlayed = 0;
			}
			if (dictionary.ContainsKey("classic"))
			{
				array[i].classicStamp = (bool)dictionary["classic"];
			}
			if (dictionary.ContainsKey("new"))
			{
				array[i].newStamp = (bool)dictionary["new"];
			}
			object[] array2 = (dictionary["tags"] as List<object>).ToArray();
			array[i].tags = new string[array2.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array[i].tags[j] = (string)array2[j];
			}
		}
		return array;
	}

	public static Dictionary<string, PlayerData> ParseFriendDict(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ParseServerResponse(_request.m_WWW.text);
		List<object> list = dictionary["data"] as List<object>;
		Dictionary<string, PlayerData> dictionary2 = new Dictionary<string, PlayerData>();
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary3 = list[i] as Dictionary<string, object>;
			PlayerData value = ParsePlayerData(dictionary3);
			dictionary2.Add(value.playerId, value);
		}
		return dictionary2;
	}

	public static PlayerData[] ParsePlayers(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		PlayerData[] array = new PlayerData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = ParsePlayerData(list[i] as Dictionary<string, object>);
		}
		return array;
	}

	public static PlayerData ParsePlayerData(Dictionary<string, object> _dictionary)
	{
		PlayerData result = new PlayerData
		{
			playerId = (string)_dictionary["id"],
			name = (string)_dictionary["name"]
		};
		if (_dictionary.ContainsKey("acceptNotifications"))
		{
			result.acceptNotifications = (bool)_dictionary["acceptNotifications"];
		}
		if (_dictionary.ContainsKey("facebookId"))
		{
			result.facebookId = (string)_dictionary["facebookId"];
		}
		if (_dictionary.ContainsKey("gameCenterId"))
		{
			result.gameCenterId = (string)_dictionary["gameCenterId"];
		}
		return result;
	}

	public static NotificationData[] ParseNotifications(Dictionary<string, object> _dictionary)
	{
		List<object> list = _dictionary["data"] as List<object>;
		NotificationData[] array = new NotificationData[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			array[i].playerId = (string)dictionary["playerId"];
			array[i].message = (string)dictionary["message"];
			array[i].read = (bool)dictionary["read"];
			array[i].type = (string)dictionary["type"];
			if (dictionary.ContainsKey("data"))
			{
				array[i].data = (string)dictionary["data"];
			}
		}
		return array;
	}

	public static DataBlob CreateLevelDataBlob(Minigame _minigameData)
	{
		MemoryStream inputStream = LevelSerializer.SerializeLevelToStream(_minigameData);
		MemoryStream memoryStream = FilePacker.ZipStream(inputStream, 5);
		string[] value = new string[2] { "TestTag1", "TestTag2" };
		Hashtable hashtable = new Hashtable();
		hashtable.Add("name", _minigameData.m_name);
		hashtable.Add("description", _minigameData.m_description);
		if (_minigameData.m_minigameId != null)
		{
			hashtable.Add("id", _minigameData.m_minigameId);
		}
		hashtable.Add("tags", value);
		hashtable.Add("creatorName", PlayerPrefsX.GetUserName());
		hashtable.Add("creatorId", PlayerPrefsX.GetUserId());
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			hashtable.Add("creatorFacebookId", PlayerPrefsX.GetFacebookId());
		}
		if (PlayerPrefsX.GetGameCenterId() != null)
		{
			hashtable.Add("creatorGameCenterId", PlayerPrefsX.GetGameCenterId());
		}
		hashtable.Add("published", _minigameData.m_published);
		string text = Json.Serialize(hashtable);
		Debug.Log(text);
		Hashtable header = new Hashtable();
		byte[] data = FilePacker.CombineByteArrays(new byte[2][]
		{
			FilePacker.StringToByteArray(text),
			memoryStream.ToArray()
		}, header);
		return new DataBlob
		{
			header = header,
			data = data
		};
	}

	public static DataBlob CreateGhostDataBlob(Ghost _ghost)
	{
		MemoryStream memoryStream = Ghost.SerializeToStream(_ghost);
		Debug.Log("GHOSTSIZE: " + memoryStream.Length);
		MemoryStream memoryStream2 = FilePacker.ZipStream(memoryStream, 5);
		Debug.Log("ZIPSIZE: " + memoryStream2.Length);
		Hashtable hashtable = new Hashtable();
		hashtable.Add("data", "replay");
		string text = Json.Serialize(hashtable);
		Debug.Log(text);
		Hashtable header = new Hashtable();
		byte[] data = FilePacker.CombineByteArrays(new byte[2][]
		{
			FilePacker.StringToByteArray(text),
			memoryStream2.ToArray()
		}, header);
		return new DataBlob
		{
			header = header,
			data = data
		};
	}

	public static string GenerateQuickLoginJSON()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("id", PlayerPrefsX.GetUserId());
		string text = Json.Serialize(hashtable);
		Debug.Log("QUICK LOGIN JSON GENERATED: " + text);
		return text;
	}

	public static string GenerateUserJSON()
	{
		Hashtable hashtable = new Hashtable();
		if (PlayerPrefsX.GetUserId() != null)
		{
			hashtable.Add("id", PlayerPrefsX.GetUserId());
		}
		if (PlayerPrefsX.GetUserName() != null)
		{
			hashtable.Add("name", PlayerPrefsX.GetUserName());
		}
		string gameCenterId = PlayerPrefsX.GetGameCenterId();
		if (gameCenterId != null)
		{
			hashtable.Add("gameCenterId", gameCenterId);
		}
		string facebookId = PlayerPrefsX.GetFacebookId();
		if (facebookId != null)
		{
			hashtable.Add("facebookId", facebookId);
		}
		string text = Json.Serialize(hashtable);
		Debug.Log("USER JSON GENERATED: " + text);
		return text;
	}
}
