public class UIRaceTimer : UITextbox
{
	public UIRaceTimer(UIComponent _parent, string _tag)
		: base(_parent, false, _tag, "00.000", "Fonts/HurmeSemiBoldMN", 0.05f, RelativeTo.ScreenShortest, false, Align.Right)
	{
	}

	public void SetTimeFromTicks(int _ticks)
	{
		string text = HighScores.TicksToTime(_ticks);
		SetText(text);
	}

	public void SetTimeFromScore(int _score)
	{
		string text = HighScores.ScoreToTime(_score);
		SetText(text);
	}

	public new void SetText(string _text)
	{
		m_text = _text;
		TextMeshS.SetTextOptimized(m_tmc, _text);
	}
}
