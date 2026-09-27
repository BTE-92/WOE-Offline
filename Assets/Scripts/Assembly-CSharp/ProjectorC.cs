using UnityEngine;

public class ProjectorC : BasicComponent
{
	public static int m_componentCount;

	public TransformC p_TC;

	public Projector m_projector;

	public Vector3 m_offset;

	public ProjectorC()
		: base(ComponentType.Projector)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		m_projector = null;
		m_offset = Vector3.zero;
		p_TC = null;
	}

	~ProjectorC()
	{
		m_componentCount--;
	}
}
