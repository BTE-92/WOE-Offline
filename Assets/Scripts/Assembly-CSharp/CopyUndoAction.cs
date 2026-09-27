using System.Collections.Generic;

public class CopyUndoAction : UndoAction
{
	public GraphElement[] m_elements;

	public uint[] m_originalElementIDs;

	public uint[] m_elementIDs;

	public CopyUndoAction(GraphElement[] _originalElements, GraphElement[] _copiedElements)
	{
		m_elements = _copiedElements;
		m_elementIDs = new uint[_copiedElements.Length];
		m_originalElementIDs = new uint[_originalElements.Length];
		for (int i = 0; i < _copiedElements.Length; i++)
		{
			m_elementIDs[i] = _copiedElements[i].m_id;
			m_originalElementIDs[i] = _originalElements[i].m_id;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		Debug.Log("undo copy");
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
		for (int j = 0; j < m_originalElementIDs.Length; j++)
		{
			GraphElement element2 = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_originalElementIDs[j]);
			PsState.m_selection.Add(element2);
		}
	}

	public override void ApplyRedo()
	{
		Debug.Log("redo copy");
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
}
