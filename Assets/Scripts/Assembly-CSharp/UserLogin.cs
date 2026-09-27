using System.Collections.Generic;
using UnityEngine;

public static class UserLogin
{
	public static UserLoginState m_userLoginState;

	public static Dictionary<string, object> m_mergeData;

	public static void Login()
	{
		Debug.Log("Start login sequence");
		if (m_userLoginState == UserLoginState.NOT_LOGGED_IN)
		{
			if (PlayerPrefsX.GetUserId() == null)
			{
				NewUserLogin();
			}
			else
			{
				ExistingUserLogin();
			}
		}
	}

	private static void NewUserLogin()
	{
		PlayerPrefsX.SetUserName(GenerateNewUserName());
		GameCenterManager.Login(NewUserGCLoginComplete);
	}

	private static string GenerateNewUserName()
	{
		return "New User " + Random.Range(0, int.MaxValue);
	}

	private static void NewUserGCLoginComplete()
	{
		Debug.Log("GC Login complete");
		StartPlayerLogin();
	}

	private static void ExistingUserLogin()
	{
		StartPlayerLogin();
		if (PlayerPrefsX.GetGameCenterId() != null)
		{
			GameCenterManager.Login(GCLoginCompelete);
		}
		else
		{
			GameCenterManager.NoLogin();
		}
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			FacebookManager.Login(FBLoginComplete);
		}
		else
		{
			FacebookManager.NoLogin();
		}
	}

	private static void GCLoginCompelete()
	{
		Debug.Log("GC Login complete");
	}

	private static void FBLoginComplete(FBUserData? _fbUserData)
	{
		if (_fbUserData.HasValue)
		{
			PlayerPrefsX.SetFacebookId(_fbUserData.Value.id);
			PlayerPrefsX.SetFacebookName(_fbUserData.Value.username);
			PlayerPrefsX.SetUserName(_fbUserData.Value.username);
		}
		Debug.Log("FB Login complete");
	}

	private static void StartPlayerLogin()
	{
		m_userLoginState = UserLoginState.STARTED_LOGIN;
		if (PlayerPrefsX.GetUserId() != null)
		{
			Debug.Log("Starting login with EXISTING ID: " + PlayerPrefsX.GetUserId());
			ServerManager.Login(LoginOk, LoginFailed, ClientTools.GenerateUserJSON());
		}
		else
		{
			Debug.Log("Starting login with NEW USER. GameCenter id: " + PlayerPrefsX.GetGameCenterId());
			ServerManager.Login(LoginOk, LoginFailed, ClientTools.GenerateUserJSON());
		}
	}

	private static void LoginOk(WWWRequest req)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(req.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			PlayerPrefsX.SetPlayerData(ClientTools.ParsePlayerData(dictionary));
			m_userLoginState = UserLoginState.LOGGED_IN;
			ServerManager.RegisterForNotifications();
			Server.GetLikes(LikesOk);
			Debug.Log("PLAYER LOGGED IN");
		}
		else
		{
			string text = (string)dictionary["error"];
			Debug.LogError("USER LOGIN FAILED. Server response: " + text);
			m_userLoginState = UserLoginState.NOT_LOGGED_IN;
		}
	}

	private static void LoginFailed(WWWRequest req)
	{
		Debug.LogError("USER LOGIN FAILED");
		m_userLoginState = UserLoginState.NOT_LOGGED_IN;
	}

	public static void ChangeUserName(string _newName)
	{
		if (m_userLoginState == UserLoginState.LOGGED_IN)
		{
			PlayerPrefsX.SetUserName(_newName);
			WWWRequestManager.RemoveRequestsWithTag("PLAYER_UPDATE");
			string url = IpConfig.ServerUrl + "/v1/player/save?id=" + PlayerPrefsX.GetUserId();
			WWWRequest wWWRequest = new PostRequest(url, ClientTools.GenerateUserJSON(), null, "PLAYER_UPDATE", false, 5f);
			wWWRequest.requestComplete += UserDataUpdateOk;
			wWWRequest.requestFailed += UserDataUpdateFailed;
		}
	}

	private static void UserDataUpdateOk(WWWRequest req)
	{
		Debug.Log("USER DATA UPDATE OK: " + req.m_WWW.text);
	}

	private static void UserDataUpdateFailed(WWWRequest req)
	{
		Debug.LogError("USER DATA UPDATE FAILED");
	}

	private static void LikesOk(List<string> likes)
	{
	}
}
