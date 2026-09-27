using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelPlayerNode : SimpleLevelNode
{
	public LevelPlayerNode(Type _assembleClassType, string _name, Vector3 _pos, Vector3 _rot, Vector3 _sca)
		: base(_assembleClassType, _name, _pos, _rot, _sca)
	{
		m_width = 15f;
		m_height = 15f;
		m_isRect = false;
	}

	public LevelPlayerNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_assembleClassType = Type.GetType((string)info.GetValue("assembleClassType", typeof(string)));
		SetPropertyDefaults();
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isRemovable = false;
		m_isCopyable = false;
		m_isRotateable = false;
	}

	public new LevelPlayerNode DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (LevelPlayerNode)binaryFormatter.Deserialize(memoryStream);
		}
	}
}
