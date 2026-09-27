using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public abstract class GraphNode : GraphElement, ISerializable, IDeserializationCallback
{
	public PullMode m_pullMode;

	public TriggerMode m_triggerMode;

	public bool m_triggerFlag;

	public float m_triggerTimer;

	public float m_triggerInterval;

	public float m_resourceCount;

	public float m_maxResourceCount;

	public GraphNode()
		: base("GraphNode")
	{
		m_elementType = GraphElementType.Node;
		m_pullMode = PullMode.PullAny;
		m_triggerMode = TriggerMode.Automatic;
		m_triggerTimer = 0f;
		m_triggerInterval = 0f;
		m_resourceCount = 0f;
		m_maxResourceCount = -1f;
	}

	public GraphNode(string _name)
		: base(_name)
	{
		m_elementType = GraphElementType.Node;
	}

	public GraphNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_pullMode = (PullMode)(uint)info.GetValue("pullMode", typeof(uint));
		m_triggerMode = (TriggerMode)(uint)info.GetValue("triggerMode", typeof(uint));
		m_triggerInterval = (float)info.GetValue("triggerInterval", typeof(float));
	}

	public override void Update()
	{
		if (m_disabled)
		{
			return;
		}
		if (m_triggerMode == TriggerMode.Automatic)
		{
			m_triggerTimer -= Main.m_gameDeltaTime;
			if (!(m_triggerTimer <= 0f))
			{
				return;
			}
			Trigger();
			if (m_triggerMode == TriggerMode.Automatic)
			{
				if (m_triggerInterval > 0f)
				{
					m_triggerTimer += m_triggerInterval;
				}
				else
				{
					m_triggerTimer = 0f;
				}
			}
			else
			{
				m_triggerTimer = 0f;
			}
		}
		else if (m_triggerFlag)
		{
			Trigger();
			m_triggerFlag = false;
		}
	}

	public override void Trigger()
	{
		if (m_disabled)
		{
			return;
		}
		if (m_pullMode == PullMode.TriggerAny)
		{
			for (int i = 0; i < m_outputs.Count; i++)
			{
				m_outputs[i].Trigger();
			}
		}
		else if (m_pullMode == PullMode.TriggerAllOrNone)
		{
			if (ResourceDemandSatisfied(m_stateOutputCount))
			{
				for (int j = 0; j < m_outputs.Count; j++)
				{
					m_outputs[j].Trigger();
				}
			}
		}
		else if (m_pullMode == PullMode.PullAny)
		{
			for (int k = 0; k < m_inputs.Count; k++)
			{
				m_inputs[k].Pull();
			}
		}
		else if (m_pullMode == PullMode.PullAndTriggerAny)
		{
			for (int l = 0; l < m_inputs.Count; l++)
			{
				m_inputs[l].Pull();
			}
			for (int m = 0; m < m_outputs.Count; m++)
			{
				m_outputs[m].Trigger();
			}
		}
		else if (m_pullMode == PullMode.PullAllOrNone)
		{
			if (EnoughResourcesToPull(m_stateInputCount))
			{
				for (int n = 0; n < m_inputs.Count; n++)
				{
					m_inputs[n].Pull();
				}
			}
		}
		else
		{
			if (m_pullMode != PullMode.PullAndTriggerAllOrNone || !EnoughResourcesToPull(m_stateInputCount))
			{
				return;
			}
			for (int num = 0; num < m_inputs.Count; num++)
			{
				m_inputs[num].Pull();
			}
			if (ResourceDemandSatisfied(m_stateOutputCount))
			{
				for (int num2 = 0; num2 < m_outputs.Count; num2++)
				{
					m_outputs[num2].Trigger();
				}
			}
		}
	}

	public virtual bool ResourceDemandSatisfied(int _index)
	{
		if (_index == m_outputs.Count)
		{
			return true;
		}
		float num = 0f;
		for (int i = _index; i < m_outputs.Count; i++)
		{
			num += m_outputs[i].m_label.m_value;
		}
		if (m_resourceCount >= num)
		{
			return true;
		}
		return false;
	}

	public virtual bool EnoughResourcesToPull(int _index)
	{
		if (_index == m_inputs.Count)
		{
			return true;
		}
		for (int i = _index; i < m_inputs.Count; i++)
		{
			GraphConnection graphConnection = m_inputs[i];
			if (graphConnection.m_start != null && graphConnection.m_label.m_value > (graphConnection.m_start as GraphNode).m_resourceCount)
			{
				return false;
			}
		}
		return true;
	}

	public new GraphNode DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (GraphNode)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("pullMode", (uint)m_pullMode);
		info.AddValue("triggerMode", (uint)m_triggerMode);
		info.AddValue("triggerInterval", m_triggerInterval);
	}
}
