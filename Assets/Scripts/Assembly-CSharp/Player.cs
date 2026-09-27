public static class Player
{
	public static bool m_isSet;

	public static Controller m_controller;

	public static TransformC m_mainTransform;

	public static bool m_gameIsStarted;

	public static void Set(Controller _controller, TransformC _mainTransform)
	{
		if (!m_isSet)
		{
			m_mainTransform = _mainTransform;
			m_controller = _controller;
			m_isSet = true;
			m_gameIsStarted = false;
		}
	}

	public static void StartRun()
	{
		if (!PsState.m_gameStarted)
		{
			PsState.m_playerReachedGoal = false;
			if (Main.m_currentGame.m_currentScene.m_name == "GameScene")
			{
				m_gameIsStarted = true;
				GameScene.StartGame();
			}
			else if (Main.m_currentGame.m_currentScene.m_name == "EditorScene")
			{
				m_gameIsStarted = true;
				EditorScene.StartGame();
			}
		}
	}

	public static void StopRun()
	{
		if (PsState.m_playerReachedGoal && PsState.m_gameTicks < PsState.m_sessionBestTime)
		{
			PsState.m_sessionBestTime = PsState.m_gameTicks;
		}
		if (PsState.m_gameTicks > PsState.m_sessionLongestRunTicks)
		{
			PsState.m_sessionLongestRunTicks = PsState.m_gameTicks;
		}
	}

	public static void OpenController()
	{
		if (m_controller != null && !m_controller.m_open)
		{
			m_controller.Open();
		}
	}

	public static void CloseController()
	{
		if (m_controller != null && m_controller.m_open)
		{
			m_controller.Close();
		}
	}

	public static void SetAsAudioListener()
	{
		if (m_mainTransform != null)
		{
			SoundS.SetListener(m_mainTransform.transform.gameObject);
		}
	}

	public static void Remove()
	{
		if (m_isSet)
		{
			SoundS.SetListener(CameraS.m_mainCamera.gameObject);
			CloseController();
			m_controller = null;
			m_mainTransform = null;
			m_isSet = false;
		}
	}
}
