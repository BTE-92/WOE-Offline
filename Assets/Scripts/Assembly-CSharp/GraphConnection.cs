using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public abstract class GraphConnection : GraphElement, ISerializable, IDeserializationCallback
{
	public ConnectionType m_connectionType;

	public ConnectionLabel m_label;

	public uint m_startId;

	public uint m_endId;

	private float m_length;

	private float m_labelPos;

	public GraphElement m_start;

	public GraphElement m_end;

	public List<Vector3> m_points;

	private Vertex3[] m_tempPoints;

	public GraphConnection(string _name)
		: base(_name)
	{
		m_points = new List<Vector3>();
		m_points.Add(Vector3.zero);
		m_points.Add(Vector3.zero);
		m_label = new ConnectionLabel();
		m_labelPos = 0.75f;
		m_startId = 0u;
		m_endId = 0u;
		m_elementType = GraphElementType.Connection;
		m_connectionType = ConnectionType.State;
	}

	public GraphConnection(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_tempPoints = (Vertex3[])info.GetValue("points", typeof(Vertex3[]));
		m_label = (ConnectionLabel)info.GetValue("label", typeof(ConnectionLabel));
		m_connectionType = (ConnectionType)(uint)info.GetValue("connectionType", typeof(uint));
		m_startId = (uint)info.GetValue("startId", typeof(int));
		m_endId = (uint)info.GetValue("endId", typeof(int));
		m_labelPos = (float)info.GetValue("labelPos", typeof(float));
	}

	public override Vector3 GetPosition()
	{
		m_length = 0f;
		for (int i = 1; i < m_points.Count; i++)
		{
			m_length += (m_points[i] - m_points[i - 1]).magnitude;
		}
		float num = m_length * m_labelPos;
		float num2 = 0f;
		int index = 0;
		Vector3 vector = Vector3.zero;
		for (int j = 0; j < m_points.Count - 1; j++)
		{
			vector = m_points[j + 1] - m_points[j];
			float magnitude = vector.magnitude;
			num2 += magnitude;
			if (num2 > num)
			{
				index = j;
				num2 -= magnitude;
				break;
			}
		}
		m_position = m_points[index] + vector.normalized * (num - num2);
		return m_position;
	}

	public virtual void CalculateStartPosition(bool _reassemble)
	{
		if (m_start != null)
		{
			m_points[0] = m_start.GetConnectionPosition(m_points[1]);
			if (_reassemble)
			{
				Reset();
			}
		}
	}

	public virtual void CalculateEndPosition(bool _reassemble)
	{
		if (m_end != null)
		{
			m_points[m_points.Count - 1] = m_end.GetConnectionPosition(m_points[m_points.Count - 2]);
			if (_reassemble)
			{
				Reset();
			}
		}
	}

	public override void Dispose()
	{
		m_start = null;
		m_end = null;
		Clear(false);
		base.Dispose();
	}

	public override void Initialize()
	{
		if (m_startId != 0)
		{
			m_start = m_graph.GetElement(m_startId);
			m_start.AddOutput(this);
		}
		if (m_endId != 0)
		{
			m_end = m_graph.GetElement(m_endId);
			m_end.AddInput(this);
		}
		GetPosition();
	}

	public override void Assemble()
	{
		GetPosition();
	}

	public override void Clear(bool _isReset)
	{
		DebugDraw.Clear(CameraS.m_mainCamera, EntityManager.GetComponentsByType(ComponentType.Transform)[0] as TransformC);
		base.Clear(_isReset);
	}

	public virtual void Pull()
	{
	}

	public override void Trigger()
	{
		if (m_end != null)
		{
			m_end.Trigger();
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public new GraphConnection DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (GraphConnection)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void OnDeserialization(object sender)
	{
		m_points = new List<Vector3>(m_tempPoints.Length);
		for (int i = 0; i < m_tempPoints.Length; i++)
		{
			m_points.Add(m_tempPoints[i].ToVector3());
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		if (m_start != null)
		{
			m_startId = m_start.m_id;
		}
		else
		{
			m_startId = 0u;
		}
		if (m_end != null)
		{
			m_endId = m_end.m_id;
		}
		else
		{
			m_endId = 0u;
		}
		m_tempPoints = new Vertex3[m_points.Count];
		for (int i = 0; i < m_tempPoints.Length; i++)
		{
			m_tempPoints[i] = new Vertex3(m_points[i]);
		}
		info.AddValue("points", m_tempPoints);
		info.AddValue("label", m_label);
		info.AddValue("connectionType", (uint)m_connectionType);
		info.AddValue("startId", m_startId);
		info.AddValue("endId", m_endId);
		info.AddValue("labelPos", m_labelPos);
	}
}
