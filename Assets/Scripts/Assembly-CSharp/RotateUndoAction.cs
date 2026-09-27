using System.Collections.Generic;
using UnityEngine;

public class RotateUndoAction : UndoAction
{
	public uint[] m_elements;

	public Vector3[] m_originalRotations;

	public Vector3[] m_newRotations;

	public RotateUndoAction(List<GraphElement> _elements, List<Vector3> _originalRotations)
	{
		m_elements = new uint[_elements.Count];
		m_newRotations = new Vector3[_elements.Count];
		m_originalRotations = _originalRotations.ToArray();
		for (int i = 0; i < _elements.Count; i++)
		{
			m_elements[i] = _elements[i].m_id;
			m_newRotations[i] = _elements[i].m_rotation;
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
				element.m_rotation = m_originalRotations[i];
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
				element.m_rotation = m_newRotations[i];
				element.Reset();
			}
		}
	}
}
