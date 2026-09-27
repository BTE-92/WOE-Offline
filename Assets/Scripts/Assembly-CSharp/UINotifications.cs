using System.Collections.Generic;

public class UINotifications : UIScrollableCanvas
{
	protected UIVerticalList m_verticalArea;

	private NotificationData[] m_notificationList;

	public UINotifications(UIPagedCanvas _parent)
		: base(_parent, "Notifications")
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		m_verticalArea = new UIVerticalList(this, "Notifications Vertical Area");
		m_verticalArea.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(m_verticalArea, string.Empty);
		UILabel uILabel = new UILabel(parent, "Hiding Label", "Notifications", Align.Left, Align.Center);
	}

	public override void Destroy()
	{
		base.Destroy();
		WWWRequestManager.RemoveRequestsWithTag("NOTIFICATIONS_DOWNLOAD");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<NotificationData> cache = CacheManager.GetCache("NOTIFICATION_CACHE") as Cache<NotificationData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/notification/find?playerId=" + PlayerPrefsX.GetUserId(), "NOTIFICATIONS_DOWNLOAD", false, 5f);
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

	private void GenerateContent(Cache<NotificationData> _cache)
	{
		m_verticalArea.DestroyChildren(1);
		NotificationData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			new UINotificationBanner(m_verticalArea, "NotificationBanner", objects[i]);
		}
		Update();
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<NotificationData> cache = new Cache<NotificationData>("NOTIFICATION_CACHE", 60f);
			cache.AddToHead(ClientTools.ParseNotifications(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("Notifications server error!");
		}
	}

	private void ServerRequestFAILED(WWWRequest _request)
	{
		Debug.LogError("Notifications download failed");
	}
}
