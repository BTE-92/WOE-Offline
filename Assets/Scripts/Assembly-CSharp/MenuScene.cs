public class MenuScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine;

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

	public MenuScene(string _sceneName)
	{
		m_name = _sceneName;
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
		SoundS.SetMute(PsState.m_mute);
		PsState.m_gameState = GameState.Menu;
		m_stateMachine = new StateMachine();
		m_stateMachine.ChangeState(new GDCMenuState());
		_initComplete = true;
		CacheManager.RemoveAll();
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
	}

	public void Destroy()
	{
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~MenuScene()
	{
	}
}
