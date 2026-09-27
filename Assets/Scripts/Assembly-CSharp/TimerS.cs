using System.Collections.Generic;

public static class TimerS
{
	public static DynamicArray<TimerC> m_components;

	private static List<Entity> m_removeEntityList;

	public static void Initialize()
	{
		m_components = new DynamicArray<TimerC>();
		m_removeEntityList = new List<Entity>();
	}

	public static TimerC AddComponent(Entity _entity, string _name, float _duration, float _delay, bool _destroyEntityOnTimeout, TimerComponentDelegate _timeoutHandler = null)
	{
		TimerC timerC = m_components.AddItem();
		timerC.name = _name;
		timerC.duration = _duration;
		timerC.currentTime = 0f - _delay;
		timerC.timeoutHandler = _timeoutHandler;
		timerC.isDone = false;
		timerC.destroyEntityOnTimeout = _destroyEntityOnTimeout;
		timerC.customComponent = null;
		EntityManager.AddComponentToEntity(_entity, timerC);
		return timerC;
	}

	public static void RemoveComponent(TimerC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_c.timeoutHandler = null;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void Update()
	{
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			TimerC timerC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (timerC.m_active)
			{
				timerC.currentTime += Main.m_gameDeltaTime;
				if (timerC.currentTime >= timerC.duration)
				{
					timerC.isDone = true;
					if (timerC.timeoutHandler != null)
					{
						timerC.timeoutHandler(timerC);
					}
					if (timerC.destroyEntityOnTimeout && !m_removeEntityList.Contains(timerC.p_entity))
					{
						m_removeEntityList.Add(timerC.p_entity);
					}
				}
			}
		}
		while (m_removeEntityList.Count > 0)
		{
			int index = m_removeEntityList.Count - 1;
			EntityManager.RemoveEntity(m_removeEntityList[index]);
			m_removeEntityList.RemoveAt(index);
		}
		m_components.Update();
	}
}
