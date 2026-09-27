public class ItemC : BasicComponent
{
	public Item m_item;

	public bool m_canPickUp;

	public bool m_canUse;

	public ItemC()
		: base((ComponentType)31)
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
	}

	public override void Destroy()
	{
		m_item.Destroy();
		m_item = null;
	}
}
