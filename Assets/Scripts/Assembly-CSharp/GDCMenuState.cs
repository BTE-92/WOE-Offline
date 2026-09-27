using UnityEngine;

public class GDCMenuState : BasicState
{
	public UIGDCMenuBase m_base;

	public UIMenuTabButton m_editor;

	public UIMenuTabButton m_me;

	public string m_currentTab;

	private Entity m_bgEntity;

	public override void Enter(IStatedObject _parent)
	{
		m_base = new UIGDCMenuBase(null, "Base");
		UIVerticalList uIVerticalList = new UIVerticalList(m_base.m_right, "RightMenu");
		uIVerticalList.SetVerticalAlign(1f);
		m_editor = new UIMenuTabButton(uIVerticalList, "Editor", "Create Game");
		m_me = new UIMenuTabButton(uIVerticalList, "Me", "Me");
		UIGDCDiscover uIGDCDiscover = new UIGDCDiscover(m_base);
		uIGDCDiscover.Focus();
		m_bgEntity = EntityManager.AddEntity();
		TransformC tc = TransformS.AddComponent(m_bgEntity);
		float num = Mathf.Max(Screen.width, Screen.height);
		PrefabS.CreateRect(tc, Vector3.forward * 100f, num, num, Color.white, ResourceManager.GetMaterial("Desert/DesertBackgroundMat"), CameraS.m_uiCamera);
		m_base.m_container.Update();
		for (int i = 0; i < CameraS.m_mainCamera.transform.childCount; i++)
		{
			Transform child = CameraS.m_mainCamera.transform.GetChild(i);
			child.gameObject.SetActive(true);
		}
	}

	public override void Execute()
	{
		if (m_editor.m_hit)
		{
			PsState.m_lastDownloadedLevelBytesZipped = null;
			PsState.m_lastDownloadedLevelId = null;
			Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"), new FadeLoadingScene(Color.black));
		}
		else if (m_me.m_hit)
		{
			m_base.DestroyChildren();
			new UIGDCProfile(m_base);
			m_base.GoToPage(0, true);
			m_base.Update();
		}
	}

	public override void Exit()
	{
		m_base.Destroy();
		EntityManager.RemoveEntity(m_bgEntity);
	}
}
