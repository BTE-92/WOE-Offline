public class GamePlayState : BasicState
{
	public UIGameMenu m_gameMenu;

	public override void Enter(IStatedObject _parent)
	{
		Player.OpenController();
		m_gameMenu = new UIGameMenu(null, "GameMenu");
		m_gameMenu.SetAlign(1f, 1f);
	}

	public override void Execute()
	{
		if (!PsState.m_gamePaused && PsState.m_gameStarted && !PsState.m_gameEnded)
		{
			PsState.m_gameTicks++;
			m_gameMenu.m_timer.SetTimeFromTicks(PsState.m_gameTicks);
		}
		if (m_gameMenu.m_restartButton.m_hit)
		{
			GameScene.ResetGame();
			Player.OpenController();
			Player.SetAsAudioListener();
		}
		else if (m_gameMenu.m_pauseButton.m_hit)
		{
			GameScene.PauseGame();
		}
	}

	public override void Exit()
	{
		m_gameMenu.Destroy();
	}
}
