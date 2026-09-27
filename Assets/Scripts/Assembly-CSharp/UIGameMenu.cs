public class UIGameMenu : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_pauseButton;

	public UIRaceTimer m_timer;

	public UIGameMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 1f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.03f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(uIHorizontalList, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"), false);
		m_restartButton.SetHeight(0.07f, RelativeTo.ScreenShortest);
		m_restartButton.SetVerticalAlign(1f);
		m_pauseButton = new UIRectSpriteButton(uIHorizontalList, "Pause", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_menu"), false);
		m_pauseButton.SetHeight(0.07f, RelativeTo.ScreenShortest);
		m_pauseButton.SetVerticalAlign(1f);
		m_timer = new UIRaceTimer(this, "Timer");
		m_timer.SetHorizontalAlign(1f);
		Update();
	}
}
