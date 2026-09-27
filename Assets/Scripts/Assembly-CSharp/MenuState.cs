using UnityEngine;

public class MenuState : BasicState
{
	public UIMenuBase m_base;

	public UIMenuTabButton m_gameFeed;

	public UIMenuTabButton m_discover;

	public UIMenuTabButton m_pins;

	public UIMenuTabButton m_notifications;

	public UIMenuTabButton m_friends;

	public UIMenuTabButton m_me;

	public string m_currentTab;

	public override void Enter(IStatedObject _parent)
	{
		m_base = new UIMenuBase(null, "Base");
		UIVerticalList parent = new UIVerticalList(m_base.m_left, "LeftMenu");
		m_gameFeed = new UIMenuTabButton(parent, "GameFeed", "Game Feed");
		m_discover = new UIMenuTabButton(parent, "Discover", "Discover");
		m_pins = new UIMenuTabButton(parent, "Pins", "Pins");
		m_notifications = new UIMenuTabButton(parent, "Notifications", "Notifications");
		m_friends = new UIMenuTabButton(parent, "Friends", "Friends");
		m_me = new UIMenuTabButton(parent, "Me", "Me");
		new UIGameFeed(m_base);
		m_base.m_container.Update();
		for (int i = 0; i < CameraS.m_mainCamera.transform.childCount; i++)
		{
			Transform child = CameraS.m_mainCamera.transform.GetChild(i);
			child.gameObject.SetActive(true);
		}
	}

	public override void Execute()
	{
		if (m_gameFeed.m_hit)
		{
			m_base.DestroyChildren();
			new UIGameFeed(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
		else if (m_discover.m_hit)
		{
			m_base.DestroyChildren();
			new UIDiscover(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
		else if (m_pins.m_hit)
		{
			m_base.DestroyChildren();
			m_base.GoToPage(0, true);
			m_base.Update();
		}
		else if (m_notifications.m_hit)
		{
			m_base.DestroyChildren();
			new UINotifications(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
		else if (m_friends.m_hit)
		{
			m_base.DestroyChildren();
			new UIFriends(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
		else if (m_me.m_hit)
		{
			m_base.DestroyChildren();
			new UIProfile(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
	}

	public override void Exit()
	{
		m_base.Destroy();
	}
}
