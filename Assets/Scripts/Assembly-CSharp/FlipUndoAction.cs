public class FlipUndoAction : UndoAction
{
	public uint[] m_elements;

	public FlipUndoAction(GraphElement[] _elements)
	{
		m_elements = new uint[_elements.Length];
		for (int i = 0; i < _elements.Length; i++)
		{
			m_elements[i] = _elements[i].m_id;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		Debug.LogWarning("not implemented");
	}

	public override void ApplyRedo()
	{
		Debug.LogWarning("not implemented");
	}
}
