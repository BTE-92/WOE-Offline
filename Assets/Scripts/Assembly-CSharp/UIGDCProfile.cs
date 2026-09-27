using System.Collections.Generic;

public class UIGDCProfile : UIScrollableCanvas
{
	private const string FACEBOOK_LOGOUT = "Logout from facebook";

	private const string FACEBOOK_LOGIN = "Connect to facebook";

	private const string GAMECENTER_LOGIN = "Connect to GameCenter";

	private const string GAMECENTER_LOGOUT = "Logout from GameCenter";

	private UITextField m_nameField;

	public Keyboard m_keyboard;

	public string m_name;

	private UITextButton m_facebookButton;

	private UITextButton m_gameCenterButton;

	private UITextButton m_backButton;

	protected UIVerticalList m_verticalArea;

	public UIGDCProfile(UIScrollableCanvas _parent)
		: base(_parent, "Discover")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		UIModel uIModel = new UIModel(this);
		m_name = PlayerPrefsX.GetUserName();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Maincontainer");
		uIVerticalList.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(uIVerticalList, string.Empty);
		m_backButton = new UITextButton(parent, "BackButton", "Back", "Fonts/HurmeRegular", 0.05f);
		m_backButton.SetHorizontalAlign(0f);
		m_verticalArea = new UIVerticalList(uIVerticalList, "Discover Vertical Area");
		m_verticalArea.SetHorizontalAlign(0.5f);
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, "Buttoncontainer");
		uIVerticalList2.SetAlign(0.5f, 1f);
		m_facebookButton = new UITextButton(uIVerticalList2, "facebookButton", string.Empty, "Fonts/HurmeRegular", 0.05f);
		m_gameCenterButton = new UITextButton(uIVerticalList2, "gameCenterButton", string.Empty, "Fonts/HurmeRegular", 0.05f);
		SetSocialButtonTexts();
		EventS.AddListener("SAVED_MINIGAME_BANNER_EVENT", ContentEventHandler, false);
		EventS.AddListener("PUBLISHED_MINIGAME_BANNER_EVENT", ContentEventHandler, false);
	}

	private void SetSocialButtonTexts()
	{
		string text = ((PlayerPrefsX.GetFacebookId() != null) ? "Logout from facebook" : "Connect to facebook");
		m_facebookButton.SetText(text);
		string text2 = ((PlayerPrefsX.GetGameCenterId() != null) ? "Logout from GameCenter" : "Connect to GameCenter");
		m_gameCenterButton.SetText(text2);
	}

	public override void Destroy()
	{
		m_keyboard = null;
		base.Destroy();
		EventS.RemoveComponentWithIdentifier("SAVED_MINIGAME_BANNER_EVENT");
		EventS.RemoveComponentWithIdentifier("PUBLISHED_MINIGAME_BANNER_EVENT");
		WWWRequestManager.RemoveRequestsWithTag("MINIGAME_LIST_DOWNLOAD");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<MinigameMetaData> cache = CacheManager.GetCache("PROFILE_CACHE") as Cache<MinigameMetaData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/minigame/meta/find?creatorId=" + PlayerPrefsX.GetUserId(), "MINIGAME_LIST_DOWNLOAD", false, 5f);
		}
		else
		{
			GenerateContent(cache);
		}
		if (wWWRequest != null)
		{
			wWWRequest.requestComplete += ContentLoadOk;
			wWWRequest.requestFailed += ContentLoadFailed;
		}
	}

	private void GenerateContent(Cache<MinigameMetaData> _cache)
	{
		m_verticalArea.DestroyChildren(4);
		MinigameMetaData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			if (!objects[i].published)
			{
				new UISavedMinigameBanner(m_verticalArea, "MinigameBanner", objects[i]);
			}
		}
		for (int j = 0; j < objects.Length; j++)
		{
			if (objects[j].published)
			{
				new UIPublishedMinigameBanner(m_verticalArea, "MinigameBanner", objects[j]);
			}
		}
		Update();
	}

	private void ContentEventHandler(EventC _c)
	{
		MinigameMetaData minigameMetaData = (MinigameMetaData)_c.properties["metadata"];
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MinigameEditorLoadState(minigameMetaData.id));
	}

	private void ContentLoadOk(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<MinigameMetaData> cache = new Cache<MinigameMetaData>("PROFILE_CACHE", 60f);
			cache.AddToHead(ClientTools.ParseMinigameList(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("MiniGameList server error!");
		}
	}

	private void ContentLoadFailed(WWWRequest _request)
	{
		Debug.LogError("Minigame list download failed");
	}

	public override void Step()
	{
		if (m_keyboard != null)
		{
			m_keyboard.Update();
		}
		if (m_backButton.m_hit)
		{
			UIGDCMenuBase uIGDCMenuBase = m_parent as UIGDCMenuBase;
			uIGDCMenuBase.DestroyChildren();
			new UIGDCDiscover(uIGDCMenuBase);
			uIGDCMenuBase.GoToPage(0, true);
			uIGDCMenuBase.Update();
		}
		if (m_facebookButton.m_hit)
		{
			FacebookButtonHit();
		}
		if (m_gameCenterButton.m_hit)
		{
			GameCenterButtonHit();
		}
		base.Step();
	}

	private void FacebookButtonHit()
	{
		if (PlayerPrefsX.GetFacebookId() == null)
		{
			FacebookManager.Login(FBLoginComplete);
			return;
		}
		m_facebookButton.SetText("Connect to facebook");
		FacebookManager.Logout();
	}

	private void GameCenterButtonHit()
	{
		if (PlayerPrefsX.GetGameCenterId() == null)
		{
			GameCenterManager.Login(GCLoginComplete);
			return;
		}
		m_gameCenterButton.SetText("Connect to GameCenter");
		GameCenterManager.Logout();
	}

	private void FBLoginComplete(FBUserData? _userData)
	{
		if (_userData.HasValue)
		{
			Debug.Log("FB Login complete");
			ServerManager.Login(LoginOk, LoginFailed, ClientTools.GenerateUserJSON());
		}
	}

	private void GCLoginComplete()
	{
		ServerManager.Login(LoginOk, LoginFailed, ClientTools.GenerateUserJSON());
	}

	private void LoginOk(WWWRequest req)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(req.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			PlayerPrefsX.SetPlayerData(ClientTools.ParsePlayerData(dictionary));
			SetSocialButtonTexts();
			ServerManager.ReloadFriends();
			Debug.Log("Login to social service success");
			return;
		}
		string text = (string)dictionary["error"];
		if (text.Equals("MULTIPLE"))
		{
			PlayerData[] players = ClientTools.ParsePlayers(dictionary);
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MergeAccountState(players));
		}
		else
		{
			Debug.LogError("SERVER ERROR when trying to login after connecting to a social service");
		}
	}

	private void LoginFailed(WWWRequest req)
	{
		Debug.LogError("LOGIN AFTER FACEBOOK / GC INTEGRATION FAILED");
	}
}
