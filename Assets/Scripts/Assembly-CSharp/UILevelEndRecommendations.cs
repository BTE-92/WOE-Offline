using System.Collections.Generic;
using UnityEngine;

public class UILevelEndRecommendations : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_exitButton;

	public UIScrollableCanvas m_scrollableGameCanvas;

	public UIVerticalList m_highscoreList;

	protected UIVerticalList m_verticalArea;

	private MinigameMetaData[] m_minigameList;

	private bool m_contentLoaded;

	public UILevelEndRecommendations(UIComponent _parent)
		: base(_parent, "recommendations")
	{
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(this, "Content", null, string.Empty);
		uICanvas.SetWidth(0.5f, RelativeTo.ScreenWidth);
		uICanvas.SetHeight(1f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		m_scrollableGameCanvas = new UIScrollableCanvas(uICanvas, "HighscoreScrollCanvas");
		m_scrollableGameCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_scrollableGameCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollableGameCanvas.RemoveDrawHandler();
		m_verticalArea = new UIVerticalList(m_scrollableGameCanvas, "HighscoreList");
		m_verticalArea.SetVerticalAlign(1f);
		m_verticalArea.RemoveDrawHandler();
		m_verticalArea.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_verticalArea.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		EventS.AddListener("MINIGAME_BANNER_EVENT", ContentEventHandler, false);
		m_contentLoaded = false;
	}

	public override void Step()
	{
		if (!m_contentLoaded)
		{
			m_contentLoaded = true;
			LoadContent();
		}
		base.Step();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<MinigameMetaData> cache = CacheManager.GetCache("DISCOVER_CACHE") as Cache<MinigameMetaData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/minigame/meta/find?published=true", "MINIGAME_LIST_DOWNLOAD", false, 5f);
		}
		else if (m_verticalArea.m_childs.Count == 0)
		{
			GenerateContent(cache);
		}
		if (wWWRequest != null)
		{
			wWWRequest.requestComplete += ServerRequestOK;
			wWWRequest.requestFailed += ServerRequestFAILED;
		}
	}

	private void GenerateContent(Cache<MinigameMetaData> _cache)
	{
		m_verticalArea.DestroyChildren();
		MinigameMetaData[] objects = _cache.GetObjects();
		if (objects.Length <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		int num = 0;
		while (list.Count < 4 && num < 1000)
		{
			int num2 = Random.Range(0, objects.Length - 1);
			if (objects[num2].id != PsState.m_lastDownloadedLevelId && !list.Contains(num2))
			{
				new UIMinigameBanner(m_verticalArea, "MinigameBanner", objects[num2]);
				list.Add(num2);
			}
			num++;
		}
		m_scrollableGameCanvas.Update();
	}

	private void ContentEventHandler(EventC _c)
	{
		MinigameMetaData minigameMetaData = (MinigameMetaData)_c.properties["metadata"];
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MinigameLoadState(minigameMetaData.id));
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<MinigameMetaData> cache = new Cache<MinigameMetaData>("DISCOVER_CACHE", 60f);
			cache.AddToHead(ClientTools.ParseMinigameList(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("MiniGameList server error!");
		}
	}

	private void ServerRequestFAILED(WWWRequest _request)
	{
		Debug.LogError("Minigame list download failed");
	}
}
