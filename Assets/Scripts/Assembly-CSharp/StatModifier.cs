public class StatModifier
{
	public float m_multipler;

	public float m_endTime;

	public float m_duration;

	public int m_stack;

	public int m_maxStack;

	public StatModifier()
	{
		m_multipler = 1f;
		m_endTime = -1f;
		m_duration = 10f;
		m_stack = 0;
		m_maxStack = 1;
	}
}
