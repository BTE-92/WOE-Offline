public class UILevelEndComments : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_exitButton;

	public UIScrollableCanvas m_scrollableHighscoreCanvas;

	public UIVerticalList m_highscoreList;

	public UILevelEndComments(UIComponent _parent)
		: base(_parent, "comments")
	{
		new UIText(this, false, string.Empty, "comments", "Fonts/KGLetHerGo", 0.08f, RelativeTo.ScreenHeight);
	}
}
