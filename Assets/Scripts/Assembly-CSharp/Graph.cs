using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Graph : GraphNode, ISerializable, IDeserializationCallback
{
	public List<GraphElement> m_elements;

	public GraphElement[] m_tempElements;

	public Graph()
	{
		m_name = "Graph";
		m_elements = new List<GraphElement>();
		m_elementType = GraphElementType.Graph;
	}

	public Graph(string _name)
	{
		m_name = _name;
		m_elements = new List<GraphElement>();
		m_elementType = GraphElementType.Graph;
	}

	public Graph(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_tempElements = (GraphElement[])info.GetValue("elements", typeof(GraphElement[]));
	}

	public override void Initialize()
	{
		base.Initialize();
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].Initialize();
		}
	}

	public override void Assemble()
	{
		base.Assemble();
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].Assemble();
		}
	}

	public GraphElement GetElement(uint _id)
	{
		for (int i = 0; i < m_elements.Count; i++)
		{
			if (m_elements[i].m_id == _id)
			{
				return m_elements[i];
			}
		}
		return null;
	}

	public GraphElement GetElement(string _name)
	{
		for (int i = 0; i < m_elements.Count; i++)
		{
			if (m_elements[i].m_name == _name)
			{
				return m_elements[i];
			}
		}
		return null;
	}

	public void AddElement(GraphElement _element)
	{
		_element.m_graph = this;
		if (!m_elements.Contains(_element))
		{
			m_elements.Add(_element);
		}
	}

	public bool RemoveElement(GraphElement _element)
	{
		return m_elements.Remove(_element);
	}

	public override void Dispose()
	{
		while (m_elements.Count > 0)
		{
			int index = m_elements.Count - 1;
			m_elements[index].Dispose();
		}
		m_elements = null;
		base.Clear(false);
		base.Dispose();
	}

	public override void Clear(bool _isReset)
	{
		if (m_elements != null)
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				m_elements[i].Clear(_isReset);
			}
		}
		base.Clear(_isReset);
	}

	public override void Reset()
	{
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].Reset();
		}
		base.Reset();
	}

	public override void Update()
	{
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].Update();
		}
	}

	public new Graph DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (Graph)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void OnDeserialization(object sender)
	{
		base.OnDeserialization(sender);
		m_elements = new List<GraphElement>(m_tempElements);
		m_tempElements = null;
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].m_graph = this;
			if (m_elements[i].m_elementType == GraphElementType.Connection)
			{
				GraphConnection graphConnection = m_elements[i] as GraphConnection;
				GraphElement element = GetElement(graphConnection.m_startId);
				GraphElement element2 = GetElement(graphConnection.m_endId);
				element.AddOutput(graphConnection);
				element2.AddInput(graphConnection);
			}
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("elements", m_elements.ToArray());
	}
}
