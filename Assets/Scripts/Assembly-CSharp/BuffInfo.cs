public class BuffInfo
{
	public Buff m_buff;

	public IAssembledClass m_source;

	public IAssembledClass m_target;

	public float m_nextTick;

	public float m_lastTick;

	public int m_tries;

	public int m_stack;

	public int m_maxStack;

	public BuffInfo(Buff _buff, IAssembledClass _source, IAssembledClass _target)
	{
		m_buff = _buff;
		m_source = _source;
		m_target = _target;
		m_nextTick = -1f;
		m_lastTick = -1f;
		m_tries = 1;
		m_stack = 1;
		m_maxStack = 1;
	}
}
