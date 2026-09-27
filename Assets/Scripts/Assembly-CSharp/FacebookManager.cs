using System;
using Facebook;
using UnityEngine;

public static class FacebookManager
{
	public class FacebookPictureDownloader
	{
		public Action<Texture2D> m_callback;

		public FacebookPictureDownloader(string _facebookId, bool _largePicture)
		{
			string text = "https://graph.facebook.com/" + _facebookId + "/picture";
			if (_largePicture)
			{
				text += "?type=large";
			}
			GetRequest getRequest = new GetRequest(text, string.Empty, false, 5f);
			getRequest.requestComplete += PictureDownloaded;
		}

		private void PictureDownloaded(WWWRequest _request)
		{
			m_callback(_request.m_WWW.texture);
		}
	}

	public static Action<FBUserData?> m_loginCompleteCallback;

	public static bool m_loginComplete;

	private static bool m_initComplete;

	public static void Login(Action<FBUserData?> _loginComplete)
	{
		Debug.Log("FB Login start");
		m_loginComplete = false;
		m_loginCompleteCallback = _loginComplete;
		if (m_initComplete)
		{
			OnFBInitComplete();
		}
		else
		{
			FB.Init(OnFBInitComplete);
		}
	}

	public static void Logout()
	{
		Debug.Log("FB Logout");
		PlayerPrefsX.DeleteKey("FacebookId");
		ServerManager.ReloadFriends();
	}

	public static void NoLogin()
	{
		m_loginComplete = true;
	}

	public static bool IsLoggedIn()
	{
		return PlayerPrefsX.GetFacebookId() != null && FB.IsLoggedIn;
	}

	private static void OnFBInitComplete()
	{
		m_initComplete = true;
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		if (FB.IsLoggedIn)
		{
			FB.API("/me?fields=id,username", HttpMethod.GET, FBUserDataCallback);
		}
		else
		{
			FB.Login("read_friendlists", FBLoginCallback);
		}
	}

	private static void FBLoginCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError("Error Response from FB Login:\n" + result.Error);
		}
		else if (FB.IsLoggedIn)
		{
			Debug.Log("FB Login was successful!");
			FB.API("/me?fields=id,username", HttpMethod.GET, FBUserDataCallback);
		}
		else
		{
			Debug.Log("FB Login cancelled by Player");
			m_loginComplete = true;
			m_loginCompleteCallback(null);
		}
	}

	private static void FBUserDataCallback(FBResult result)
	{
		m_loginComplete = true;
		if (result.Error != null)
		{
			Debug.LogError("Error Response:\n" + result.Error);
			m_loginCompleteCallback(null);
			return;
		}
		Debug.Log("FB Me call was successful!");
		FBUserData value = ClientTools.ParseFBUserData(result.Text);
		PlayerPrefsX.SetFacebookId(value.id);
		PlayerPrefsX.SetFacebookName(value.username);
		Debug.Log(result.Text);
		m_loginCompleteCallback(value);
	}

	public static void GetPicture(string _facebookId, Action<Texture2D> _callback, bool _largePicture = false)
	{
		FacebookPictureDownloader facebookPictureDownloader = new FacebookPictureDownloader(_facebookId, _largePicture);
		facebookPictureDownloader.m_callback = _callback;
	}
}
