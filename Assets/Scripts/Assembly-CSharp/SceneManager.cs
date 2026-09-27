using System;
using UnityEngine;

public class SceneManager
{
	public IScene m_currentScene;

	public IScene m_changeToScene;

	public ILoadingScene m_loadingScene;

	public bool m_sceneChanged;

	public bool m_currentSceneRemoved;

	public void ChangeScene(IScene _scene, ILoadingScene _loadingScene = null)
	{
		m_changeToScene = _scene;
		m_loadingScene = _loadingScene;
		if (m_loadingScene != null)
		{
			m_loadingScene.ToScene = m_changeToScene;
			m_loadingScene.FromScene = m_currentScene;
		}
		m_currentSceneRemoved = false;
		m_sceneChanged = false;
		Main.m_currentGame.m_currentScene = _scene;
	}

	public IScene GetCurrentScene()
	{
		return m_currentScene;
	}

	private void DestroyCurrentScene()
	{
		if (m_currentScene != null)
		{
			m_currentScene.Destroy();
			m_currentScene = null;
			EntityManager.Update();
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	public void UpdateLogic()
	{
		if (m_changeToScene != null)
		{
			if (m_loadingScene != null)
			{
				m_loadingScene.Load();
				m_changeToScene = null;
			}
			else
			{
				DestroyCurrentScene();
				m_currentScene = m_changeToScene;
				m_currentScene.Load();
				m_changeToScene = null;
			}
		}
		if (m_loadingScene != null && m_loadingScene.InitComplete)
		{
			if (!m_sceneChanged)
			{
				if (m_loadingScene.IntroComplete() && !m_currentSceneRemoved)
				{
					DestroyCurrentScene();
					m_loadingScene.ToScene.Load();
					m_currentSceneRemoved = true;
				}
				if (m_currentSceneRemoved && m_loadingScene.ToScene.m_initComplete)
				{
					m_currentScene = m_loadingScene.ToScene;
					m_sceneChanged = true;
					m_loadingScene.StartOutro();
				}
			}
			m_loadingScene.Update();
			if (m_loadingScene.OutroComplete())
			{
				m_loadingScene.Destroy();
				m_loadingScene = null;
			}
		}
		if (m_currentScene != null && m_currentScene.m_initComplete)
		{
			m_currentScene.Update();
		}
	}
}
