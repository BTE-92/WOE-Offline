using UnityEngine;

public class TLTouch : IPoolable
{
	private int _index;

	public int m_fingerId;

	public Vector2 m_startPosition;

	public Vector2 m_currentPosition;

	public Vector2 m_deltaPosition;

	public TouchPhase m_phase;

	public int m_tapCount;

	public bool m_consumed;

	public TouchAreaC m_primaryArea;

	public TouchAreaPhase m_primaryPhase;

	public float m_primaryAreaDepth;

	public TouchAreaC m_secondaryArea;

	public TouchAreaPhase m_secondaryPhase;

	public float m_secondaryAreaDepth;

	public bool m_secondaryLocked;

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

	public void Reset()
	{
		m_consumed = false;
		m_secondaryLocked = false;
		m_secondaryPhase = TouchAreaPhase.RollOut;
		m_primaryPhase = TouchAreaPhase.Began;
	}

	public void Destroy()
	{
	}
}
