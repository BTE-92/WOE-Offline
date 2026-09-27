using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasicAssembledClass : IAssembledClass
{
	private GraphElement _graphElement;

	private List<Entity> _assembledEntities;

	public GraphElement m_graphElement
	{
		get
		{
			return _graphElement;
		}
		set
		{
			_graphElement = value;
		}
	}

	public List<Entity> m_assembledEntities
	{
		get
		{
			return _assembledEntities;
		}
		set
		{
			_assembledEntities = value;
		}
	}

	public BasicAssembledClass(GraphElement _graphElement)
	{
		m_assembledEntities = new List<Entity>();
		m_graphElement = _graphElement;
	}

	public virtual void CreateGraphElementTouchArea(Mesh _collisionMesh)
	{
		if (m_graphElement.m_TAC != null)
		{
			Debug.LogError("Trying to add multiple touch areas to GraphElement.");
			return;
		}
		m_graphElement.m_isRect = true;
		m_graphElement.m_width = _collisionMesh.bounds.extents.x;
		m_graphElement.m_width = _collisionMesh.bounds.extents.y;
		m_graphElement.m_TAC = TouchAreaS.AddMeshArea(m_graphElement.m_TC, "Select", _collisionMesh, CameraS.m_mainCamera, true, m_graphElement.m_TC);
		TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
	}

	public virtual void CreateGraphElementTouchArea(float _graphElementRadius)
	{
		if (m_graphElement.m_TAC != null)
		{
			Debug.LogError("Trying to add multiple touch areas to GraphElement.");
			return;
		}
		m_graphElement.m_isRect = false;
		m_graphElement.m_radius = _graphElementRadius;
		m_graphElement.m_TAC = TouchAreaS.AddCircleArea(m_graphElement.m_TC, "Select", _graphElementRadius, CameraS.m_mainCamera, true, m_graphElement.m_TC);
		TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
	}

	public virtual void CreateGraphElementTouchArea(float _graphElementWidth, float _graphElementHeight)
	{
		if (m_graphElement.m_TAC != null)
		{
			Debug.LogError("Trying to add multiple touch areas to GraphElement.");
			return;
		}
		m_graphElement.m_isRect = true;
		m_graphElement.m_width = _graphElementWidth;
		m_graphElement.m_height = _graphElementHeight;
		m_graphElement.m_TAC = TouchAreaS.AddRectArea(m_graphElement.m_TC, "Select", _graphElementWidth, _graphElementHeight, CameraS.m_mainCamera, true, m_graphElement.m_TC);
		TouchAreaS.AddTouchEventListener(m_graphElement.m_TAC, SelectionTouchHandler);
	}

	public virtual void Destroy()
	{
		while (m_assembledEntities.Count > 0)
		{
			int index = m_assembledEntities.Count - 1;
			if (m_assembledEntities[index].m_index > -1)
			{
				EntityManager.RemoveEntity(m_assembledEntities[index]);
			}
			m_assembledEntities.RemoveAt(index);
		}
		m_graphElement.m_assembledClasses.Remove(this);
	}

	protected void SelectionTouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchCount != 1 || _touchPhases[0] != TouchAreaPhase.ReleaseIn || _touchIsSecondary[0])
		{
			return;
		}
		List<GraphElement> oldElements = new List<GraphElement>(PsState.m_selection.ToArray());
		if (PsState.m_selection.Contains(m_graphElement))
		{
			if (PsState.m_specialDown)
			{
				m_graphElement.m_selected = false;
				PsState.m_selection.Remove(m_graphElement);
				PsState.m_transformGizmo.Update();
			}
		}
		else
		{
			if (!PsState.m_specialDown && PsState.m_transformGizmo != null)
			{
				PsState.m_transformGizmo.Destroy();
			}
			PsState.m_selection.Add(m_graphElement);
			m_graphElement.m_selected = true;
			if (PsState.m_transformGizmo == null)
			{
				PsState.m_transformGizmo = new TransformGizmo(true);
				if (PsState.m_selection.Count == 1 && PsState.m_transformGizmo.m_TAC != null)
				{
					PsState.m_transformGizmo.m_TAC.m_touchCount = 0;
					_touches[0].m_primaryArea = PsState.m_transformGizmo.m_TAC;
				}
			}
			else
			{
				PsState.m_transformGizmo.Update();
			}
		}
		new SelectUndoAction(oldElements, PsState.m_selection);
	}
}
