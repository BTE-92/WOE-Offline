using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Level : ISerializable
{
	public string m_name;

	public string m_description;

	public string m_fileName;

	public Hashtable m_settings;

	public List<LevelLayer> m_layers;

	public LevelLayer m_currentLayer;

	public int m_currentLayerIndex;

	public Level(int _width, int _height)
	{
		m_settings = new Hashtable();
		m_layers = new List<LevelLayer>();
		m_currentLayer = new LevelLayer(LevelLayerType.BaseLayer, _width, _height);
		m_layers.Add(m_currentLayer);
		m_currentLayerIndex = 0;
	}

	public Level(SerializationInfo info, StreamingContext ctxt)
	{
		m_settings = (Hashtable)info.GetValue("settings", typeof(Hashtable));
		m_layers = new List<LevelLayer>((LevelLayer[])info.GetValue("layers", typeof(LevelLayer[])));
		m_name = (string)info.GetValue("name", typeof(string));
		m_fileName = (string)info.GetValue("fileName", typeof(string));
		try
		{
			m_description = (string)info.GetValue("description", typeof(string));
		}
		catch
		{
			m_description = "Description Missing";
		}
	}

	public virtual void ApplySettings()
	{
	}

	public virtual void Destroy()
	{
		while (m_layers.Count > 0)
		{
			int index = m_layers.Count - 1;
			m_layers[index].Dispose();
			m_layers.RemoveAt(index);
		}
		m_layers = null;
		m_currentLayer = null;
		m_currentLayerIndex = -1;
		m_settings.Clear();
		m_settings = null;
	}

	public virtual void Update()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			if (m_layers[i].m_assembled && !m_layers[i].m_disabled)
			{
				m_layers[i].Update();
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("settings", m_settings);
		info.AddValue("layers", m_layers.ToArray());
		info.AddValue("name", m_name);
		info.AddValue("fileName", m_fileName);
		info.AddValue("description", m_description);
	}
}
