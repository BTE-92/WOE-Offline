using System.Collections;

public class EventC : BasicComponent
{
	public static int m_componentCount;

	public bool dispatched;

	public float startTime;

	public float delay;

	public bool removeAfterDelegate;

	public bool delegateAtRemove;

	public bool delegateOnlyOnce;

	public int count;

	public string name;

	public Hashtable properties;

	public EventComponentDelegate eventDelegate;

	public int delegatedCount;

	public EventC()
		: base(ComponentType.Event)
	{
		properties = new Hashtable();
		eventDelegate = EventS.DelegatedEventDebugMethod;
		m_componentCount++;
	}

	~EventC()
	{
		m_componentCount--;
	}
}
