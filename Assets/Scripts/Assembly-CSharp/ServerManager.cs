using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using MiniJSON;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#else
using NotificationServices = UnityEngine.NotificationServices;
using NotificationType = UnityEngine.PushNotificationType;
#endif

public static class ServerManager
{
	private enum PushRegistrationState
	{
		NOT_INITIALIZED = 0,
		SENT_TO_APPLE = 1,
		SENT_TO_SERVER = 2,
		ERROR = 3,
		REGISTERED = 4
	}

	private enum FriendLoadingState
	{
		NOT_SENT = 0,
		WAIT_FRIEND_LOADING = 1,
		FRIENDS_READY = 2,
		FRIENDS_SENT = 3
	}

	private static FriendLoadingState m_FBFriendLoadingState;

	private static FriendLoadingState m_GCFriendLoadingState;

	private static PushRegistrationState m_pushRegistrationState;

	private static string[] m_facebookFriends;

	public static void RegisterForNotifications()
	{
		if (PlayerPrefs.GetString("deviceToken").Equals(string.Empty))
		{
			Debug.Log("Registering for notifications");
			NotificationServices.RegisterForNotifications((NotificationType)7);
			m_pushRegistrationState = PushRegistrationState.SENT_TO_APPLE;
		}
	}

	public static void Login(Action<WWWRequest> _onOk, Action<WWWRequest> _onFailure, string json)
	{
		string url = IpConfig.ServerUrl + "/v1/player/login";
		WWWRequest wWWRequest = new PostRequest(url, json, null, string.Empty, false, 5f);
		wWWRequest.requestComplete += _onOk;
		wWWRequest.requestFailed += _onFailure;
	}

	public static void ReloadFriends()
	{
		m_FBFriendLoadingState = FriendLoadingState.NOT_SENT;
		m_GCFriendLoadingState = FriendLoadingState.NOT_SENT;
	}

	public static void CancelFriendLoad()
	{
		m_FBFriendLoadingState = FriendLoadingState.FRIENDS_SENT;
		m_GCFriendLoadingState = FriendLoadingState.FRIENDS_SENT;
	}

	public static void Update()
	{
		if (m_pushRegistrationState == PushRegistrationState.SENT_TO_APPLE)
		{
			if (NotificationServices.deviceToken != null)
			{
				sendDeviceTokenToServer();
				m_pushRegistrationState = PushRegistrationState.SENT_TO_SERVER;
			}
			else if (NotificationServices.registrationError != null)
			{
				Debug.LogError("ERROR REGISTERING FOR PUSH MESSAGES: " + NotificationServices.registrationError);
				m_pushRegistrationState = PushRegistrationState.ERROR;
			}
		}
		if (m_FBFriendLoadingState == FriendLoadingState.NOT_SENT && m_GCFriendLoadingState == FriendLoadingState.NOT_SENT && UserLogin.m_userLoginState == UserLoginState.LOGGED_IN && FacebookManager.m_loginComplete && GameCenterManager.m_loginComplete)
		{
			LoadFriends();
		}
		if (m_FBFriendLoadingState == FriendLoadingState.FRIENDS_READY && m_GCFriendLoadingState == FriendLoadingState.FRIENDS_READY)
		{
			SendFriendsToServer();
		}
	}

	private static void sendDeviceTokenToServer()
	{
		string text = IpConfig.ServerUrl + "/v1/push/token/save";
		text = text + "?playerId=" + PlayerPrefsX.GetUserId();
		Debug.Log("Sending deviceToken to server. Token: " + FilePacker.ByteArrayToString(NotificationServices.deviceToken));
		WWWRequest wWWRequest = new PostRequest(text, NotificationServices.deviceToken, null, string.Empty, false, 5f);
		wWWRequest.requestComplete += TokenSendOk;
		wWWRequest.requestFailed += TokenSendFailed;
	}

