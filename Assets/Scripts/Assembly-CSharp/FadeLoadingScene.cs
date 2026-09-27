using UnityEngine;

public class FadeLoadingScene : ILoadingScene
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

	public SpriteSheet m_loadingScreenSpriteSheet;

	public Entity m_loadingScreenEntity;

	public TransformC m_loadingScreenTC;

	public Color m_fadeColor;

	public bool m_fadeIn;

	public float m_fadeDuration;

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

	public FadeLoadingScene(Color _fadeColor, bool _fadeIn = true, float _fadeDuration = 0.25f)
	{
		m_fadeColor = _fadeColor;
		m_fadeIn = _fadeIn;
		m_fadeDuration = _fadeDuration;
	}

	private void StartIntro()
	{
		if (m_fadeIn)
		{
			TweenC tweenComponent = TweenS.AddTransformTween(m_loadingScreenTC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, Vector3.one, m_fadeDuration, 0f);
			TweenS.AddTweenEventListener(tweenComponent, IntroTweenEventHandler);
		}
		else
		{
			m_introComplete = true;
		}
		m_introStarted = true;
	}

	public void StartOutro()
	{
		TweenC tweenComponent = TweenS.AddTransformTween(m_loadingScreenTC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.one, Vector3.zero, m_fadeDuration, 0f);
		TweenS.AddTweenEventListener(tweenComponent, OutroTweenEventHandler);
		m_outroStarted = true;
	}

	public void Load()
	{
		m_camera = CameraS.AddCamera("Loading Screen Camera", true);
		m_loadingScreenSpriteSheet = SpriteS.AddSpriteSheet(m_camera, ResourceManager.GetMaterial("UI/LoadingScreenFadeMat"), 1f);
		Initialize();
	}

	public void Initialize()
	{
		m_loadingScreenEntity = EntityManager.AddEntity("FadeLoadingScreenEntity");
		m_loadingScreenEntity.m_persistent = true;
		m_loadingScreenTC = TransformS.AddComponent(m_loadingScreenEntity, "FadeLoadingScreenTC", Vector3.forward * -240f);
		SpriteC sprite = SpriteS.AddComponent(m_loadingScreenTC, new Frame(0f, 0f, Screen.width, Screen.height), m_loadingScreenSpriteSheet);
		SpriteS.SetColor(sprite, new Color(m_fadeColor.r, m_fadeColor.g, m_fadeColor.b, (!m_fadeIn) ? 1f : 0f));
		m_initComplete = true;
		StartIntro();
	}

	private void IntroTweenEventHandler(TweenC _c)
	{
		m_introComplete = true;
		TweenS.RemoveTweenEventListener(_c, IntroTweenEventHandler);
		TweenS.RemoveComponent(_c);
	}

	private void OutroTweenEventHandler(TweenC _c)
	{
		m_outroComplete = true;
		TweenS.RemoveTweenEventListener(_c, OutroTweenEventHandler);
		TweenS.RemoveComponent(_c);
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		CameraS.RemoveCamera(m_camera);
		EntityManager.RemoveEntity(m_loadingScreenEntity);
		m_loadingScreenEntity = null;
		m_loadingScreenTC = null;
		SpriteS.RemoveSpriteSheet(m_loadingScreenSpriteSheet);
		m_loadingScreenSpriteSheet = null;
	}

	public bool IntroComplete()
	{
		return m_introComplete;
	}

	public bool OutroComplete()
	{
		return m_outroComplete;
	}

	~FadeLoadingScene()
	{
	}
}
