public class EditorScene : IScene, IStatedObject
{
	public const int AG_BRUSH_COUNT = 4;

	public string _name;

	private StateMachine _stateMachine = new StateMachine();

	private bool _initComplete;

	public static AutoGeometryBrush[] m_agBrush = new AutoGeometryBrush[4];

	public static AutoGeometryBrush m_tireSkidBrush;

	public static AutoGeometryBrush m_edgeBrush;

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

	public EditorScene(string _sceneName)
	{
		m_name = _sceneName;
		PsState.m_gamePaused = false;
	}

	public static void StartGame()
	{
		PsState.m_gameStarted = true;
		PsState.m_editorPaused = false;
	}

	public static void InitGame()
	{
		PsState.m_gameTicks = 0;
		PsState.m_gameEnded = false;
		PsState.m_gameStarted = false;
		PsState.m_editorPaused = true;
		ChipmunkProWrapper.ucpSpaceReindexStatic();
	}

	public static void ResetGame()
	{
		Player.Remove();
		EntityManager.RemoveEntitiesByTag("GTAG_INGAME_PARTICLES");
		(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitGame();
	}

	public static void WinGame()
	{
		PsState.m_gameEnded = true;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorTestEndState());
		Screenshot component = CameraS.m_mainCamera.gameObject.GetComponent<Screenshot>();
		component.TakeScreenshot();
	}

	public static void LoseGame()
	{
		PsState.m_gameEnded = true;
		ResetGame();
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		Initialize();
	}

	public void Initialize()
	{
		PsState.m_gameState = GameState.Edit;
		m_agBrush[0] = AutoGeometryManager.AddBrush("Autogeometry/RoundBrush3", true);
		m_agBrush[1] = AutoGeometryManager.AddBrush("Autogeometry/RoundBrush8");
		m_agBrush[2] = AutoGeometryManager.AddBrush("Autogeometry/RoundBrush16");
		m_agBrush[3] = AutoGeometryManager.AddBrush("Autogeometry/SquareBrush24");
		m_edgeBrush = AutoGeometryManager.AddBrush("Autogeometry/EdgeBrushRound", false, false);
		m_tireSkidBrush = AutoGeometryManager.AddBrush("Autogeometry/TireSkidBrush");
		m_stateMachine = new StateMachine();
		m_stateMachine.ChangeState(new EditorBaseState());
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
		_initComplete = true;
	}

	public void Reset()
	{
		Destroy();
		Load();
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
		AutoGeometryManager.DestroyAllBrushes();
		LevelManager.DestroyCurrentLevel();
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~EditorScene()
	{
	}
}
