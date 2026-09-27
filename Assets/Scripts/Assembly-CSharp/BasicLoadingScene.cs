using UnityEngine;

public class BasicLoadingScene : ILoadingScene
{
	private StateMachine m_stateMachine;

	private IScene m_fromScene;

	private IScene m_toScene;

	private bool m_initComplete;

	private bool m_introComplete;

	private bool m_outroComplete;

	private bool m_introStarted;

	private bool m_outroStarted;

	public Camera m_camera;

	public GameObject m_prefab;

	private int sticker;

	private int ticker;

	public StateMachine StateMachine
	{
		get
		{
			return m_stateMachine;
		}
		set
		{
			m_stateMachine = value;
		}
	}

	public IScene FromScene
	{
		get
		{
			return m_fromScene;
		}
		set
		{
			m_fromScene = value;
		}
	}

	public IScene ToScene
	{
		get
		{
			return m_toScene;
		}
		set
		{
			m_toScene = value;
		}
	}

	public bool InitComplete
	{
		get
		{
			return m_initComplete;
		}
	}

	private void StartIntro()
	{
		Debug.LogInfo("LOADING SCREEN: Intro started");
		m_initComplete = true;
		m_introStarted = true;
		sticker = ticker;
	}

	public void StartOutro()
	{
		m_outroStarted = true;
		sticker = ticker;
		Debug.LogInfo("LOADING SCREEN: Outro started");
	}

	public void Load()
	{
		Debug.LogInfo("LOADING SCREEN: Loading assets");
		ResourceManager.AddResourceGroup("LoadingScreen");
		ResourceManager.AddResourceToGroup("LoadingScreen", new UnityResource("LoadingScreenPrefab", "LoadScreenPrefab"));
		Initialize();
	}

	public void Initialize()
	{
		m_camera = CameraS.AddCamera("Loading Screen Camera", true);
		m_prefab = Object.Instantiate(ResourceManager.GetGameObject("LoadingScreenPrefab")) as GameObject;
		m_prefab.layer = m_camera.gameObject.layer;
		m_prefab.transform.localScale = new Vector3(1f, 1f, 1f) * Screen.height * 0.02f;
		m_prefab.transform.position = m_camera.transform.position + new Vector3(500f, 0f, 100f);
		StartIntro();
	}

	public void Update()
	{
		ticker++;
		if (!m_introComplete && m_introStarted)
		{
			float num = 1f - ToolBox.getPositionBetween(ticker, sticker, sticker + 30);
			m_prefab.transform.position = m_camera.transform.position + new Vector3(1000f * num, 0f, 100f);
			if (num == 0f)
			{
				m_introComplete = true;
			}
		}
		if (!m_outroComplete && m_outroStarted)
		{
			float positionBetween = ToolBox.getPositionBetween(ticker, sticker, sticker + 30);
			m_prefab.transform.position = m_camera.transform.position + new Vector3(1000f * positionBetween, 0f, 100f);
			if (positionBetween == 1f)
			{
				m_outroComplete = true;
			}
		}
		m_prefab.transform.Rotate(new Vector3(0f, 10f, 0f));
	}

	public void Destroy()
	{
		CameraS.RemoveCamera(m_camera);
		Object.DestroyImmediate(m_prefab);
		ResourceManager.UnloadResourceGroup("LoadingScreen");
		Debug.LogInfo("LOADING SCREEN: Destroy");
	}

	public bool IntroComplete()
	{
		return m_introComplete;
	}

	public bool OutroComplete()
	{
		return m_outroComplete;
	}

	~BasicLoadingScene()
	{
		Debug.Log(string.Concat(this, ": Memory Freed"));
	}
}
