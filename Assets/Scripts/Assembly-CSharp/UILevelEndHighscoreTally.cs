public class UILevelEndHighscoreTally : UIVerticalList
{
	public UIScrollableCanvas m_scrollableHighscoreCanvas;

	public UIVerticalList m_highscoreList;

	public UILevelEndHighscoreTally(UIComponent _parent)
		: base(_parent, "highscoretally")
	{
		SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
		SetMargins(0.025f);
		UICanvas uICanvas = new UICanvas(this, "Header", null, string.Empty);
		uICanvas.SetMargins(0.025f);
		uICanvas.SetWidth(0.5f, RelativeTo.ScreenWidth);
		uICanvas.SetHeight(0.15f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		UIText uIText = new UIText(uICanvas, false, "Score", "<color=#80ff33>" + HighScores.ScoreToTime(PsState.m_lastSentScore) + "</color>", "Fonts/HurmeSemiBold", 0.08f, RelativeTo.ScreenHeight);
		uIText.SetHorizontalAlign(0.5f);
		UICanvas uICanvas2 = new UICanvas(this, "Content", null, string.Empty);
		uICanvas2.SetWidth(0.5f, RelativeTo.ScreenWidth);
		uICanvas2.SetHeight(0.7f, RelativeTo.ScreenHeight);
		uICanvas2.RemoveDrawHandler();
		uICanvas2.SetDrawHandler(UIDrawHandlers.ScrollableList);
		m_scrollableHighscoreCanvas = new UIScrollableCanvas(uICanvas2, "HighscoreScrollCanvas");
		m_scrollableHighscoreCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_scrollableHighscoreCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollableHighscoreCanvas.RemoveDrawHandler();
		m_highscoreList = new UIVerticalList(m_scrollableHighscoreCanvas, "HighscoreList");
		m_highscoreList.SetVerticalAlign(1f);
		m_highscoreList.RemoveDrawHandler();
		m_highscoreList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_highscoreList.SetSpacing(0.005f, RelativeTo.ScreenShortest);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_highscoreList, "Highscore Horizontal Area");
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetMargins(0.05f, 0.05f, 0f, 0f, RelativeTo.ScreenShortest);
		UITextbox uITextbox = new UITextbox(uIHorizontalList, false, "User", "<color=#68bafe>Player</color>", "Fonts/HurmeSemiBold", 0.04f, RelativeTo.ScreenHeight);
		uITextbox.SetWidth(0.5f, RelativeTo.ParentWidth);
		UITextbox uITextbox2 = new UITextbox(uIHorizontalList, false, "Score", "<color=#68bafe>Time</color>", "Fonts/HurmeSemiBold", 0.04f, RelativeTo.ScreenHeight, false, Align.Right);
		uITextbox2.SetWidth(0.5f, RelativeTo.ParentWidth);
	}

	public void GenerateContent(HighscoreData[] _highscoreData)
	{
		m_highscoreList.DestroyChildren(1);
		for (int i = 0; i < _highscoreData.Length; i++)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(m_highscoreList, "Highscore Horizontal Area");
			uIHorizontalList.SetVerticalAlign(1f);
			uIHorizontalList.SetMargins(0.05f, 0.05f, 0f, 0f, RelativeTo.ScreenShortest);
			uIHorizontalList.RemoveDrawHandler();
			string text = "<color=#ffffff>";
			if (_highscoreData[i].name == PlayerPrefsX.GetUserName())
			{
				text = "<color=#80ff33>";
			}
			UITextbox uITextbox = new UITextbox(uIHorizontalList, false, "User", text + (i + 1) + ". " + _highscoreData[i].name + "</color>", "Fonts/HurmeSemiBold", 0.035f, RelativeTo.ScreenHeight);
			uITextbox.SetWidth(0.5f, RelativeTo.ParentWidth);
			UITextbox uITextbox2 = new UITextbox(uIHorizontalList, false, "Score", text + HighScores.ScoreToTime(_highscoreData[i].score) + "</color>", "Fonts/HurmeSemiBoldMN", 0.035f, RelativeTo.ScreenHeight, false, Align.Right);
			uITextbox2.SetWidth(0.5f, RelativeTo.ParentWidth);
		}
		m_highscoreList.Update();
	}
}
