using UnityEngine;

public class GameScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine = new StateMachine();

	private bool _initComplete;

	private int mainTicker;

	public string m_name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public StateMachine m_stateMachine
	{
		get
		{
			return _stateMachine;
		}
		set
		{
			_stateMachine = value;
		}
	}

	public bool m_initComplete
	{
		get
		{
			return _initComplete;
		}
	}

	public GameScene(string _sceneName)
	{
		m_name = _sceneName;
		PsState.m_lastSentScore = 0;
		PsState.m_editorPaused = false;
	}

	public static void StartGame()
	{
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		PsState.m_gameStarted = true;
		PsState.m_gamePaused = false;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
	}

	public static void PauseGame()
	{
		SoundS.SetMute(true);
		PsState.m_gamePaused = true;
		EntityManager.SetActivityOfEntitiesWithTag("GTAG_INGAME_PARTICLES", false, false);
		EntityManager.SetActivityOfEntitiesWithTag("GTAG_UNIT", false, false);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePauseState());
	}

	public static void ResumeGame()
	{
		SoundS.SetMute(PsState.m_mute);
		EntityManager.SetActivityOfEntitiesWithTag("GTAG_INGAME_PARTICLES", true, false);
		EntityManager.SetActivityOfEntitiesWithTag("GTAG_UNIT", true, false);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
		PsState.m_gamePaused = false;
	}

	public static void InitGame()
	{
		SoundS.SetMute(PsState.m_mute);
		Player.SetAsAudioListener();
		PsState.m_gameTicks = 0;
		PsState.m_gameEnded = false;
		PsState.m_gameStarted = false;
		PsState.m_gamePaused = true;
		ChipmunkProWrapper.ucpSpaceReindexStatic();
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
	}

	public static void ResetGame()
	{
		EntityManager.RemoveEntitiesByTag("GTAG_INGAME_PARTICLES");
		Player.Remove();
		(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitGame();
	}

	public static void WinGame()
	{
		PsState.m_gameEnded = true;
		CacheManager.RemoveCache("HIGHSCORE_CACHE_" + (LevelManager.m_currentLevel as Minigame).m_minigameId);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameWinState());
	}

	public static void LoseGame()
	{
		PsState.m_gameEnded = true;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameLoseState());
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		Initialize();
	}

	public void Reset()
	{
	}

	public void Initialize()
	{
		PsState.m_gameState = GameState.Play;
		if (PsState.m_lastDownloadedLevelBytesZipped != null)
		{
			LevelManager.LoadLevel(FilePacker.UnZipBytes(PsState.m_lastDownloadedLevelBytesZipped));
		}
		if (LevelManager.m_currentLevel != null)
		{
			Minigame minigame = LevelManager.m_currentLevel as Minigame;
			minigame.m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
			minigame.m_minigameId = PsState.m_lastDownloadedLevelId;
			minigame.ApplySettings();
		}
		else
		{
			DefaultMinigame.Assemble();
		}
		InitGame();
		_initComplete = true;
	}

	public void Update()
	{
		mainTicker++;
		m_stateMachine.Update();
		AutoGeometryManager.Update();
	}

	public void Destroy()
	{
		Player.Remove();
		LevelManager.DestroyCurrentLevel();
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~GameScene()
	{
	}
}
