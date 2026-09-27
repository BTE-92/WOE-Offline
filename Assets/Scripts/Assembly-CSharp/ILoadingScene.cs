public interface ILoadingScene
{
	StateMachine StateMachine { get; set; }

	IScene FromScene { get; set; }

	IScene ToScene { get; set; }

	bool InitComplete { get; }

	void Load();

	void Initialize();

	void Update();

	void Destroy();

	void StartOutro();

	bool IntroComplete();

	bool OutroComplete();
}
