public interface IScene
{
	string m_name { get; set; }

	StateMachine m_stateMachine { get; set; }

	bool m_initComplete { get; }

	IState GetCurrentState();

	void Load();

	void Initialize();

	void Reset();

	void Update();

	void Destroy();
}
