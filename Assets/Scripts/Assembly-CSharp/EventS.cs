using System;
using System.Collections;

public static class EventS
{
	public static DynamicArray<EventC> m_components;

	private static Entity m_eventEntity;

	public static void Initialize()
	{
		m_components = new DynamicArray<EventC>();
		m_eventEntity = EntityManager.AddEntity();
		m_eventEntity.m_persistent = true;
	}

	public static EventC AddComponent(Entity _entity, string _identifier, EventComponentDelegate _eventHandler, float _delay, bool _dispatchAutomaticly, bool _removeAfterDelegate, bool _delegateAtDestroy, bool _delegateOnlyOnce)
	{
		EventC eventC = m_components.AddItem();
		eventC.name = _identifier;
		eventC.delay = _delay;
		eventC.dispatched = _dispatchAutomaticly;
		eventC.startTime = Main.m_gameTime;
		eventC.count = 0;
		eventC.removeAfterDelegate = _removeAfterDelegate;
		eventC.delegateAtRemove = _delegateAtDestroy;
		eventC.delegateOnlyOnce = _delegateOnlyOnce;
		AddEventHandler(eventC, _eventHandler);
		EntityManager.AddComponentToEntity(_entity, eventC);
		return eventC;
	}

	public static void RemoveComponent(EventC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_c.properties = new Hashtable();
		if (_c.delegateAtRemove && (!_c.delegateOnlyOnce || _c.count == 0))
		{
			_c.eventDelegate(_c);
		}
		if (_c.eventDelegate != null)
		{
			Delegate[] invocationList = _c.eventDelegate.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				_c.eventDelegate = (EventComponentDelegate)Delegate.Remove(_c.eventDelegate, (EventComponentDelegate)obj);
			}
		}
		_c.delegatedCount = 0;
		_c.count = 0;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void RemoveComponentWithIdentifier(string _identifier)
	{
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			EventC eventC = m_components.m_array[m_components.m_aliveIndices[i]];
			if (eventC.name == _identifier)
			{
				RemoveComponent(eventC);
				break;
			}
		}
	}

	public static EventC FindEventComponent(string _identifier)
	{
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			EventC eventC = m_components.m_array[m_components.m_aliveIndices[i]];
			if (eventC.name == _identifier)
			{
				return eventC;
			}
		}
		return null;
	}

	public static EventC FindEventComponent(Entity _entity)
	{
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			EventC eventC = m_components.m_array[m_components.m_aliveIndices[i]];
			if (eventC.p_entity == _entity)
			{
				return eventC;
			}
		}
		return null;
	}

	public static void AddProperty(EventC _c, string _key, object _value)
	{
		_c.properties.Add(_key, _value);
	}

	public static void AddEventHandler(EventC _c, EventComponentDelegate _eventHandler)
	{
		if (_c.delegatedCount == 0)
		{
			_c.eventDelegate = _eventHandler;
		}
		else
		{
			_c.eventDelegate = (EventComponentDelegate)Delegate.Combine(_c.eventDelegate, _eventHandler);
		}
		_c.delegatedCount++;
	}

	public static void RemoveEventHandler(EventC _c, EventComponentDelegate _eventHandler)
	{
		if (_c.count > 0)
		{
			_c.eventDelegate = (EventComponentDelegate)Delegate.Remove(_c.eventDelegate, _eventHandler);
			_c.delegatedCount--;
		}
		if (_c.delegatedCount == 0)
		{
			_c.eventDelegate = DelegatedEventDebugMethod;
		}
	}

	public static void DelegatedEventDebugMethod(EventC _c)
	{
		Debug.Log(_c.name + ":\n");
		foreach (string key in _c.properties.Keys)
		{
			Debug.Log(string.Concat(_c.properties[key], "\n"));
		}
	}

	public static void DispatchComponent(EventC _c, bool _remove)
	{
		bool flag = false;
		bool flag2 = false;
		if (_c.m_active)
		{
			if (Main.m_gameTime + _c.delay <= Main.m_gameTime && (!_c.delegateOnlyOnce || _c.count == 0))
			{
				_c.count++;
				if (_c.delegateOnlyOnce || _c.delay == 0f)
				{
					_c.dispatched = false;
				}
				else
				{
					_c.startTime = Main.m_gameTime;
				}
				if (_remove || _c.removeAfterDelegate)
				{
					flag2 = true;
				}
				flag = true;
			}
			else if (!_c.delegateOnlyOnce || _c.count == 0)
			{
				_c.dispatched = true;
				_c.startTime = Main.m_gameTime;
			}
		}
		if (flag && _c.eventDelegate != null)
		{
			_c.eventDelegate(_c);
		}
		if (flag2 && _c.p_entity != null)
		{
			RemoveComponent(_c);
		}
	}

	public static void Dispatch(string _identifier, string[] _propertyKeys, object[] _propertyValues, bool _removeListener)
	{
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			EventC eventC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (eventC.m_active && eventC.name == _identifier)
			{
				if (_propertyKeys != null && _propertyValues != null)
				{
					for (int i = 0; i < _propertyKeys.Length; i++)
					{
						if (!eventC.properties.ContainsKey(_propertyKeys[i]))
						{
							AddProperty(eventC, _propertyKeys[i], _propertyValues[i]);
						}
						else
						{
							eventC.properties[_propertyKeys[i]] = _propertyValues[i];
						}
					}
				}
				DispatchComponent(eventC, _removeListener);
			}
		}
	}

	public static EventC AddListener(string _identifier, EventComponentDelegate _eventHandler, bool _removeAfterDelegate)
	{
		EventC eventC = m_components.AddItem();
		eventC.name = _identifier;
		eventC.delay = 0f;
		eventC.dispatched = false;
		eventC.startTime = Main.m_gameTime;
		eventC.count = 0;
		eventC.removeAfterDelegate = _removeAfterDelegate;
		eventC.delegateAtRemove = false;
		eventC.delegateOnlyOnce = false;
		AddEventHandler(eventC, _eventHandler);
		EntityManager.AddComponentToEntity(m_eventEntity, eventC);
		return eventC;
	}

	public static void Update()
	{
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			EventC eventC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (eventC.m_active && eventC.dispatched && eventC.startTime + eventC.delay <= Main.m_gameTime && (!eventC.delegateOnlyOnce || eventC.count == 0))
			{
				eventC.count++;
				eventC.dispatched = false;
				eventC.eventDelegate(eventC);
				if (eventC.removeAfterDelegate)
				{
					RemoveComponent(eventC);
				}
			}
		}
		m_components.Update();
	}
}