	private static void TokenSendOk(WWWRequest req)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(req.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Debug.Log("TOKEN SEND: Token send OK");
			PlayerPrefs.SetString("deviceToken", FilePacker.ByteArrayToString(NotificationServices.deviceToken));
			PlayerPrefs.Save();
		}
		else
		{
			Debug.LogError("TOKEN SEND ERROR: " + (string)dictionary["status"]);
		}
	}

	private static void TokenSendFailed(WWWRequest req)
	{
		m_pushRegistrationState = PushRegistrationState.ERROR;
		Debug.Log("TOKEN SEND: Token Send Failed");
	}

	public static void LoadFriends()
	{
		Debug.Log("LOAD FRIENDS");
		CacheManager.RemoveCache("FRIENDS_CACHE");
		if (FacebookManager.IsLoggedIn())
		{
			LoadGCFriends();
		}
		else
		{
			m_GCFriendLoadingState = FriendLoadingState.FRIENDS_READY;
		}
		if (FacebookManager.IsLoggedIn())
		{
			LoadFBFriends();
		}
		else
		{
			m_FBFriendLoadingState = FriendLoadingState.FRIENDS_READY;
		}
	}

	private static void LoadGCFriends()
	{
		m_GCFriendLoadingState = FriendLoadingState.WAIT_FRIEND_LOADING;
		Social.localUser.LoadFriends(delegate(bool success)
		{
			if (success)
			{
				m_GCFriendLoadingState = FriendLoadingState.FRIENDS_READY;
			}
			else
			{
				Debug.LogError("GC friend loading failed");
			}
		});
	}

	private static void LoadFBFriends()
	{
		Debug.Log("Load FB Friends");
		m_FBFriendLoadingState = FriendLoadingState.WAIT_FRIEND_LOADING;
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			FB.API("/me/friends?fields=id,username,installed", HttpMethod.GET, FBFriendsLoaded);
			return;
		}
		m_facebookFriends = new string[0];
		m_FBFriendLoadingState = FriendLoadingState.FRIENDS_READY;
	}

	private static void FBFriendsLoaded(FBResult result)
	{
		Debug.Log("FB FRIENDS RESULT " + result.Text);
		m_facebookFriends = ClientTools.ParseFBFriendIds(result.Text);
		m_FBFriendLoadingState = FriendLoadingState.FRIENDS_READY;
	}

	private static void SendFriendsToServer()
	{
		string text = IpConfig.ServerUrl + "/v1/player/friend/save";
		text = text + "?playerId=" + PlayerPrefsX.GetUserId();
		PostRequest postRequest = new PostRequest(text, GenerateFriendJSON(), null, string.Empty, false, 5f);
		postRequest.requestComplete += FriendSendOk;
		postRequest.requestFailed += FriendSendFailed;
		m_FBFriendLoadingState = FriendLoadingState.FRIENDS_SENT;
		m_GCFriendLoadingState = FriendLoadingState.FRIENDS_SENT;
	}

	private static void FriendSendOk(WWWRequest _request)
	{
		SocialManager.m_friendData = ClientTools.ParseFriendDict(_request);
	}

	private static void FriendSendFailed(WWWRequest req)
	{
		Debug.Log("FRIEND SEND: Friend Send Failed: " + req.m_WWW.text);
	}

	private static string GenerateFriendJSON()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("name", PlayerPrefsX.GetUserName());
		if (PlayerPrefsX.GetFacebookId() != null)
		{
			hashtable.Add("facebookId", PlayerPrefsX.GetFacebookId());
		}
		if (PlayerPrefsX.GetGameCenterId() != null)
		{
			hashtable.Add("gameCenterId", PlayerPrefsX.GetGameCenterId());
		}
		hashtable.Add("gcFriends", GenerateGCFriendJSON());
		hashtable.Add("fbFriends", m_facebookFriends);
		string text = Json.Serialize(hashtable);
		Debug.Log("FRIEND JSON GENERATED: " + text);
		return text;
	}

	private static string[] GenerateGCFriendJSON()
	{
		if (!GameCenterManager.IsLoggedIn())
		{
			return null;
		}
		string[] array = new string[Social.localUser.friends.Length];
		for (int num = Social.localUser.friends.Length - 1; num > -1; num--)
		{
			array[num] = Social.localUser.friends[num].id;
		}
		return array;
	}
}
