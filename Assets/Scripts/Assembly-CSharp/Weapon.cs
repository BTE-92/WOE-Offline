public class Weapon : Item
{
	public WeaponType m_weaponType;

	public Weapon(GraphElement _graphElement)
		: base(_graphElement, ItemType.Weapon)
	{
	}
}
