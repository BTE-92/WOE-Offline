using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

public sealed class InteractiveConsole : MonoBehaviour
{
	private bool isInit;

	public string FriendSelectorTitle = string.Empty;

	public string FriendSelectorMessage = "Derp";

	public string FriendSelectorFilters = "[\"all\",\"app_users\",\"app_non_users\"]";

	public string FriendSelectorData = "{}";

	public string FriendSelectorExcludeIds = string.Empty;

	public string FriendSelectorMax = string.Empty;

	public string DirectRequestTitle = string.Empty;

	public string DirectRequestMessage = "Herp";

	private string DirectRequestTo = string.Empty;

	public string FeedToId = string.Empty;

	public string FeedLink = string.Empty;

	public string FeedLinkName = string.Empty;

	public string FeedLinkCaption = string.Empty;

	public string FeedLinkDescription = string.Empty;

	public string FeedPicture = string.Empty;

	public string FeedMediaSource = string.Empty;

	public string FeedActionName = string.Empty;

	public string FeedActionLink = string.Empty;

	public string FeedReference = string.Empty;

	public bool IncludeFeedProperties;

	private Dictionary<string, string[]> FeedProperties = new Dictionary<string, string[]>();

	public string PayProduct = string.Empty;

	public string ApiQuery = string.Empty;

	public float PlayerLevel = 1f;

	public string Width = "800";

	public string Height = "600";

	public bool CenterHorizontal = true;

	public bool CenterVertical;

	public string Top = "10";

	public string Left = "10";

	private string status = "Ready";

	private string lastResponse = string.Empty;

	public GUIStyle textStyle = new GUIStyle();

	private Texture2D lastResponseTexture;

	private Vector2 scrollPosition = Vector2.zero;

	private int buttonHeight = 60;

	private int mainWindowWidth = Screen.width - 30;

	private int mainWindowFullWidth = Screen.width;

