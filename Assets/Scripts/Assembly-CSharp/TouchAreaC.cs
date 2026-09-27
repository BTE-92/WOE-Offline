using UnityEngine;

public class TouchAreaC : BasicComponent
{
	public TransformC m_TC;

	public MeshCollider m_collider;

	public ColliderShape m_colliderShape;

	public Camera m_camera;

	public bool m_consume;

	public bool m_allowSecondary;

	public bool m_isDragged;

	public bool m_wasDragged;

	public float m_dragThreshold;

	public Vector2 m_dragOffset;

	public string m_name;

	public int m_maxTouches;

	public int m_touchCount;

	public IComponent m_customComponent;

	public int m_delegatedCount;

	public TouchEventDelegate d_TouchEventDelegate;

	public bool m_clip;

	public cpBB m_clipBB;

	public TouchAreaC()
		: base(ComponentType.TouchArea)
	{
		m_clipBB = default(cpBB);
	}

	public override void Reset()
	{
		base.Reset();
		m_allowSecondary = false;
		m_maxTouches = 1;
		m_touchCount = 0;
		m_clip = false;
		m_isDragged = false;
		m_wasDragged = false;
		if (Screen.height < Screen.width)
		{
			m_dragThreshold = 0.015f * (float)Screen.height;
		}
		else
		{
			m_dragThreshold = 0.015f * (float)Screen.width;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
