using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LevelSerializer
{
	public static void SerializeLevelToFile(string _file, Level _levelData)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Stream stream = File.Open(_file, FileMode.Create);
		if (stream != null)
		{
			binaryFormatter.Serialize(stream, _levelData);
			stream.Close();
		}
	}

	public static byte[] SerializeLevelToBytes(Level _levelData)
	{
		return SerializeLevelToStream(_levelData).ToArray();
	}

	public static MemoryStream SerializeLevelToStream(Level _levelData)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, _levelData);
		return memoryStream;
	}

	public static void SerializeGraph(string _file, Graph _graph)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Stream stream = File.Open(_file, FileMode.Create);
		binaryFormatter.Serialize(stream, _graph);
		stream.Close();
	}

	public static Level DeSerializeLevelFromFile(string _path)
	{
		Debug.Log("Deserializing level...");
		if (LevelIO.FileExists(_path))
		{
			Level level = null;
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			Stream stream = File.Open(_path, FileMode.Open);
			level = (Level)binaryFormatter.Deserialize(stream);
			stream.Close();
			return level;
		}
		Debug.LogWarning("File not found: " + _path);
		return null;
	}

	public static Level DeSerializeLevelFromBytes(byte[] _bytes)
	{
		Debug.Log("Deserializing level...");
		MemoryStream memoryStream = new MemoryStream(_bytes, 0, _bytes.Length, false, true);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Level level = null;
		level = (Level)binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return level;
	}

	public static Graph DeSerializeGraph(string _path)
	{
		if (LevelIO.FileExists(_path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			Stream stream = File.Open(_path, FileMode.Open);
			Graph result = (Graph)binaryFormatter.Deserialize(stream);
			stream.Close();
			return result;
		}
		Debug.LogWarning("File not found: " + _path);
		return null;
	}

	public static Level DeSerializeUnityLevel(string _path)
	{
		Level level = null;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		TextAsset textAsset = Resources.Load(_path) as TextAsset;
		if (textAsset != null)
		{
			Stream serializationStream = new MemoryStream(textAsset.bytes);
			return (Level)binaryFormatter.Deserialize(serializationStream);
		}
		Resources.UnloadAsset(textAsset);
		return null;
	}

	public static Graph DeSerializeUnityGraph(string _path)
	{
		Graph graph = null;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		TextAsset textAsset = Resources.Load(_path) as TextAsset;
		if (textAsset != null)
		{
			Stream serializationStream = new MemoryStream(textAsset.bytes);
			return (Graph)binaryFormatter.Deserialize(serializationStream);
		}
		Resources.UnloadAsset(textAsset);
		return null;
	}
}
