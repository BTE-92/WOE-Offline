public class UIEditorMenu : UIHorizontalList
{
	public UIRectSpriteButton m_playButton;

	public UIRectSpriteButton m_fileMenuButton;

	public UIEditorFileMenu m_fileMenu;

	public UIEditorMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 1f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.03f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		m_playButton = new UIRectSpriteButton(this, "Play", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_play"), false);
		m_playButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
		m_playButton.SetVerticalAlign(1f);
		m_fileMenuButton = new UIRectSpriteButton(this, "Menu", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_menu"), false);
		m_fileMenuButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
		m_fileMenuButton.SetVerticalAlign(1f);
		Update();
	}

	public void OpenFileMenu()
	{
		if (m_fileMenuButton != null)
		{
			m_fileMenuButton.Destroy();
			m_fileMenuButton = null;
			m_fileMenu = new UIEditorFileMenu(this, "Menu");
			m_fileMenu.SetVerticalAlign(1f);
			Update();
		}
	}

	public void CloseFileMenu()
	{
		if (m_fileMenu != null)
		{
			m_fileMenu.Destroy();
			m_fileMenu = null;
			m_fileMenuButton = new UIRectSpriteButton(this, "Menu", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_menu"), false);
			m_fileMenuButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
			m_fileMenuButton.SetVerticalAlign(1f);
			Update();
		}
	}
}
