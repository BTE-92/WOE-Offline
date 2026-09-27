using UnityEngine;

public class PlayerPrefsX
{
	public const string GAME_CENTER_ID = "GameCenterId";

	public const string GAME_CENTER_NAME = "GameCenterName";

	public const string FACEBOOK_ID = "FacebookId";

	public const string FACEBOOK_NAME = "FacebookName";

	public const string USER_NAME = "UserName";

	public const string USER_ID = "UserID";

	public const string ACCEPT_NOTIFICATIONS = "AcceptNotifications";

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
		PlayerPrefs.Save();
	}

	public static void SetPlayerData(PlayerData _data)
	{
		PlayerPrefs.SetString("UserID", _data.playerId);
		PlayerPrefs.SetString("UserName", _data.name);
		PlayerPrefs.SetString("GameCenterId", _data.gameCenterId);
		PlayerPrefs.SetString("FacebookId", _data.facebookId);
		PlayerPrefs.SetInt("AcceptNotifications", _data.acceptNotifications ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static void SetGameCenterName(string _value)
	{
		SaveString("GameCenterName", _value);
	}

	public static void SetFacebookName(string _value)
	{
		SaveString("FacebookName", _value);
	}

	public static void SetAcceptNotifications(bool _value)
	{
		SetBool("AcceptNotifications", _value);
	}

	public static bool GetAcceptNotifications()
	{
		return GetBool("AcceptNotifications");
	}

	public static void SetFacebookId(string _facebookId)
	{
		SaveString("FacebookId", _facebookId);
	}

	public static void SetGameCenterId(string _gameCenterId)
	{
		SaveString("GameCenterId", _gameCenterId);
	}

	public static void SetUserName(string _userName)
	{
		SaveString("UserName", _userName);
	}

	public static void SetUserId(string _userId)
	{
		SaveString("UserID", _userId);
	}

	public static string GetFacebookId()
	{
		return GetString("FacebookId");
	}

	public static string GetUserName()
	{
		if (GetString("FacebookName") != null)
		{
			return GetString("FacebookName");
		}
		if (GetString("GameCenterName") != null)
		{
			return GetString("GameCenterName");
		}
		return GetString("UserName");
	}

	public static string GetUserId()
	{
		return GetString("UserID");
	}

	public static string GetGameCenterId()
	{
		return GetString("GameCenterId");
	}

	private static void SaveString(string name, string value)
	{
		PlayerPrefs.SetString(name, value);
		PlayerPrefs.Save();
	}

	private static string GetString(string key)
	{
		string text = PlayerPrefs.GetString(key);
		if (text.Equals(string.Empty))
		{
			return null;
		}
		return text;
	}

	private static void SetBool(string _name, bool _booleanValue)
	{
		PlayerPrefs.SetInt(_name, _booleanValue ? 1 : 0);
		PlayerPrefs.Save();
	}

	private static bool GetBool(string _name)
	{
		return PlayerPrefs.GetInt(_name) == 1;
	}

	private static bool GetBool(string name, bool defaultValue)
	{
		if (PlayerPrefs.HasKey(name))
		{
			return GetBool(name);
		}
		return defaultValue;
	}
}
