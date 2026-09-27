using System.Collections.Generic;

public class UIMinigameSummary : UIScrollableCanvas
{
	protected UIVerticalList m_gameCardArea;

	protected UIVerticalList m_highscoreArea;

	private MinigameMetaData m_minigameMetaData;

	private UITextButton m_backButton;

	private UITextButton m_playButton;

	private bool m_destroyAfterPageChange;

	public UIMinigameSummary(UIPagedCanvas _parent, MinigameMetaData _minigameMetaData)
		: base(_parent, "MinigameCard")
	{
		m_minigameMetaData = _minigameMetaData;
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(0.5f, 1f);
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Maincontainer");
		uIVerticalList.SetAlign(0.5f, 1f);
		UIHidingVerticalListBar parent = new UIHidingVerticalListBar(uIVerticalList, string.Empty);
		m_backButton = new UITextButton(parent, "BackButton", "Back", "Fonts/HurmeRegular", 0.05f);
		m_backButton.SetHorizontalAlign(0f);
		m_gameCardArea = new UIVerticalList(uIVerticalList, "Minigame Vertical Area");
		m_gameCardArea.SetMargins(0.025f, 0.025f, 0.075f, 0.075f, RelativeTo.ScreenShortest);
		m_playButton = new UITextButton(m_gameCardArea, "PlayButton", "Play", "Fonts/HurmeRegular", 0.05f);
		new UIMinigameCard(m_gameCardArea, "tets", _minigameMetaData);
		m_highscoreArea = new UIVerticalList(uIVerticalList, "Highscore Vertical Area");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_highscoreArea, "Highscore Horizontal Area");
		uIHorizontalList.SetVerticalAlign(1f);
		UILabel uILabel = new UILabel(uIHorizontalList, "User", "Player", Align.Left, Align.Center);
		uILabel.SetWidth(0.5f, RelativeTo.ParentWidth);
		UILabel uILabel2 = new UILabel(uIHorizontalList, "Score", "Score", Align.Right, Align.Center);
		uILabel2.SetWidth(0.5f, RelativeTo.ParentWidth);
	}

	public override void Step()
	{
		UIPagedCanvas uIPagedCanvas = m_parent as UIPagedCanvas;
		if (m_playButton.m_hit)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MinigameLoadState(m_minigameMetaData.id));
		}
		else if (m_backButton.m_hit)
		{
			uIPagedCanvas.PreviousPage();
			m_destroyAfterPageChange = true;
		}
		if (!uIPagedCanvas.IsChangingPage() && m_destroyAfterPageChange)
		{
			Destroy();
			uIPagedCanvas.Update();
		}
		else
		{
			base.Step();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		WWWRequestManager.RemoveRequestsWithTag("FIND_HIGHSCORES");
	}

	public override void Focus()
	{
		LoadContent();
	}

	private void LoadContent()
	{
		WWWRequest wWWRequest = null;
		Cache<HighscoreData> cache = CacheManager.GetCache("HIGHSCORE_CACHE_" + m_minigameMetaData.id) as Cache<HighscoreData>;
		if (cache == null)
		{
			wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/highscore/find?gameId=" + m_minigameMetaData.id, "FIND_HIGHSCORES", false, 5f);
		}
		else
		{
			GenerateContent(cache);
		}
		if (wWWRequest != null)
		{
			wWWRequest.requestComplete += ServerRequestOK;
			wWWRequest.requestFailed += ServerRequestFAILED;
		}
	}

	private void GenerateContent(Cache<HighscoreData> _cache)
	{
		m_highscoreArea.DestroyChildren(1);
		HighscoreData[] objects = _cache.GetObjects();
		for (int i = 0; i < objects.Length; i++)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(m_highscoreArea, "Highscore Horizontal Area");
			uIHorizontalList.SetVerticalAlign(1f);
			UILabel uILabel = new UILabel(uIHorizontalList, "User", i + 1 + ". " + objects[i].name, Align.Left, Align.Center);
			uILabel.SetWidth(0.5f, RelativeTo.ParentWidth);
			UILabel uILabel2 = new UILabel(uIHorizontalList, "Score", HighScores.ScoreToTime(objects[i].score), Align.Right, Align.Center);
			uILabel2.SetWidth(0.5f, RelativeTo.ParentWidth);
		}
		Update();
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Cache<HighscoreData> cache = new Cache<HighscoreData>("HIGHSCORE_CACHE_" + m_minigameMetaData.id, 60f);
			cache.AddToHead(HighScores.ParseHighscoreJSON(dictionary));
			CacheManager.AddCache(cache);
			GenerateContent(cache);
		}
		else
		{
			Debug.Log("Highscore Server Error");
		}
	}

	private void ServerRequestFAILED(WWWRequest _request)
	{
		Debug.LogError("Highscore Download Failed");
	}
}
