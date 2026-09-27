public class StartupScene : IScene, IStatedObject
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

	public StartupScene(string _sceneName)
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
		PsState.m_gameState = GameState.Menu;
		m_stateMachine = new StateMachine();
		m_stateMachine.ChangeState(new StartupState());
		_initComplete = true;
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		m_stateMachine.Update();
	}

	public void Destroy()
	{
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~StartupScene()
	{
	}
}
