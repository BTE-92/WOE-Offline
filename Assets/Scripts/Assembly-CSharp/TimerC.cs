public class TimerC : BasicComponent
{
	public static int m_componentCount;

	public string name;

	public float currentTime;

	public float startTime;

	public float duration;

	public float delay;

	public bool isDone;

	public bool destroyEntityOnTimeout;

	public IComponent customComponent;

	public TimerComponentDelegate timeoutHandler;

	public TimerC()
		: base(ComponentType.Timer)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
	}

	~TimerC()
	{
		m_componentCount--;
	}
}
