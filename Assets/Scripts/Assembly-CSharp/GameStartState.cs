public class GameStartState : BasicState
{
	public UIGameMenu m_gameMenu;

	private bool m_cameraIsSet;

	public override void Enter(IStatedObject _parent)
	{
		Player.OpenController();
		m_gameMenu = new UIGameMenu(null, "GameMenu");
		m_gameMenu.SetAlign(1f, 1f);
	}

	public override void Execute()
	{
		if (!m_cameraIsSet)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player");
			if (element != null)
			{
				CameraS.ResetMainCamera(element.m_position, 500f);
				m_cameraIsSet = true;
			}
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
