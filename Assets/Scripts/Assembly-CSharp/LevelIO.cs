using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelIO
{
	public static string GetApplicationRunPath()
	{
		string dataPath = Application.dataPath;
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			string text = dataPath;
			dataPath = text + "/../../SaveData/" + Main.m_currentGame.m_projectCode + "/" + Main.m_currentGame.m_projectVersion;
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			string text = dataPath;
			dataPath = text + "/../SaveData/" + Main.m_currentGame.m_projectCode + "/" + Main.m_currentGame.m_projectVersion;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			dataPath = Application.dataPath;
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			dataPath = Application.dataPath;
		}
		else
		{
			Debug.Log(Application.dataPath);
			Debug.Log(Application.persistentDataPath);
			dataPath = Application.persistentDataPath + "/Documents";
		}
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		return dataPath;
	}

	public static string GetLevelPath()
	{
		string text = GetApplicationRunPath() + "/" + Main.m_currentGame.m_projectCode + "/Resources/" + Main.m_currentGame.m_projectCode + "/Levels";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!Directory.Exists(text + "/Logic"))
		{
			Directory.CreateDirectory(text + "/Logic");
		}
		if (!Directory.Exists(text + "/Items"))
		{
			Directory.CreateDirectory(text + "/Items");
		}
		return text;
	}

	public static List<string> GetLevels()
	{
		List<string> list = new List<string>();
		string[] files = Directory.GetFiles(GetLevelPath());
		string[] separator = new string[3] { "/", "\\", "." };
		for (int i = 0; i < files.Length; i++)
		{
			string[] array = files[i].Split(separator, StringSplitOptions.None);
			if (array[array.Length - 1] != "meta")
			{
				string item = array[array.Length - 2];
				list.Add(item);
			}
		}
		return list;
	}

	public static bool FileExists(string _path)
	{
		return File.Exists(_path);
	}

	public static bool LevelExists(string _levelFile)
	{
		List<string> levels = GetLevels();
		for (int i = 0; i < levels.Count; i++)
		{
			if (levels[i] == _levelFile)
			{
				return true;
			}
		}
		return false;
	}
}
