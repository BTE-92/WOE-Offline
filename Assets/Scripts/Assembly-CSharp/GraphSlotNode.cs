using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class GraphSlotNode : GraphNode
{
	public SlotType m_slotType;

	public List<Graph> m_externalGraphs;

	public List<GraphNode> m_externalNodes;

	public int[] m_externalGraphIds;

	public int[] m_externalNodeIds;

	public GraphSlotNode(string _name, Vector3 _pos, Vector3 _rot)
		: base(_name)
	{
		m_position = _pos;
		m_rotation = _rot;
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
		m_externalGraphs = new List<Graph>();
		m_externalNodes = new List<GraphNode>();
	}

	public GraphSlotNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_slotType = (SlotType)(int)info.GetValue("slotType", typeof(SlotType));
		m_externalGraphIds = (int[])info.GetValue("externalGraphIds", typeof(int[]));
		m_externalNodeIds = (int[])info.GetValue("externalNodeIds", typeof(int[]));
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Assemble()
	{
		m_assembled = true;
	}

	public new SimpleLevelNode DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (SimpleLevelNode)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void OnDeserialization(object sender)
	{
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("slotType", m_slotType);
		info.AddValue("externalGroups", m_externalGraphIds);
		info.AddValue("externalNodes", m_externalNodeIds);
	}
}
