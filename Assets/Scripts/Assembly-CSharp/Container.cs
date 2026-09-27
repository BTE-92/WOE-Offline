public class Container : Item
{
	public int m_capacity;

	public int m_itemCount;

	public Item[] m_items;

	public Container(GraphElement _graphElement)
		: base(_graphElement, ItemType.Container)
	{
		m_capacity = 5;
		m_itemCount = 0;
		m_items = new Item[m_capacity];
	}
}
