using UnityEngine;

public class Cache<T> : ICache
{
	private string _key;

	private float _created;

	private float _updated;

	private float _expires;

	private float _expireDuration;

	private T[] m_objects;

	public string m_key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	public float m_created
	{
		get
		{
			return _created;
		}
		set
		{
			_created = value;
		}
	}

	public float m_updated
	{
		get
		{
			return _updated;
		}
		set
		{
			_updated = value;
		}
	}

	public float m_expires
	{
		get
		{
			return _expires;
		}
		set
		{
			_expires = value;
		}
	}

	public float m_expireDuration
	{
		get
		{
			return _expireDuration;
		}
		set
		{
			_expireDuration = value;
		}
	}

	public Cache(string _key, float _expireDuration)
	{
		m_key = _key;
		m_expireDuration = _expireDuration;
		m_created = Time.realtimeSinceStartup;
		m_updated = m_created;
		m_expires = m_created + m_expireDuration;
		m_objects = new T[0];
	}

	public bool IsExpired()
	{
		if (Time.realtimeSinceStartup > m_expires)
		{
			return true;
		}
		return false;
	}

	public int GetCacheLength()
	{
		return m_objects.Length;
	}

	public T[] GetObjects()
	{
		return m_objects;
	}

	public void AddToHead(T[] _objects)
	{
		T[] array = new T[m_objects.Length];
		m_objects.CopyTo(array, 0);
		m_objects = new T[array.Length + _objects.Length];
		_objects.CopyTo(m_objects, 0);
		array.CopyTo(m_objects, _objects.Length);
		array = null;
		m_updated = Time.realtimeSinceStartup;
		m_expires = m_updated + m_expireDuration;
	}

	public void AddToTail(T[] _objects)
	{
		T[] array = new T[m_objects.Length];
		m_objects.CopyTo(array, 0);
		m_objects = new T[array.Length + _objects.Length];
		array.CopyTo(m_objects, 0);
		_objects.CopyTo(m_objects, array.Length);
		array = null;
		m_updated = Time.realtimeSinceStartup;
		m_expires = m_updated + m_expireDuration;
	}

	public void Insert(int _index, T[] _objects)
	{
		if (_index == 0)
		{
			AddToHead(_objects);
			return;
		}
		T[] array = new T[_index];
		for (int i = 0; i < _index; i++)
		{
			array[i] = m_objects[i];
		}
		int num = m_objects.Length - _index;
		T[] array2 = new T[num];
		for (int j = 0; j < _index; j++)
		{
			array2[j] = m_objects[_index + j];
		}
		m_objects = new T[array.Length + array2.Length + _objects.Length];
		array.CopyTo(m_objects, 0);
		_objects.CopyTo(m_objects, array.Length);
		array2.CopyTo(m_objects, array.Length + _objects.Length);
		array = null;
		array2 = null;
		m_updated = Time.realtimeSinceStartup;
		m_expires = m_updated + m_expireDuration;
	}

	public void Clear()
	{
		m_created = Time.realtimeSinceStartup;
		m_updated = m_created;
		m_expires = m_created + m_expireDuration;
		m_objects = new T[0];
	}
}
