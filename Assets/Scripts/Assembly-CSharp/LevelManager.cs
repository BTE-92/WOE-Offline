public static class LevelManager
{
	public static Level m_currentLevel;

	public static uint m_uniqueID;

	public static uint GetUniqueId()
	{
		return ++m_uniqueID;
	}

	public static void LoadLevel(byte[] _bytes)
	{
		m_uniqueID = 0u;
		if (m_currentLevel != null)
		{
			DestroyCurrentLevel();
		}
		EntityManager.Update();
		Level level = LevelSerializer.DeSerializeLevelFromBytes(_bytes);
		if (level != null)
		{
			m_currentLevel = level;
			m_currentLevel.m_currentLayer = m_currentLevel.m_layers[0];
			m_currentLevel.m_currentLayerIndex = 0;
			AssembleCurrentLevel();
		}
	}

	public static void LoadLevel(string _fileName)
	{
		m_uniqueID = 0u;
		if (m_currentLevel != null)
		{
			DestroyCurrentLevel();
		}
		EntityManager.Update();
		Level level = LevelSerializer.DeSerializeLevelFromFile(LevelIO.GetLevelPath() + "/" + _fileName + ".bytes");
		if (level != null)
		{
			m_currentLevel = level;
			m_currentLevel.m_currentLayer = m_currentLevel.m_layers[0];
			m_currentLevel.m_currentLayerIndex = 0;
			AssembleCurrentLevel();
		}
	}

	public static Graph LoadGraph(string _fileName)
	{
		m_uniqueID = 0u;
		if (_fileName != string.Empty)
		{
			string path = LevelIO.GetLevelPath() + "/Logic/" + _fileName + ".bytes";
			if (LevelIO.FileExists(path))
			{
				return LevelSerializer.DeSerializeGraph(path);
			}
		}
		return null;
	}

	public static void SaveCurrentLevel()
	{
		if (m_currentLevel != null)
		{
			SaveCurrentLevel(m_currentLevel.m_fileName);
		}
	}

	public static void SaveCurrentLevel(string _fileName)
	{
		if (m_currentLevel != null)
		{
			LevelSerializer.SerializeLevelToFile(LevelIO.GetLevelPath() + "/" + _fileName + ".bytes", m_currentLevel);
		}
	}

	public static void SaveGraph(Graph _graph, string _fileName)
	{
		if (_graph != null)
		{
			LevelSerializer.SerializeGraph(LevelIO.GetLevelPath() + "/Logic/" + _fileName + ".bytes", _graph);
		}
	}

	public static void AssembleCurrentLevel()
	{
		if (m_currentLevel != null)
		{
			m_currentLevel.m_layers[m_currentLevel.m_currentLayerIndex].Initialize();
			m_currentLevel.m_layers[m_currentLevel.m_currentLayerIndex].Assemble();
		}
	}

	public static void ClearCurrentLevel(bool _isReset)
	{
		if (m_currentLevel != null)
		{
			for (int i = 0; i < m_currentLevel.m_layers.Count; i++)
			{
				m_currentLevel.m_layers[i].Clear(_isReset);
			}
		}
	}

	public static void ResetCurrentLevel()
	{
		if (m_currentLevel != null)
		{
			ClearCurrentLevel(true);
			m_currentLevel.m_currentLayerIndex = 0;
			m_currentLevel.m_currentLayer = m_currentLevel.m_layers[0];
			EntityManager.Update();
			AssembleCurrentLevel();
		}
	}

	public static void DestroyCurrentLevel()
	{
		if (m_currentLevel != null)
		{
			m_currentLevel.Destroy();
		}
		m_currentLevel = null;
	}

	public static Level NewLevel(int _width, int _height)
	{
		if (m_currentLevel != null)
		{
			DestroyCurrentLevel();
		}
		Level level = new Level(_width, _height);
		level.m_fileName = "MyLevel";
		level.m_name = "MyLevel";
		level.m_description = "MyDescription";
		m_currentLevel = level;
		return level;
	}

	public static Level NewLevel(Level _level)
	{
		if (m_currentLevel != null)
		{
			DestroyCurrentLevel();
		}
		m_currentLevel = _level;
		return _level;
	}

	public static void Update()
	{
		if (m_currentLevel != null)
		{
			m_currentLevel.Update();
		}
	}
}
