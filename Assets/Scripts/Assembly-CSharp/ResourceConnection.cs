using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ResourceConnection : GraphConnection, ISerializable, IDeserializationCallback
{
	public ResourceConnection()
		: base("ResourceConnection")
	{
		m_connectionType = ConnectionType.Resource;
	}

	public ResourceConnection(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override void Pull()
	{
		if (m_start == null || m_start.m_elementType == GraphElementType.Connection)
		{
			return;
		}
		GraphNode graphNode = m_start as GraphNode;
		GraphNode graphNode2 = m_end as GraphNode;
		if (graphNode.m_resourceCount != 0f)
		{
			float num = m_label.Calculate(graphNode.m_resourceCount);
			if (num > graphNode.m_resourceCount)
			{
				graphNode2.m_resourceCount += graphNode.m_resourceCount;
				graphNode.m_resourceCount = 0f;
			}
			else
			{
				graphNode.m_resourceCount -= num;
				graphNode2.m_resourceCount += num;
			}
		}
	}

	public override void Trigger()
	{
		if (m_end != null)
		{
			m_end.Trigger();
		}
	}

	public new ResourceConnection DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (ResourceConnection)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
	}
}
