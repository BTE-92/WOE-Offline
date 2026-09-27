public interface IGame
{
	string m_projectCode { get; set; }

	string m_projectVersion { get; set; }

	IScene m_currentScene { get; set; }

	SceneManager m_sceneManager { get; set; }

	void RemoveComponent(IComponent _c);

	void Initialize(IScene _scene);

	void Update();
}
