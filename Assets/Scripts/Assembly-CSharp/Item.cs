public class Item : BasicAssembledClass
{
	public ItemC m_itemC;

	public ItemType m_itemType;

	public float m_durability;

	public float m_maxDurability;

	public float m_maxHealthModifer;

	public float m_maxEnergyModifier;

	public float[] m_shieldModifier;

	public float[] m_armorModifier;

	public Item(GraphElement _graphElement, ItemType _itemType)
		: base(_graphElement)
	{
		m_itemType = _itemType;
	}

	public virtual void Update()
	{
	}

	public virtual void Use()
	{
	}

	public virtual void PickUp()
	{
	}

	public virtual void Discard()
	{
	}
}
