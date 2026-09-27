public class ProtoRampCurved : Unit
{
	public ProtoRampCurved(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
