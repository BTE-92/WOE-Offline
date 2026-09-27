public class Tag : IPoolable
{
	private int _index;

	private bool _active;

	public string m_tag;

	public Entity p_entity;

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

	public Tag()
	{
	}

	public Tag(string _tag, Entity _e)
	{
		m_tag = _tag;
		p_entity = _e;
	}

	public void Reset()
	{
	}

	public void Destroy()
	{
	}
}
