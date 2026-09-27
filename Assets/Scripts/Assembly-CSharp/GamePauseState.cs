using UnityEngine;

public class GamePauseState : BasicState
{
	public UIHorizontalList m_area;

	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_resumeButton;

	public UIRectSpriteButton m_exitButton;

	private bool m_changingScene;

	public override void Enter(IStatedObject _parent)
	{
		CameraS.m_updateComponents = false;
		Player.CloseController();
		m_area = new UIHorizontalList(null, string.Empty);
		m_area.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		m_area.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(m_area, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"), false);
		m_restartButton.SetHeight(0.14f, RelativeTo.ScreenShortest);
		m_restartButton.SetVerticalAlign(1f);
		m_resumeButton = new UIRectSpriteButton(m_area, "Resume", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_play"), false);
		m_resumeButton.SetHeight(0.14f, RelativeTo.ScreenShortest);
		m_resumeButton.SetVerticalAlign(1f);
		m_exitButton = new UIRectSpriteButton(m_area, "Exit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_exit"), false);
		m_exitButton.SetHeight(0.14f, RelativeTo.ScreenShortest);
		m_exitButton.SetVerticalAlign(1f);
		m_area.Update();
	}

	public override void Execute()
	{
		if (m_changingScene)
		{
			return;
		}
		if (m_restartButton.m_hit)
		{
			m_changingScene = true;
			GameScene.ResetGame();
		}
		else if (m_resumeButton.m_hit)
		{
			m_changingScene = true;
			if (PsState.m_gameStarted)
			{
				GameScene.ResumeGame();
			}
			else
			{
				GameScene.ResetGame();
			}
		}
		else if (m_exitButton.m_hit)
		{
			m_changingScene = true;
			Main.m_currentGame.m_sceneManager.ChangeScene(new MenuScene("MenuScene"), new FadeLoadingScene(Color.black));
		}
	}

	public override void Exit()
	{
		m_area.Destroy();
		CameraS.m_updateComponents = true;
	}
}
