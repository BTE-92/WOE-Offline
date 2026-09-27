using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Ghost : ISerializable
{
	public bool m_recording;

	public bool m_playing;

	public Hashtable m_nodes;

	public int m_playbackTick;

	public int m_keyframeCount;

	public Ghost(bool _record = true)
	{
		m_nodes = new Hashtable();
		Init(_record);
	}

	public Ghost(SerializationInfo info, StreamingContext ctxt)
	{
		Init(false);
		m_keyframeCount = (int)info.GetValue("keyframeCount", typeof(int));
		m_nodes = (Hashtable)info.GetValue("nodes", typeof(Hashtable));
		Debug.Log("Ghost deserialized... keyframes: " + m_keyframeCount);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("keyframeCount", m_keyframeCount);
		info.AddValue("nodes", m_nodes);
		Debug.Log("Ghost serialized... keyframes: " + m_keyframeCount);
	}

	public Ghost DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (Ghost)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public void Init(bool _record)
	{
		m_recording = _record;
		m_playing = !_record;
		m_playbackTick = 0;
		m_keyframeCount = 0;
		if (_record)
		{
			Debug.Log("New ghost created for recording.");
		}
		else
		{
			Debug.Log("New playback ghost created.");
		}
	}

	public void AddNode(string _name, TransformC _TC)
	{
		GhostNode value = new GhostNode(_name, _TC);
		m_nodes.Add(_name, value);
		Debug.Log("Node " + _name + " added to ghost.");
	}

	public virtual void Update()
	{
		if (m_recording)
		{
			foreach (DictionaryEntry node in m_nodes)
			{
				GhostNode ghostNode = node.Value as GhostNode;
				ghostNode.AddKeyFrame();
			}
			m_keyframeCount++;
		}
		else if (m_playing)
		{
			m_playbackTick = Mathf.Min(m_playbackTick + 1, m_keyframeCount - 1);
		}
	}

	public void StopRecord()
	{
		if (m_recording)
		{
			m_recording = false;
		}
	}

	public bool PlaybackEnded()
	{
		return m_playbackTick == m_keyframeCount - 1;
	}

	public Vector2 GetCurrentPosition(string _nodeName)
	{
		return (m_nodes[_nodeName] as GhostNode).GetKeyFramePos(m_playbackTick);
	}

	public float GetCurrentRotation(string _nodeName)
	{
		return (m_nodes[_nodeName] as GhostNode).GetKeyFrameRotation(m_playbackTick);
	}

	public virtual void Destroy()
	{
		if (m_recording)
		{
			Debug.Log("Destroying recording ghost");
		}
		else
		{
			Debug.Log("Destroying playback ghost");
		}
	}

	public static byte[] SerializeToBytes(Ghost _ghost)
	{
		return SerializeToStream(_ghost).ToArray();
	}

	public static MemoryStream SerializeToStream(Ghost _ghost)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, _ghost);
		return memoryStream;
	}

	public static Ghost DeSerializeFromBytes(byte[] _bytes)
	{
		Debug.Log("Deserializing ghost...");
		MemoryStream memoryStream = new MemoryStream(_bytes, 0, _bytes.Length, false, true);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Ghost ghost = null;
		ghost = (Ghost)binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return ghost;
	}
}
