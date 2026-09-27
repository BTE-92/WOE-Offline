using System.Collections.Generic;

public class SelectUndoAction : UndoAction
{
	public uint[] m_oldElements;

	public uint[] m_newElements;

	public SelectUndoAction(List<GraphElement> _oldElements, List<GraphElement> _newElements)
	{
		m_oldElements = new uint[_oldElements.Count];
		m_newElements = new uint[_newElements.Count];
		for (int i = 0; i < _oldElements.Count; i++)
		{
			m_oldElements[i] = _oldElements[i].m_id;
		}
		for (int j = 0; j < _newElements.Count; j++)
		{
			m_newElements[j] = _newElements[j].m_id;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		for (int i = 0; i < PsState.m_selection.Count; i++)
		{
			PsState.m_selection[i].m_selected = false;
		}
		PsState.m_selection.Clear();
		for (int j = 0; j < m_oldElements.Length; j++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_oldElements[j]);
			if (element != null)
			{
				element.m_selected = true;
				PsState.m_selection.Add(element);
			}
		}
	}

	public override void ApplyRedo()
	{
		for (int i = 0; i < PsState.m_selection.Count; i++)
		{
			PsState.m_selection[i].m_selected = false;
		}
		PsState.m_selection.Clear();
		for (int j = 0; j < m_newElements.Length; j++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_newElements[j]);
			if (element != null)
			{
				element.m_selected = true;
				PsState.m_selection.Add(element);
			}
		}
	}
}
