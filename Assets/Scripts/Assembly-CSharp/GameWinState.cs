using System.Collections.Generic;
using UnityEngine;

public class GameWinState : BasicState
{
	public UILevelEndHighscoreTally m_highscoreTally;

	public UILevelEndRewards m_rewards;

	public UILevelEndRecommendations m_recommendations;

	public UICanvas m_buttonArea;

	private bool m_changingScene;

	private string m_highscoreRequest;

	private UIScrollableCanvas m_scrollableCanvas;

	private UIHorizontalList m_horizontalList;

	private Entity m_particleE;

	private HighscoreData[] m_highscoredata;

	private bool m_printHighscores;

	public override void Enter(IStatedObject _parent)
	{
		Player.CloseController();
		PsState.m_lastSentScore = HighScores.TicksToScore(PsState.m_gameTicks, true);
		Entity entity = EntityManager.AddEntity();
		TimerC timerC = TimerS.AddComponent(entity, "windelay", 1f, 0f, true, DelayedEnter);
		m_changingScene = true;
		m_printHighscores = false;
		m_highscoredata = null;
		SendHighscores();
	}

	private void DelayedEnter(TimerC _c)
	{
		m_particleE = EntityManager.AddEntity();
		TransformC transformC = TransformS.AddComponent(m_particleE);
		TransformS.SetPosition(transformC, Vector3.up * Screen.height * 0.6f);
		TransformS.SetRotation(transformC, Vector3.right * 270f);
		PrefabC c = PrefabS.AddComponent(transformC, Vector3.zero, ResourceManager.GetGameObject("ParticleFx/ConfettiRain"));
		PrefabS.SetCamera(c, CameraS.m_uiCamera);
		m_scrollableCanvas = new UIScrollableCanvas(null, "scrollableCanvas");
		m_scrollableCanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		m_scrollableCanvas.SetHeight(1f, RelativeTo.ScreenHeight);
		m_scrollableCanvas.SetVerticalAlign(1f);
		m_scrollableCanvas.m_maxScrollInertialX = 200f;
		m_scrollableCanvas.m_maxScrollInertialY = 0f;
		m_scrollableCanvas.SetScrollPosition(0.5f, 0f);
		m_scrollableCanvas.RemoveDrawHandler();
		m_horizontalList = new UIHorizontalList(m_scrollableCanvas, "hlist");
		m_horizontalList.SetSpacing(0.2f, RelativeTo.ScreenHeight);
		m_horizontalList.SetMargins(0.2f, 0.2f, 0f, 0f);
		m_horizontalList.SetVerticalAlign(0f);
		m_horizontalList.RemoveDrawHandler();
		m_highscoreTally = new UILevelEndHighscoreTally(m_horizontalList);
		m_rewards = new UILevelEndRewards(m_horizontalList);
		m_recommendations = new UILevelEndRecommendations(m_horizontalList);
		m_changingScene = false;
		m_scrollableCanvas.Update();
		m_printHighscores = true;
		if (m_highscoredata != null)
		{
			m_highscoreTally.GenerateContent(m_highscoredata);
		}
	}

	private void SendHighscores()
	{
		int starts = 1;
		m_highscoreRequest = HighScores.SendHighscore(PsState.m_lastSentScore, starts, (LevelManager.m_currentLevel as Minigame).m_minigameId, true, ServerRequestOK, ServerRequestFAILED);
	}

	private void ServerRequestOK(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			m_highscoredata = HighScores.ParseHighscoreJSON(dictionary);
			if (m_printHighscores)
			{
				m_highscoreTally.GenerateContent(m_highscoredata);
			}
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

	public override void Execute()
	{
		if (!m_changingScene)
		{
			if (m_rewards.m_restartButton.m_hit || Input.GetKeyDown(KeyCode.R))
			{
				m_changingScene = true;
				GameScene.ResetGame();
			}
			else if (m_rewards.m_exitButton.m_hit)
			{
				m_changingScene = true;
				Main.m_currentGame.m_sceneManager.ChangeScene(new MenuScene("MenuScene"), new FadeLoadingScene(Color.black));
			}
		}
	}

	public override void Exit()
	{
		PostRequestQueue.RemoveCustomEventHandlersFromPostRequest(m_highscoreRequest);
		m_scrollableCanvas.Destroy();
		EntityManager.RemoveEntity(m_particleE);
	}
}
