public class UnitC : BasicComponent
{
	public Unit m_unit;

	public UnitC()
		: base((ComponentType)30)
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
	}

	public override void Destroy()
	{
		m_unit.Destroy();
		m_unit = null;
	}
}
