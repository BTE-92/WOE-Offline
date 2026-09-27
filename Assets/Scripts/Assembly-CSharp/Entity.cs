using System.Collections.Generic;

public class Entity : IPoolable
{
	public static int m_instanceCount;

	private int _index;

	private bool _active;

	public List<IComponent> m_components;

	public bool m_persistent;

	public int m_entityLogicCount;

	public EntityLogicDelegate d_entityLogic;

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

	public Entity()
	{
		m_index = -1;
		m_components = new List<IComponent>();
		m_persistent = false;
		m_entityLogicCount = 0;
		m_instanceCount++;
	}

	public void Reset()
	{
		m_persistent = false;
	}

	public void Destroy()
	{
	}

	~Entity()
	{
		m_instanceCount--;
	}
}
