public class WoodenCrate : Crate
{
	private const float SIZE = 40f;

	private const float FRICTION = 0.6f;

	private const float ELASTICITY = 0.25f;

	private const float WEIGHT = 5f;

	private const string RESOURCE = "Units/WoodenCratePrefab";

	public WoodenCrate(GraphElement _graphElement)
		: base(_graphElement, 40f, 0.6f, 0.25f, 5f, "Units/WoodenCratePrefab")
	{
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		m_graphElement.m_isRotateable = true;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Impact, 35f);
		SetBaseArmor(DamageType.Weapon, 20f);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
