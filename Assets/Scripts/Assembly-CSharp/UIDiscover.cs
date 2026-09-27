using System.Collections.Generic;

public class UIDiscover : UIScrollableCanvas
{
	protected UIVerticalList m_verticalArea;

	private MinigameMetaData[] m_minigameList;

	public UIDiscover(UIPagedCanvas _parent)
		: base(_parent, "Discover")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		m_verticalArea = new UIVerticalList(this, "Discover Vertical Area");
		m_verticalArea.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(m_verticalArea, string.Empty);
		UILabel uILabel = new UILabel(parent, "Hiding Label", "Discover", Align.Left, Align.Center);
		EventS.AddListener("MINIGAME_BANNER_EVENT", ContentEventHandler, false);
	}

	public override void Destroy()
	{
		base.Destroy();
		EventS.RemoveComponentWithIdentifier("MINIGAME_BANNER_EVENT");
		WWWRequestManager.RemoveRequestsWithTag("MINIGAME_LIST_DOWNLOAD");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<MinigameMetaData> cache = CacheManager.GetCache("DISCOVER_CACHE") as Cache<MinigameMetaData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/minigame/meta/find?published=true", "MINIGAME_LIST_DOWNLOAD", false, 5f);
		}
		else
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
		m_verticalArea.DestroyChildren(1);
		MinigameMetaData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			new UIMinigameBanner(m_verticalArea, "MinigameBanner", objects[i]);
		}
		Update();
	}

	private void ContentEventHandler(EventC _c)
	{
		MinigameMetaData minigameMetaData = (MinigameMetaData)_c.properties["metadata"];
		UIPagedCanvas uIPagedCanvas = m_parent as UIPagedCanvas;
		UIMinigameSummary uIMinigameSummary = new UIMinigameSummary(uIPagedCanvas, minigameMetaData);
		uIMinigameSummary.SetVerticalAlign(1f);
		uIPagedCanvas.NextPage();
		uIPagedCanvas.Update();
		uIPagedCanvas.DisableTouchAreas();
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
