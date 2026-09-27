using System;
using System.Runtime.Serialization;

[Serializable]
public class BasicComponent : IComponent, IPoolable, ISerializable
{
	private bool _active;

	private int _identifier;

	private int _index;

	private Entity _entity;

	private ComponentType _componentType;

	private bool _wasActive;

	public bool m_active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
		}
	}

	public int m_identifier
	{
		get
		{
			return _identifier;
		}
		set
		{
			_identifier = value;
		}
	}

	public int m_index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public Entity p_entity
	{
		get
		{
			return _entity;
		}
		set
		{
			_entity = value;
		}
	}

	public ComponentType m_componentType
	{
		get
		{
			return _componentType;
		}
		set
		{
			_componentType = value;
		}
	}

	public bool m_wasActive
	{
		get
		{
			return _wasActive;
		}
		set
		{
			_wasActive = value;
		}
	}

	public BasicComponent(ComponentType _componentType)
	{
		m_componentType = _componentType;
	}

	public BasicComponent(SerializationInfo info, StreamingContext ctxt)
	{
		m_componentType = (ComponentType)(int)info.GetValue("componentType", typeof(ComponentType));
	}

	public virtual void Reset()
	{
		m_active = false;
		m_wasActive = false;
		m_identifier = -1;
	}

	public virtual void Destroy()
	{
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("componentType", m_componentType);
	}
}
