public class UILevelEndRewards : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_exitButton;

	public UIScrollableCanvas m_scrollableHighscoreCanvas;

	public UIVerticalList m_highscoreList;

	public UILevelEndRewards(UIComponent _parent)
		: base(_parent, "rewards")
	{
		RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(this, "Content", null, string.Empty);
		uICanvas.SetWidth(0.5f, RelativeTo.ScreenWidth);
		uICanvas.SetHeight(1f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(uICanvas, "varea");
		uIVerticalList.SetVerticalAlign(0.5f);
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		new UIText(uIVerticalList, false, string.Empty, "<color=#afed32>nicely done!</color>", "Fonts/KGLetHerGo", 0.08f, RelativeTo.ScreenHeight);
		UICanvas uICanvas2 = new UICanvas(uIVerticalList, "Footer", null, string.Empty);
		uICanvas2.SetMargins(0.025f);
		uICanvas2.SetWidth(0.5f, RelativeTo.ParentWidth);
		uICanvas2.SetHeight(0.15f, RelativeTo.ScreenHeight);
		uICanvas2.SetVerticalAlign(0f);
		uICanvas2.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(uICanvas2, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"));
		m_restartButton.SetHeight(0.1f, RelativeTo.ScreenHeight);
		m_restartButton.SetHorizontalAlign(0f);
		m_exitButton = new UIRectSpriteButton(uICanvas2, "Exit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_exit"));
		m_exitButton.SetHeight(0.1f, RelativeTo.ScreenHeight);
		m_exitButton.SetHorizontalAlign(1f);
	}
}
