using System.Collections.Generic;
using UnityEngine;

public class MoveUndoAction : UndoAction
{
	public uint[] m_elements;

	public Vector3[] m_originalPositions;

	public Vector3[] m_newPositions;

	public MoveUndoAction(List<GraphElement> _elements, List<Vector3> _originalPositions)
	{
		m_elements = new uint[_elements.Count];
		m_newPositions = new Vector3[_elements.Count];
		m_originalPositions = _originalPositions.ToArray();
		for (int i = 0; i < _elements.Count; i++)
		{
			m_elements[i] = _elements[i].m_id;
			m_newPositions[i] = _elements[i].m_position;
		}
		UndoManager.Add(this);
	}

	public override void ApplyUndo()
	{
		for (int i = 0; i < m_elements.Length; i++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_elements[i]);
			if (element != null)
			{
				element.m_position = m_originalPositions[i];
				element.Reset();
			}
		}
	}

	public override void ApplyRedo()
	{
		for (int i = 0; i < m_elements.Length; i++)
		{
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement(m_elements[i]);
			if (element != null)
			{
				element.m_position = m_newPositions[i];
				element.Reset();
			}
		}
	}
}
