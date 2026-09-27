using System;
using System.Runtime.Serialization;

[Serializable]
public class LevelLayer : Graph
{
	public Graph m_logicGraph;

	public string m_logicFile;

	public bool m_logicModified;

	public LevelLayerType m_layerType;

	public int m_layerWidth;

	public int m_layerHeight;

	public LevelLayer(LevelLayerType _layerType, int _width, int _height)
	{
		m_layerType = _layerType;
		m_layerWidth = _width;
		m_layerHeight = _height;
		m_logicGraph = new Graph("Logic");
		AddElement(m_logicGraph);
	}

	public LevelLayer(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_logicFile = (string)info.GetValue("logicFile", typeof(string));
		m_logicModified = (bool)info.GetValue("logicModified", typeof(bool));
		m_layerType = (LevelLayerType)(uint)info.GetValue("layerType", typeof(uint));
		m_layerWidth = (int)info.GetValue("layerWidth", typeof(int));
		m_layerHeight = (int)info.GetValue("layerHeight", typeof(int));
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Dispose()
	{
		m_logicGraph = null;
		base.Dispose();
	}

	public override void OnDeserialization(object sender)
	{
		base.OnDeserialization(sender);
		m_logicGraph = m_elements[0] as Graph;
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("logicFile", m_logicFile);
		info.AddValue("logicModified", m_logicModified);
		info.AddValue("layerType", (uint)m_layerType);
		info.AddValue("layerWidth", m_layerWidth);
		info.AddValue("layerHeight", m_layerHeight);
	}
}
