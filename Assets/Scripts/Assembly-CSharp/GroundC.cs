public class GroundC : BasicComponent
{
	public Ground m_ground;

	public GroundC()
		: base((ComponentType)32)
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
	}

	public override void Destroy()
	{
		m_ground.Destroy();
		m_ground = null;
	}
}
