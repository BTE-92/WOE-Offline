using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SimpleLevelNode : GraphNode
{
	public Type m_assembleClassType;

	public SimpleLevelNode(Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_name)
	{
		m_assembleClassType = _assembleClassType;
		m_position = _pos;
		m_rotation = _rot;
		m_scale = _sca;
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
	}

	public SimpleLevelNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_assembleClassType = Type.GetType((string)info.GetValue("assembleClassType", typeof(string)));
	}

	public override void Initialize()
	{
		base.Initialize();
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

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("assembleClassType", m_assembleClassType.ToString());
	}

	public override void Assemble()
	{
		base.Assemble();
		object[] args = new object[1] { this };
		if (m_assembleClassType != null)
		{
			IAssembledClass item = Activator.CreateInstance(m_assembleClassType, args) as IAssembledClass;
			m_assembledClasses.Add(item);
		}
		else
		{
			Debug.Log("tried to construct a class that does not exist, ignoring");
		}
	}
}
