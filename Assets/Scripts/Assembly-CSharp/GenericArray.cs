public class GenericArray<T>
{
	public T[] m_array;

	public int m_arrayLength;

	public int m_lastReserved;

	public int[] m_aliveIndices;

	public int m_aliveCount;

	public int[] m_freeIndices;

	public int m_freeCount;

	public GenericArray(int _arrayLength)
	{
		m_array = new T[_arrayLength];
		m_arrayLength = _arrayLength;
		m_lastReserved = 0;
		m_aliveIndices = new int[_arrayLength];
		m_aliveCount = 0;
		m_freeIndices = new int[_arrayLength];
		m_freeCount = _arrayLength;
		for (int i = 0; i < _arrayLength; i++)
		{
			m_freeIndices[i] = i;
			m_aliveIndices[i] = -1;
		}
	}

	private int AllocateNewIndex()
	{
		m_freeCount--;
		int num = m_freeIndices[m_freeCount];
		m_aliveIndices[m_aliveCount] = num;
		m_aliveCount++;
		return num;
	}

	public int AddItem()
	{
		int num = AllocateNewIndex();
		if (num > m_lastReserved)
		{
			m_lastReserved = num;
		}
		return num;
	}

	public int AddItem(T item)
	{
		int num = AllocateNewIndex();
		m_array[num] = item;
		if (num > m_lastReserved)
		{
			m_lastReserved = num;
		}
		return num;
	}

	public void RemoveItem(int index)
	{
		if (index <= -1 || index >= m_arrayLength)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < m_aliveCount; i++)
		{
			if (flag)
			{
				if (m_aliveIndices[i] == index)
				{
					flag = false;
				}
			}
			else
			{
				m_aliveIndices[i - 1] = m_aliveIndices[i];
			}
		}
		if (!flag)
		{
			m_freeIndices[m_freeCount] = index;
			m_freeCount++;
			m_aliveCount--;
			m_aliveIndices[m_aliveCount] = -1;
		}
	}

	public void Clear()
	{
		m_aliveCount = 0;
		m_lastReserved = 0;
		m_freeCount = m_arrayLength;
		for (int i = 0; i < m_arrayLength; i++)
		{
			m_freeIndices[i] = i;
			m_aliveIndices[i] = -1;
		}
	}

	public T[] ToArray()
	{
		T[] array = new T[m_aliveCount];
		for (int i = 0; i < m_aliveCount; i++)
		{
			array[i] = m_array[m_aliveIndices[i]];
		}
		return array;
	}
}
