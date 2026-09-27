using UnityEngine;

public class StartupState : BasicState
{
	public bool m_startupComplete;

	private UICanvas m_area;

	public override void Enter(IStatedObject _parent)
	{
		UserLogin.Login();
		m_area = new UICanvas(null, "SelectorContainer", null, string.Empty);
		m_area.SetWidth(1f, RelativeTo.ScreenWidth);
		m_area.SetHeight(1f, RelativeTo.ScreenHeight);
		m_area.SetDepthOffset(100f);
		m_area.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_area, "HeaderArea");
		uIVerticalList.SetMargins(0.2f, 0.2f, 0f, 0f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "Header", "connecting to server...", "Please wait");
		m_area.Update();
	}

	public override void Execute()
	{
		if (!m_startupComplete)
		{
			switch (UserLogin.m_userLoginState)
			{
			case UserLoginState.LOGGED_IN:
				Main.m_currentGame.m_sceneManager.ChangeScene(new MenuScene("MenuScene"), new FadeLoadingScene(Color.black));
				m_startupComplete = true;
				break;
			case UserLoginState.NOT_LOGGED_IN:
				break;
			case UserLoginState.STARTED_LOGIN:
				break;
			}
		}
	}

	public override void Exit()
	{
		m_area.Destroy();
	}
}
