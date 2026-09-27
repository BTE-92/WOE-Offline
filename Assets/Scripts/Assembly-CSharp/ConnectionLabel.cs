using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ConnectionLabel : ISerializable, IDeserializationCallback
{
	public string m_label;

	public LabelType m_labelType;

	public float m_value;

	public float m_range;

	public float m_minValue;

	public float m_maxValue;

	public ConnectionLabel()
	{
		m_labelType = LabelType.None;
		m_value = 1f;
		m_range = 0f;
		m_minValue = -9999f;
		m_maxValue = 9999f;
	}

	public ConnectionLabel(SerializationInfo info, StreamingContext ctxt)
	{
		m_label = (string)info.GetValue("label", typeof(string));
		m_labelType = (LabelType)(uint)info.GetValue("labelType", typeof(uint));
		m_range = (float)info.GetValue("range", typeof(float));
		m_minValue = (float)info.GetValue("min", typeof(float));
		m_maxValue = (float)info.GetValue("max", typeof(float));
	}

	public void ParseLabel()
	{
	}

	public float Calculate(float _resourceCount)
	{
		return _resourceCount * m_value;
	}

	public ConnectionLabel DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (ConnectionLabel)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public void OnDeserialization(object sender)
	{
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("label", m_label);
		info.AddValue("labelType", (uint)m_labelType);
		info.AddValue("range", m_range);
		info.AddValue("min", m_minValue);
		info.AddValue("max", m_maxValue);
	}
}
