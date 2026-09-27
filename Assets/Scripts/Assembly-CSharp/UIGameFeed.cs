using System.Collections.Generic;

public class UIGameFeed : UIScrollableCanvas
{
	private UIRectButton m_createButton;

	protected UIVerticalList m_verticalArea;

	private FeedData[] m_gameFeedList;

	public UIGameFeed(UIPagedCanvas _parent)
		: base(_parent, "Game Feed")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		m_verticalArea = new UIVerticalList(this, "Game Feed Vertical Area");
		m_verticalArea.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(m_verticalArea, string.Empty);
		UILabel uILabel = new UILabel(parent, "Hiding Label", "Game Feed", Align.Left, Align.Center);
	}

	public override void Step()
	{
		base.Step();
	}

	public override void Destroy()
	{
		base.Destroy();
		WWWRequestManager.RemoveRequestsWithTag("GAME_FEED_LIST_DOWNLOAD");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<FeedData> cache = CacheManager.GetCache("GAME_FEED_CACHE") as Cache<FeedData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/feed/find?playerId=" + PlayerPrefsX.GetUserId(), "GAME_FEED_LIST_DOWNLOAD", false, 5f);
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

	private void GenerateContent(Cache<FeedData> _cache)
	{
		m_verticalArea.DestroyChildren(1);
		FeedData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			new UIGameFeedBanner(m_verticalArea, "GameFeedBanner", objects[i]);
		}
		Update();
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<FeedData> cache = new Cache<FeedData>("GAME_FEED_CACHE", 60f);
			cache.AddToHead(ClientTools.ParseGameFeedList(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("Game feed list server error!");
		}
	}

	private void ServerRequestFAILED(WWWRequest _request)
	{
		Debug.LogError("Game feed list download failed");
	}
}
