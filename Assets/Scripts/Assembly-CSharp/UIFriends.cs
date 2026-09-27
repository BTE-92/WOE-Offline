using System.Collections.Generic;

public class UIFriends : UIScrollableCanvas
{
	protected UIVerticalList m_verticalArea;

	private PlayerData[] m_friendList;

	public UIFriends(UIPagedCanvas _parent)
		: base(_parent, "Friends")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		m_verticalArea = new UIVerticalList(this, "Friends Vertical Area");
		m_verticalArea.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(m_verticalArea, string.Empty);
		UILabel uILabel = new UILabel(parent, "Hiding Label", "Friends", Align.Left, Align.Center);
	}

	public override void Destroy()
	{
		base.Destroy();
		WWWRequestManager.RemoveRequestsWithTag("FRIENDS_DOWNLOAD");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<PlayerData> cache = CacheManager.GetCache("FRIENDS_CACHE") as Cache<PlayerData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/player/friend/find?playerId=" + PlayerPrefsX.GetUserId(), "FRIENDS_DOWNLOAD", false, 5f);
		}
		else
		{
			Debug.Log("Generating friends list from cache");
			GenerateContent(cache);
		}
		if (wWWRequest != null)
		{
			wWWRequest.requestComplete += ServerRequestOK;
			wWWRequest.requestFailed += ServerRequestFAILED;
		}
	}

	private void GenerateContent(Cache<PlayerData> _cache)
	{
		m_verticalArea.DestroyChildren(1);
		PlayerData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			new UIFriendBanner(m_verticalArea, "FriendBanner", objects[i]);
		}
		Update();
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<PlayerData> cache = new Cache<PlayerData>("FRIENDS_CACHE", 60f);
			cache.AddToHead(ClientTools.ParsePlayers(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("Friends server error!");
		}
	}

	private void ServerRequestFAILED(WWWRequest _request)
	{
		Debug.LogError("Friends download failed");
	}
}
