using System;
using UnityEngine;

public class DynamicArray<T> where T : IPoolable, new()
{
	public T[] m_array;

	public int m_currentLength;

	public float m_resizeIncreaseAmount;

	public float m_resizeDecreaseLimit;

	public float m_resizeDecreaseAmount;

	public int m_resizeMinIncrementAndSize;

	public int[] m_aliveIndices;

	public int m_aliveCount;

	public int[] m_freeIndices;

	public int m_freeCount;

	public DynamicArray(int _resizeMinIncrementAndSize = 20, float _resizeIncreaseAmount = 0.5f, float _resizeDecreaseLimit = 0.25f, float _resizeDecreaseAmount = 0.5f)
	{
		m_resizeMinIncrementAndSize = _resizeMinIncrementAndSize;
		m_currentLength = m_resizeMinIncrementAndSize;
		m_freeCount = m_resizeMinIncrementAndSize;
		m_array = new T[m_resizeMinIncrementAndSize];
		m_resizeIncreaseAmount = _resizeIncreaseAmount;
		m_resizeDecreaseLimit = _resizeDecreaseLimit;
		m_resizeDecreaseAmount = _resizeDecreaseAmount;
		m_aliveIndices = new int[m_resizeMinIncrementAndSize];
		m_aliveCount = 0;
		m_freeIndices = new int[m_resizeMinIncrementAndSize];
		for (int i = 0; i < m_resizeMinIncrementAndSize; i++)
		{
			m_freeIndices[i] = i;
			m_aliveIndices[i] = -1;
			T val = new T
			{
				m_index = i
			};
			m_array[i] = val;
		}
	}

	private T AllocateNew()
	{
		if (m_freeCount > 0)
		{
			m_freeCount--;
			int num = m_freeIndices[m_freeCount];
			m_aliveIndices[m_aliveCount] = num;
			m_aliveCount++;
			return m_array[num];
		}
		IncreaseLength(m_currentLength + Mathf.Max(m_resizeMinIncrementAndSize, Mathf.RoundToInt((float)m_currentLength * m_resizeIncreaseAmount)));
		return AllocateNew();
	}

	public T AddItem()
	{
		T result = AllocateNew();
		result.Reset();
		return result;
	}

	public void RemoveItem(T _item)
	{
		RemoveItem(_item.m_index);
	}

	public void RemoveItem(int _itemIndex)
	{
		if (_itemIndex > -1 && _itemIndex < m_currentLength)
		{
			bool flag = true;
			for (int i = 0; i < m_aliveCount; i++)
			{
				if (flag)
				{
					if (m_aliveIndices[i] == _itemIndex)
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
				m_freeIndices[m_freeCount] = _itemIndex;
				m_freeCount++;
				m_aliveCount--;
				m_aliveIndices[m_aliveCount] = -1;
			}
		}
		else
		{
			Debug.LogError("Array index out of bounds: " + _itemIndex + " / " + m_currentLength);
		}
	}

	public void AddSafetyThreshold(int _amount)
	{
		if (m_currentLength < m_aliveCount + _amount)
		{
			IncreaseLength(m_aliveCount + _amount);
		}
	}

	private void IncreaseLength(int _newLength)
	{
		int currentLength = m_currentLength;
		int num = _newLength - currentLength;
		m_currentLength = _newLength;
		Array.Resize(ref m_array, _newLength);
		Array.Resize(ref m_aliveIndices, _newLength);
		int[] array = new int[_newLength];
		for (int i = 0; i < currentLength; i++)
		{
			array[num + i] = m_freeIndices[i];
		}
		for (int j = 0; j < num; j++)
		{
			array[j] = currentLength + j;
		}
		m_freeCount += num;
		m_freeIndices = array;
		for (int k = currentLength; k < _newLength; k++)
		{
			m_freeIndices[k] = k;
			m_aliveIndices[k] = -1;
			T val = new T
			{
				m_index = k
			};
			m_array[k] = val;
		}
	}

	private void DecreaseLength(int _newLength)
	{
	}

	public void Update()
	{
	}
}