	private int TextWindowHeight
	{
		get
		{
			return (!IsHorizontalLayout()) ? 85 : Screen.height;
		}
	}

	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}

	private void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		isInit = true;
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}

	private void CallFBLogin()
	{
		FB.Login("email,publish_actions", LoginCallback);
	}

	private void LoginCallback(FBResult result)
	{
		if (result.Error != null)
		{
			lastResponse = "Error Response:\n" + result.Error;
		}
		else if (!FB.IsLoggedIn)
		{
			lastResponse = "Login cancelled by Player";
		}
		else
		{
			lastResponse = "Login was successful!";
		}
	}

	private void CallFBLogout()
	{
		FB.Logout();
	}

	private void CallFBPublishInstall()
	{
		FB.PublishInstall(PublishComplete);
	}

	private void PublishComplete(FBResult result)
	{
		Debug.Log("publish response: " + result.Text);
	}

	private void CallAppRequestAsFriendSelector()
	{
		int? maxRecipients = null;
		if (FriendSelectorMax != string.Empty)
		{
			try
			{
				maxRecipients = int.Parse(FriendSelectorMax);
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
		}
		string[] excludeIds = ((!(FriendSelectorExcludeIds == string.Empty)) ? FriendSelectorExcludeIds.Split(',') : null);
		string friendSelectorFilters = FriendSelectorFilters;
		FB.AppRequest(FriendSelectorMessage, null, friendSelectorFilters, excludeIds, maxRecipients, FriendSelectorData, FriendSelectorTitle, Callback);
	}

	private void CallAppRequestAsDirectRequest()
	{
		if (DirectRequestTo == string.Empty)
		{
			throw new ArgumentException("\"To Comma Ids\" must be specificed", "to");
		}
		string directRequestTitle = DirectRequestTitle;
		FacebookDelegate callback = Callback;
		FB.AppRequest(DirectRequestMessage, DirectRequestTo.Split(','), string.Empty, null, null, string.Empty, directRequestTitle, callback);
	}

	private void CallFBFeed()
	{
		Dictionary<string, string[]> properties = null;
		if (IncludeFeedProperties)
		{
			properties = FeedProperties;
		}
		FB.Feed(FeedToId, FeedLink, FeedLinkName, FeedLinkCaption, FeedLinkDescription, FeedPicture, FeedMediaSource, FeedActionName, FeedActionLink, FeedReference, properties, Callback);
	}

	private void CallFBPay()
	{
		FB.Canvas.Pay(PayProduct);
	}

	private void CallFBAPI()
	{
		FB.API(ApiQuery, HttpMethod.GET, Callback);
	}

	private void CallFBGetDeepLink()
	{
		FB.GetDeepLink(Callback);
	}

	public void CallAppEventLogEvent()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["fb_level"] = "Player Level";
		FB.AppEvents.LogEvent("fb_mobile_level_achieved", PlayerLevel, dictionary);
		PlayerLevel += 1f;
	}

	public void CallCanvasSetResolution()
	{
		int result;
		if (!int.TryParse(Width, out result))
		{
			result = 800;
		}
		int result2;
		if (!int.TryParse(Height, out result2))
		{
			result2 = 600;
		}
		float result3;
		if (!float.TryParse(Top, out result3))
		{
			result3 = 0f;
		}
		float result4;
		if (!float.TryParse(Left, out result4))
		{
			result4 = 0f;
		}
		if (CenterHorizontal && CenterVertical)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.CenterVertical(), FBScreen.CenterHorizontal());
		}
		else if (CenterHorizontal)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.Top(result3), FBScreen.CenterHorizontal());
		}
		else if (CenterVertical)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.CenterVertical(), FBScreen.Left(result4));
		}
		else
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.Top(result3), FBScreen.Left(result4));
		}
	}

	private void Awake()
	{
		textStyle.alignment = TextAnchor.UpperLeft;
		textStyle.wordWrap = true;
		textStyle.padding = new RectOffset(10, 10, 10, 10);
		textStyle.stretchHeight = true;
		textStyle.stretchWidth = false;
		FeedProperties.Add("key1", new string[1] { "valueString1" });
		FeedProperties.Add("key2", new string[2] { "valueString2", "http://www.facebook.com" });
	}

	private void OnGUI()
	{
		if (IsHorizontalLayout())
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
		}
		GUILayout.Space(5f);
		GUILayout.Box("Status: " + status, GUILayout.MinWidth(mainWindowWidth));
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			scrollPosition.y += Input.GetTouch(0).deltaPosition.y;
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinWidth(mainWindowFullWidth));
		GUILayout.BeginVertical();
		GUI.enabled = !isInit;
		if (Button("FB.Init"))
		{
			CallFBInit();
			status = "FB.Init() called with " + FB.AppId;
		}
		GUILayout.BeginHorizontal();
		GUI.enabled = isInit;
		if (Button("Login"))
		{
			CallFBLogin();
			status = "Login called";
		}
		GUI.enabled = FB.IsLoggedIn;
		if (Button("Logout"))
		{
			CallFBLogout();
			status = "Logout called";
		}
		GUI.enabled = isInit;
		GUILayout.EndHorizontal();
		if (Button("Publish Install"))
		{
			CallFBPublishInstall();
			status = "Install Published";
		}
		GUI.enabled = FB.IsLoggedIn;
		GUILayout.Space(10f);
		LabelAndTextField("Title (optional): ", ref FriendSelectorTitle);
		LabelAndTextField("Message: ", ref FriendSelectorMessage);
		LabelAndTextField("Exclude Ids (optional): ", ref FriendSelectorExcludeIds);
		LabelAndTextField("Filters (optional): ", ref FriendSelectorFilters);
		LabelAndTextField("Max Recipients (optional): ", ref FriendSelectorMax);
		LabelAndTextField("Data (optional): ", ref FriendSelectorData);
		if (Button("Open Friend Selector"))
		{
			try
			{
				CallAppRequestAsFriendSelector();
				status = "Friend Selector called";
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("Title (optional): ", ref DirectRequestTitle);
		LabelAndTextField("Message: ", ref DirectRequestMessage);
		LabelAndTextField("To Comma Ids: ", ref DirectRequestTo);
		if (Button("Open Direct Request"))
		{
			try
			{
				CallAppRequestAsDirectRequest();
				status = "Direct Request called";
			}
			catch (Exception ex2)
			{
				status = ex2.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("To Id (optional): ", ref FeedToId);
		LabelAndTextField("Link (optional): ", ref FeedLink);
		LabelAndTextField("Link Name (optional): ", ref FeedLinkName);
		LabelAndTextField("Link Desc (optional): ", ref FeedLinkDescription);
		LabelAndTextField("Link Caption (optional): ", ref FeedLinkCaption);
		LabelAndTextField("Picture (optional): ", ref FeedPicture);
		LabelAndTextField("Media Source (optional): ", ref FeedMediaSource);
		LabelAndTextField("Action Name (optional): ", ref FeedActionName);
		LabelAndTextField("Action Link (optional): ", ref FeedActionLink);
		LabelAndTextField("Reference (optional): ", ref FeedReference);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Properties (optional)", GUILayout.Width(150f));
		IncludeFeedProperties = GUILayout.Toggle(IncludeFeedProperties, "Include");
		GUILayout.EndHorizontal();
		if (Button("Open Feed Dialog"))
		{
			try
			{
				CallFBFeed();
				status = "Feed dialog called";
			}
			catch (Exception ex3)
			{
				status = ex3.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("API: ", ref ApiQuery);
		if (Button("Call API"))
		{
			status = "API called";
			CallFBAPI();
		}
		GUILayout.Space(10f);
		if (Button("Take & upload screenshot"))
		{
			status = "Take screenshot";
			StartCoroutine(TakeScreenshot());
		}
		if (Button("Get Deep Link"))
		{
			CallFBGetDeepLink();
		}
		if (Button("Log FB App Event"))
		{
			status = "Logged FB.AppEvent";
			CallAppEventLogEvent();
		}
		GUILayout.Space(10f);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		if (IsHorizontalLayout())
		{
			GUILayout.EndVertical();
		}
		GUI.enabled = true;
		Rect rect = GUILayoutUtility.GetRect(640f, TextWindowHeight);
		GUI.TextArea(rect, string.Format(" AppId: {0} \n Facebook Dll: {1} \n UserId: {2}\n IsLoggedIn: {3}\n AccessToken: {4}\n AccessTokenExpiresAt: {5}\n {6}", FB.AppId, (!isInit) ? "Not Loaded" : "Loaded Successfully", FB.UserId, FB.IsLoggedIn, FB.AccessToken, FB.AccessTokenExpiresAt, lastResponse), textStyle);
		if (lastResponseTexture != null)
		{
			float num = rect.y + 200f;
			if ((float)(Screen.height - lastResponseTexture.height) < num)
			{
				num = Screen.height - lastResponseTexture.height;
			}
			GUI.Label(new Rect(rect.x + 5f, num, lastResponseTexture.width, lastResponseTexture.height), lastResponseTexture);
		}
		if (IsHorizontalLayout())
		{
			GUILayout.EndHorizontal();
		}
	}

	private void Callback(FBResult result)
	{
		lastResponseTexture = null;
		if (!string.IsNullOrEmpty(result.Error))
		{
			lastResponse = "Error Response:\n" + result.Error;
			return;
		}
		if (!ApiQuery.Contains("/picture"))
		{
			lastResponse = "Success Response:\n" + result.Text;
			return;
		}
		lastResponseTexture = result.Texture;
		lastResponse = "Success Response:\n";
	}

	private IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
		wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
		FB.API("me/photos", HttpMethod.POST, Callback, wwwForm);
	}

	private bool Button(string label)
	{
		return GUILayout.Button(label, GUILayout.MinHeight(buttonHeight), GUILayout.MaxWidth(mainWindowWidth));
	}

	private void LabelAndTextField(string label, ref string text)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.MaxWidth(150f));
		text = GUILayout.TextField(text);
		GUILayout.EndHorizontal();
	}

	private bool IsHorizontalLayout()
	{
		return Screen.orientation == ScreenOrientation.LandscapeLeft;
	}
}
