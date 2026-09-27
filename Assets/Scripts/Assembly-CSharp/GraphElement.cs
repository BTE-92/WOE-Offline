using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public abstract class GraphElement : ISerializable, IDeserializationCallback
{
	public TransformC m_TC;

	public TouchAreaC m_TAC;

	public Graph m_graph;

	public GraphElementType m_elementType;

	public string m_name;

	public uint m_id;

	public float m_radius;

	public float m_width;

	public float m_height;

	public bool m_isRect;

	public Vector3 m_position;

	public Vector3 m_rotation;

	public Vector3 m_scale;

	public bool m_flipped;

	public bool m_disabled;

	public bool m_disabledAtStart;

	public bool m_selected;

	public List<GraphConnection> m_inputs;

	public List<GraphConnection> m_outputs;

	public int m_stateInputCount;

	public int m_stateOutputCount;

	public bool m_assembled;

	public List<IAssembledClass> m_assembledClasses;

	public bool m_isMoveable;

	public bool m_isRotateable;

	public bool m_isScaleable;

	public bool m_isScaleableUniform;

	public bool m_isCopyable;

	public bool m_isRemovable;

	public bool m_isModifiable;

	public bool m_isReplaceable;

	public bool m_isFlippable;

	public GraphElement(string _name)
	{
		m_name = _name;
		m_id = LevelManager.GetUniqueId();
		m_inputs = new List<GraphConnection>();
		m_outputs = new List<GraphConnection>();
		m_assembledClasses = new List<IAssembledClass>();
		m_disabled = true;
		m_radius = 25f;
		m_width = 50f;
		m_height = 50f;
		m_isRect = false;
		m_stateInputCount = 0;
		m_stateOutputCount = 0;
		m_position = Vector3.zero;
		m_rotation = Vector3.zero;
		m_scale = Vector3.one;
		m_flipped = false;
		m_assembled = false;
		SetPropertyDefaults();
	}

	public GraphElement(SerializationInfo info, StreamingContext ctxt)
	{
		m_elementType = (GraphElementType)(uint)info.GetValue("elementType", typeof(uint));
		m_name = (string)info.GetValue("name", typeof(string));
		m_id = (uint)info.GetValue("id", typeof(uint));
		float x = (float)info.GetValue("pX", typeof(float));
		float y = (float)info.GetValue("pY", typeof(float));
		float z = (float)info.GetValue("pZ", typeof(float));
		float x2 = (float)info.GetValue("rX", typeof(float));
		float y2 = (float)info.GetValue("rY", typeof(float));
		float z2 = (float)info.GetValue("rZ", typeof(float));
		float x3 = (float)info.GetValue("sX", typeof(float));
		float y3 = (float)info.GetValue("sY", typeof(float));
		float z3 = (float)info.GetValue("sZ", typeof(float));
		m_position = new Vector3(x, y, z);
		m_rotation = new Vector3(x2, y2, z2);
		m_scale = new Vector3(x3, y3, z3);
		m_flipped = (bool)info.GetValue("flipped", typeof(bool));
		m_width = (float)info.GetValue("width", typeof(float));
		m_height = (float)info.GetValue("height", typeof(float));
		m_isRect = (bool)info.GetValue("isRect", typeof(bool));
		m_disabledAtStart = (bool)info.GetValue("disabled", typeof(bool));
		SetPropertyDefaults();
		m_inputs = new List<GraphConnection>();
		m_outputs = new List<GraphConnection>();
		m_assembledClasses = new List<IAssembledClass>();
		m_disabled = true;
		m_stateInputCount = 0;
		m_stateOutputCount = 0;
		if (m_id > LevelManager.m_uniqueID)
		{
			LevelManager.m_uniqueID = m_id + 1;
		}
	}

	public virtual void SetPropertyDefaults()
	{
		m_isMoveable = true;
		m_isRotateable = false;
		m_isScaleable = false;
		m_isScaleableUniform = true;
		m_isCopyable = true;
		m_isRemovable = true;
		m_isModifiable = true;
		m_isReplaceable = true;
		m_isFlippable = false;
	}

	public virtual Vector3 GetConnectionPosition(Vector3 _point)
	{
		Vector3 vector = _point - m_position;
		return m_position + vector.normalized * m_width;
	}

	public virtual Vector3 GetPosition()
	{
		return m_position;
	}

	public virtual void AddInput(GraphConnection _connection)
	{
		if (_connection.m_end != null)
		{
			_connection.m_end.RemoveInput(_connection);
		}
		if (_connection.m_connectionType == ConnectionType.State)
		{
			m_inputs.Insert(m_stateInputCount, _connection);
			m_stateInputCount++;
		}
		else
		{
			m_inputs.Add(_connection);
		}
		_connection.m_end = this;
		_connection.m_points[_connection.m_points.Count - 1] = GetPosition();
		_connection.CalculateStartPosition(false);
		_connection.CalculateEndPosition(false);
		_connection.Reset();
	}

	public virtual void AddOutput(GraphConnection _connection)
	{
		if (_connection.m_start != null)
		{
			_connection.m_start.RemoveOutput(_connection);
		}
		if (_connection.m_connectionType == ConnectionType.State)
		{
			m_outputs.Insert(m_stateOutputCount, _connection);
			m_stateOutputCount++;
		}
		else
		{
			m_outputs.Add(_connection);
		}
		_connection.m_start = this;
		_connection.m_points[0] = GetPosition();
		_connection.CalculateEndPosition(false);
		_connection.CalculateStartPosition(false);
		_connection.Reset();
	}

	public virtual bool RemoveInput(GraphConnection _connection)
	{
		if (m_inputs.Contains(_connection))
		{
			_connection.m_end = null;
			if (_connection.m_connectionType == ConnectionType.State)
			{
				m_stateInputCount--;
			}
			m_inputs.Remove(_connection);
			return true;
		}
		return false;
	}

	public virtual bool RemoveOutput(GraphConnection _connection)
	{
		if (m_outputs.Contains(_connection))
		{
			_connection.m_start = null;
			if (_connection.m_connectionType == ConnectionType.State)
			{
				m_stateOutputCount--;
			}
			m_outputs.Remove(_connection);
			return true;
		}
		return false;
	}

	public virtual void Dispose()
	{
		Clear(false);
		while (m_inputs.Count > 0)
		{
			int index = m_inputs.Count - 1;
			if (m_inputs[index] != null)
			{
				m_inputs[index].m_end = null;
			}
			m_inputs.RemoveAt(index);
		}
		m_inputs = null;
		while (m_outputs.Count > 0)
		{
			int index2 = m_outputs.Count - 1;
			if (m_outputs[index2] != null)
			{
				m_outputs[index2].m_start = null;
			}
			m_outputs.RemoveAt(index2);
		}
		m_outputs = null;
		if (m_graph != null)
		{
			m_graph.RemoveElement(this);
		}
	}

	public virtual void Initialize()
	{
	}

	public virtual void Assemble()
	{
		if (!m_disabledAtStart)
		{
			m_disabled = false;
		}
		if (m_TC == null && PsState.m_gameState == GameState.Edit)
		{
			m_TC = EntityManager.AddEntityWithTC();
			m_TC.transform.name = m_name;
			TransformS.SetGlobalPosition(m_TC, m_position);
			TransformS.SetGlobalRotation(m_TC, m_rotation);
		}
		m_assembled = true;
	}

	public virtual void Clear(bool _isReset)
	{
		while (m_assembledClasses.Count > 0)
		{
			int index = m_assembledClasses.Count - 1;
			m_assembledClasses[index].Destroy();
		}
		if (m_TC != null)
		{
			EntityManager.RemoveEntity(m_TC.p_entity);
			m_TC = null;
			m_TAC = null;
		}
		m_assembled = false;
	}

	public virtual void Reset()
	{
		Clear(true);
		Assemble();
	}

	public virtual void Update()
	{
	}

	public virtual void Trigger()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void End()
	{
	}

	public virtual void Flip()
	{
		Debug.Log("lol");
	}

	public GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (GraphElement)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public virtual void OnDeserialization(object sender)
	{
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("elementType", (uint)m_elementType);
		info.AddValue("name", m_name);
		info.AddValue("id", m_id);
		info.AddValue("pX", m_position.x);
		info.AddValue("pY", m_position.y);
		info.AddValue("pZ", m_position.z);
		info.AddValue("rX", m_rotation.x);
		info.AddValue("rY", m_rotation.y);
		info.AddValue("rZ", m_rotation.z);
		info.AddValue("sX", m_scale.x);
		info.AddValue("sY", m_scale.y);
		info.AddValue("sZ", m_scale.z);
		info.AddValue("flipped", m_flipped);
		info.AddValue("width", m_width);
		info.AddValue("height", m_height);
		info.AddValue("isRect", m_isRect);
		info.AddValue("disabled", m_disabledAtStart);
	}
}
