using System.Collections.Generic;

public class RemoveUndoAction : UndoAction
{
	public GraphElement[] m_elements;

	public uint[] m_elementIDs;

	public RemoveUndoAction(GraphElement[] _elements)
	{
		m_elements = _elements;
		m_elementIDs = new uint[_elements.Length];
		for (int i = 0; i < _elements.Length; i++)
		{
			m_elementIDs[i] = _elements[i].m_id;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		List<GraphElement> list = new List<GraphElement>();
		for (int i = 0; i < m_elements.Length; i++)
		{
			GraphElement graphElement = m_elements[i].DeepCopy();
			LevelManager.m_currentLevel.m_currentLayer.AddElement(graphElement);
			graphElement.Assemble();
			list.Add(graphElement);
		}
		PsState.m_selection = list;
	}

	public override void ApplyRedo()
	{
		for (int i = 0; i < m_elements.Length; i++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_elementIDs[i]);
			if (element != null)
			{
				LevelManager.m_currentLevel.m_currentLayer.RemoveElement(element);
				element.Dispose();
			}
		}
		PsState.m_selection.Clear();
	}
}
