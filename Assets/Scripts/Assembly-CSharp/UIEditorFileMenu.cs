public class UIEditorFileMenu : UIVerticalList
{
	public UIRectSpriteButton m_menuButton;

	public UICanvas m_newButton;

	public UICanvas m_playerButton; // WOE TEST: opens the player unit selector (it had no button)

	public UICanvas m_saveButton;

	public UICanvas m_publishButton;

	public UICanvas m_exitButton;

	public UIEditorFileMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetSpacing(-0.02f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		m_menuButton = new UIRectSpriteButton(this, "MenuButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_menu"), false);
		m_menuButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
		m_menuButton.RemoveTouchAreas();
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0f, 0f, 0.04f, 0.04f, RelativeTo.ScreenShortest);
		uIVerticalList.SetSpacing(0.015f, RelativeTo.ScreenShortest);
		uIVerticalList.SetDrawHandler(UIDrawHandlers.EditorFileMenuContentArea);
		m_newButton = new UICanvas(uIVerticalList, string.Empty, null, string.Empty);
		m_newButton.RemoveDrawHandler();
		m_newButton.SetSize(0.11f, 0.11f, RelativeTo.ScreenShortest);
		m_newButton.SetMargins(0.01f, RelativeTo.ScreenShortest);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_newButton, false, "NewButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_new"), false);
		uIFittedSprite.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
		uIFittedSprite.SetVerticalAlign(1f);
		UITextbox uITextbox = new UITextbox(m_newButton, false, string.Empty, "NEW", "Fonts/HurmeRegular", 0.02f, RelativeTo.ScreenShortest, true, Align.Center, Align.Middle);
		uITextbox.SetVerticalAlign(0f);
		// WOE TEST: PLAYER button (the "hud_icon_player" sprite and EditorSelectorPlayer already existed but nothing used them)
		m_playerButton = new UICanvas(uIVerticalList, string.Empty, null, string.Empty);
		m_playerButton.RemoveDrawHandler();
		m_playerButton.SetSize(0.11f, 0.11f, RelativeTo.ScreenShortest);
		m_playerButton.SetMargins(0.01f, RelativeTo.ScreenShortest);
		UIFittedSprite woePlayerSprite = new UIFittedSprite(m_playerButton, false, "PlayerButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_player"), false);
		woePlayerSprite.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
		woePlayerSprite.SetVerticalAlign(1f);
		UITextbox woePlayerText = new UITextbox(m_playerButton, false, string.Empty, "PLAYER", "Fonts/HurmeRegular", 0.02f, RelativeTo.ScreenShortest, true, Align.Center, Align.Middle);
		woePlayerText.SetVerticalAlign(0f);
		m_saveButton = new UICanvas(uIVerticalList, string.Empty, null, string.Empty);
		m_saveButton.RemoveDrawHandler();
		m_saveButton.SetSize(0.11f, 0.11f, RelativeTo.ScreenShortest);
		m_saveButton.SetMargins(0.01f, RelativeTo.ScreenShortest);
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(m_saveButton, false, "SaveButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_save"), false);
		uIFittedSprite2.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
		uIFittedSprite2.SetVerticalAlign(1f);
		UITextbox uITextbox2 = new UITextbox(m_saveButton, false, string.Empty, "SAVE", "Fonts/HurmeRegular", 0.02f, RelativeTo.ScreenShortest, true, Align.Center, Align.Middle);
		uITextbox2.SetVerticalAlign(0f);
		m_exitButton = new UICanvas(uIVerticalList, string.Empty, null, string.Empty);
		m_exitButton.RemoveDrawHandler();
		m_exitButton.SetSize(0.11f, 0.11f, RelativeTo.ScreenShortest);
		m_exitButton.SetMargins(0.01f, RelativeTo.ScreenShortest);
		UIFittedSprite uIFittedSprite3 = new UIFittedSprite(m_exitButton, false, "ExitButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_exit"), false);
		uIFittedSprite3.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
		uIFittedSprite3.SetVerticalAlign(1f);
		UITextbox uITextbox3 = new UITextbox(m_exitButton, false, string.Empty, "EXIT", "Fonts/HurmeRegular", 0.02f, RelativeTo.ScreenShortest, true, Align.Center, Align.Middle);
		uITextbox3.SetVerticalAlign(0f);
	}
}
