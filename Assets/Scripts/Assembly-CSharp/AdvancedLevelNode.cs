using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class AdvancedLevelNode : Graph
{
	public Type m_assembleClassType;

	public AdvancedLevelNode(Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_name)
	{
		m_assembleClassType = _assembleClassType;
		m_position = _pos;
		m_rotation = _rot;
		m_scale = _sca;
		m_width = 20f;
		m_height = 20f;
		m_isRect = false;
	}

	public AdvancedLevelNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Assemble()
	{
		object[] args = new object[1] { this };
		IAssembledClass item = Activator.CreateInstance(m_assembleClassType, args) as IAssembledClass;
		m_assembledClasses.Add(item);
		for (int i = 0; i < m_elements.Count; i++)
		{
			m_elements[i].Assemble();
		}
		m_assembled = true;
	}

	public override void Trigger()
	{
		base.Trigger();
		if (m_disabled)
		{
		}
	}

	public new AdvancedLevelNode DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (AdvancedLevelNode)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
	}
}
