using System;
using System.Collections.Generic;
using UnityEngine;

public class TransformGizmo
{
	private TransformC m_TC;

	private TransformC m_worldTC;

	public TouchAreaC m_TAC;

	private Vector3 m_touchOffset;

	private bool m_readyToMove;

	private List<Vector3> m_startPositions;

	private List<Vector3> m_startRotations;

	private List<Vector3> m_startScales;

	private float m_startAngle;

	public TransformGizmo(bool _active)
	{
		string[] tags = new string[1] { "TransformGizmo" };
		m_TC = EntityManager.AddEntityWithTC(tags);
		m_TC.transform.gameObject.name = "GizmoTC";
		m_TC.p_entity.m_persistent = true;
		m_worldTC = TransformS.AddComponent(m_TC.p_entity, "GizmoWorldTC");
		int count = PsState.m_selection.Count;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			zero += PsState.m_selection[i].m_TC.transform.position;
		}
		zero /= (float)count;
		PsState.m_selectionOffsets = new Vector3[count];
		for (int j = 0; j < count; j++)
		{
			PsState.m_selectionOffsets[j] = zero - PsState.m_selection[j].m_TC.transform.position;
		}
		Vector3 rotation = Vector3.zero;
		if (PsState.m_selection.Count == 1)
		{
			rotation = PsState.m_selection[0].m_rotation;
		}
		Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(zero);
		position.x -= (float)Screen.width * 0.5f;
		position.y -= (float)Screen.height * 0.5f;
		position.z = -10f;
		float num = 1f;
		if (!_active)
		{
			num = 0.25f;
		}
		float num2 = (float)Screen.height / 768f * 100f;
		float num3 = num2 * 2f;
		float num4 = 0.6f * num3;
		float num5 = 1f * num3;
		float num6 = 0.05f * num3;
		float num7 = 1f * num2;
		float num8 = 0.35f * num3;
		TransformC c = TransformS.AddComponent(m_TC.p_entity, "GizmoScaleXTC");
		TransformC c2 = TransformS.AddComponent(m_TC.p_entity, "GizmoScaleYTC");
		TransformC c3 = TransformS.AddComponent(m_TC.p_entity, "GizmoScaleUniformTC");
		TransformS.ParentComponent(c, m_TC, Vector3.zero);
		TransformS.ParentComponent(c2, m_TC, Vector3.zero);
		TransformS.ParentComponent(c3, m_TC, Vector3.zero);
		TransformS.SetPosition(c, Vector3.right * (num5 - 0.15f * num3) + Vector3.forward * 5f);
		TransformS.SetPosition(c2, Vector3.up * (num5 - 0.15f * num3) + Vector3.forward * 5f);
		TransformS.SetPosition(c3, new Vector3(num5 - 0.15f * num3, num5 - 0.15f * num3, 0f) * Mathf.Sin((float)Math.PI / 4f) + Vector3.forward * 5f);
		TransformC transformC = TransformS.AddComponent(m_TC.p_entity);
		TransformS.ParentComponent(transformC, m_TC, Vector3.zero);
		transformC.forceRotation = true;
		GraphElement graphElement = PsState.m_selection[0];
		if (graphElement.m_isMoveable)
		{
			m_TAC = TouchAreaS.AddCircleArea(m_TC, "Move", num2, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
			SpriteC c4 = SpriteS.AddComponent(m_TC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_selection_circle"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c4, num3, num3);
			SpriteS.ConvertSpritesToPrefabComponent(m_TC, true);
		}
		if (graphElement.m_isRotateable)
		{
			float num9 = 135f;
			TransformC transformC2 = TransformS.AddComponent(m_TC.p_entity, "GizmoRotateTC");
			TransformS.ParentComponent(transformC2, m_TC, new Vector3(num7 * Mathf.Sin(num9 * ((float)Math.PI / 180f)), num7 * Mathf.Cos(num9 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c5 = TouchAreaS.AddCircleArea(transformC2, "RotateZ", num8 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c5, TouchHandler);
			SpriteC c6 = SpriteS.AddComponent(transformC2, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_rotate"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c6, num8, num8);
			SpriteS.ConvertSpritesToPrefabComponent(transformC2, true);
		}
		if (graphElement.m_isScaleable || graphElement.m_isScaleableUniform)
		{
		}
		if (graphElement.m_isCopyable)
		{
			float num10 = -135f;
			TransformC transformC3 = TransformS.AddComponent(m_TC.p_entity, "GizmoCopyTC");
			transformC3.forceRotation = true;
			TransformS.ParentComponent(transformC3, m_TC, new Vector3(num7 * Mathf.Sin(num10 * ((float)Math.PI / 180f)), num7 * Mathf.Cos(num10 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c7 = TouchAreaS.AddCircleArea(transformC3, "Copy", num8 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c7, TouchHandler);
			SpriteC c8 = SpriteS.AddComponent(transformC3, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_copy"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c8, num8, num8);
			SpriteS.ConvertSpritesToPrefabComponent(transformC3, true);
		}
		if (graphElement.m_isRemovable)
		{
			float num11 = -45f;
			TransformC transformC4 = TransformS.AddComponent(m_TC.p_entity, "GizmoRemoveTC");
			transformC4.forceRotation = true;
			TransformS.ParentComponent(transformC4, m_TC, new Vector3(num7 * Mathf.Sin(num11 * ((float)Math.PI / 180f)), num7 * Mathf.Cos(num11 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c9 = TouchAreaS.AddCircleArea(transformC4, "Remove", num8 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c9, TouchHandler);
			SpriteC c10 = SpriteS.AddComponent(transformC4, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_delete"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c10, num8, num8);
			SpriteS.ConvertSpritesToPrefabComponent(transformC4, true);
		}
		if (graphElement.m_isFlippable)
		{
			float num12 = 0f;
			TransformC transformC5 = TransformS.AddComponent(m_TC.p_entity, "GizmoFlipTC");
			transformC5.forceRotation = true;
			TransformS.ParentComponent(transformC5, m_TC, new Vector3(num7 * Mathf.Sin(num12 * ((float)Math.PI / 180f)), num7 * Mathf.Cos(num12 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c11 = TouchAreaS.AddCircleArea(transformC5, "Flip", num8 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c11, TouchHandler);
			SpriteC c12 = SpriteS.AddComponent(transformC5, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_flip"), PsState.m_uiSheet);
			SpriteS.SetDimensions(c12, num8, num8);
			SpriteS.ConvertSpritesToPrefabComponent(transformC5, true);
		}
		if (graphElement.m_isModifiable)
		{
		}
		if (graphElement.m_isReplaceable)
		{
		}
		TransformS.SetGlobalPosition(m_TC, position);
		TransformS.SetGlobalRotation(m_TC, rotation);
		TransformS.SetGlobalPosition(m_worldTC, zero);
		for (int k = 0; k < count; k++)
		{
			ParentToGizmo(PsState.m_selection[k]);
		}
	}

	private static Vector2[] DrawHandle(TransformC _tc, float _startAngle, float _endAngle, float _spacing, float _startRadius, float _endRadius, float _roundRadius, int _fillStyle, float _alpha)
	{
		return DrawHandle(_tc, Vector3.zero, _startAngle, _endAngle, _spacing, _startRadius, _endRadius, _roundRadius, _fillStyle, _alpha);
	}

	private static Vector2[] DrawHandle(TransformC _tc, Vector3 _offset, float _startAngle, float _endAngle, float _spacing, float _startRadius, float _endRadius, float _roundRadius, int _fillStyle, float _alpha)
	{
		DebugDraw.defaultColor = new Color(1f, 1f, 1f, 1f);
		List<Vector2> list = new List<Vector2>();
		if (_startAngle != _endAngle)
		{
			float num = (_startAngle + _spacing) * ((float)Math.PI / 180f);
			float num2 = _spacing * ((float)Math.PI / 180f) * 0.5f;
			Vector2 item = new Vector2(Mathf.Cos(num + num2) * _startRadius, Mathf.Sin(num + num2) * _startRadius);
			Vector2 vector = new Vector2(Mathf.Cos(num) * (_endRadius - _roundRadius), Mathf.Sin(num) * (_endRadius - _roundRadius));
			Vector2 vector2 = new Vector2(Mathf.Cos(num + (float)Math.PI / 2f) * _roundRadius, Mathf.Sin(num + (float)Math.PI / 2f) * _roundRadius);
			float num3 = (_endAngle - _spacing) * ((float)Math.PI / 180f);
			Vector2 item2 = new Vector2(Mathf.Cos(num3 - num2) * _startRadius, Mathf.Sin(num3 - num2) * _startRadius);
			Vector2 vector3 = new Vector2(Mathf.Cos(num3) * (_endRadius - _roundRadius), Mathf.Sin(num3) * (_endRadius - _roundRadius));
			Vector2 vector4 = new Vector2(Mathf.Cos(num3 - (float)Math.PI / 2f) * _roundRadius, Mathf.Sin(num3 - (float)Math.PI / 2f) * _roundRadius);
			Vector2[] line = DebugDraw.GetLine(vector, vector + vector2, 0);
			Vector2[] line2 = DebugDraw.GetLine(vector3, vector3 + vector4, 0);
			float num4 = Mathf.Atan2(line[1].y, line[1].x) * 57.29578f;
			float num5 = Mathf.Atan2(line2[1].y, line2[1].x) * 57.29578f;
			Vector2[] arc = DebugDraw.GetArc(_roundRadius, 9, num4 - _startAngle + 90f, _startAngle - 90f, line[1]);
			Vector2[] arc2 = DebugDraw.GetArc(_roundRadius, 9, num4 - _startAngle + 90f, num5, line2[1]);
			float magnitude = arc[0].magnitude;
			float num6 = num5 - num4;
			if (num6 < 0f)
			{
				num6 = 360f + num6;
			}
			Vector2[] arc3 = DebugDraw.GetArc(magnitude, 36, num6, num4, Vector2.zero);
			list.Add(item2);
			list.AddRange(arc2);
			list.RemoveAt(list.Count - 1);
			list.AddRange(arc3);
			list.RemoveAt(list.Count - 1);
			list.AddRange(arc);
			list.Add(item);
			switch (_fillStyle)
			{
			case 0:
				PrefabS.CreateLinePrefabComponentFromVectorArray(_tc, _offset + Vector3.forward * -10f, list.ToArray(), 4f, new Color(1f, 1f, 1f, 1f * _alpha), ResourceManager.GetMaterial("Framework/Line6Mat"), CameraS.m_uiCamera, Position.Center);
				break;
			case 1:
			{
				PrefabS.CreateLinePrefabComponentFromVectorArray(_tc, _offset + Vector3.forward * -10f, list.ToArray(), 4f, new Color(1f, 1f, 1f, 1f * _alpha), ResourceManager.GetMaterial("Framework/Line6Mat"), CameraS.m_uiCamera, Position.Center);
				Polygon polygon3 = new Polygon();
				polygon3.AddContour(new VertexList(list.ToArray()), false);
				Vector2[] circle = DebugDraw.GetCircle(_endRadius - 12f, 36, Vector2.zero, false);
				Polygon polygon4 = new Polygon();
				polygon4.AddContour(new VertexList(circle), false);
				Vector2[] circle2 = DebugDraw.GetCircle(_endRadius - 6f, 36, Vector2.zero, false);
				Polygon polygon5 = new Polygon();
				polygon5.AddContour(new VertexList(circle2), false);
				Polygon polygon6 = polygon3.Clip(GpcOperation.Difference, polygon5);
				polygon3 = polygon3.Clip(GpcOperation.Difference, polygon4);
				polygon3 = polygon3.Clip(GpcOperation.Intersection, polygon5);
				PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon3, new Color(1f, 1f, 1f, 0.2f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
				PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon6, new Color(0f, 0f, 0f, 0.15f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
				TransformS.SetRotation(_tc, Vector3.zero);
				break;
			}
			case 2:
			{
				PrefabS.CreateLinePrefabComponentFromVectorArray(_tc, _offset + Vector3.forward * -10f, list.ToArray(), 4f, new Color(1f, 1f, 1f, 1f * _alpha), ResourceManager.GetMaterial("Framework/Line6Mat"), CameraS.m_uiCamera, Position.Center);
				Vector2[] p = DrawHandle(_tc, _startAngle, _endAngle, 6f, _startRadius, _endRadius - 6f, _roundRadius * 0.5f, -1, _alpha);
				Vector2[] p2 = DrawHandle(_tc, _startAngle, _endAngle, 10f, _startRadius, _endRadius - 12f, _roundRadius * 0.25f, -1, _alpha);
				Polygon polygon = new Polygon();
				polygon.AddContour(new VertexList(p), false);
				polygon.AddContour(new VertexList(p2), true);
				Polygon polygon2 = new Polygon();
				polygon2.AddContour(new VertexList(list.ToArray()), false);
				polygon2.AddContour(new VertexList(p), true);
				PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon, new Color(1f, 1f, 1f, 0.2f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
				PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon2, new Color(0f, 0f, 0f, 0.15f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
				TransformS.SetRotation(_tc, Vector3.zero);
				break;
			}
			}
		}
		else
		{
			Vector2[] circle3 = DebugDraw.GetCircle(_endRadius, 36, Vector2.zero, true);
			list.AddRange(circle3);
			PrefabS.CreateLinePrefabComponentFromVectorArray(_tc, _offset + Vector3.forward * -10f, list.ToArray(), 4f, new Color(1f, 1f, 1f, 1f * _alpha), ResourceManager.GetMaterial("Framework/Line6Mat"), CameraS.m_uiCamera, Position.Center);
			Polygon polygon7 = new Polygon();
			polygon7.AddContour(new VertexList(list.ToArray()), false);
			Vector2[] circle4 = DebugDraw.GetCircle(_endRadius - 12f, 36, Vector2.zero, false);
			Polygon polygon8 = new Polygon();
			polygon8.AddContour(new VertexList(circle4), false);
			Polygon polygon9 = polygon7.Clip(GpcOperation.Difference, polygon8);
			polygon7 = polygon7.Clip(GpcOperation.Difference, polygon8);
			float num7 = (float)Math.PI / 12f;
			float num8 = _endRadius - 6f;
			Vector2[] p3 = new Vector2[4]
			{
				new Vector2(-12f, 3f),
				new Vector2(12f, 7f),
				new Vector2(12f, -6f),
				new Vector2(-12f, -6f)
			};
			for (int i = 0; i < 24; i++)
			{
				polygon8 = new Polygon();
				polygon8.AddContour(new VertexList(p3), false);
				Vector2 pos = new Vector2(Mathf.Cos(num7 * (float)i) * num8, Mathf.Sin(num7 * (float)i) * num8);
				polygon8 = DebugDraw.TransformPolygon(polygon8, pos, 12 + i * 15);
				polygon9 = polygon9.Clip(GpcOperation.Difference, polygon8);
			}
			Polygon polygon10 = polygon7.Clip(GpcOperation.Difference, polygon9);
			PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon9, new Color(1f, 1f, 1f, 0.2f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
			PrefabS.CreateFlatPrefabComponentsFromPolygon(_tc, _offset + Vector3.forward * -5f, polygon10, new Color(0f, 0f, 0f, 0.15f * _alpha), ResourceManager.GetMaterial("Framework/SolidMat"), CameraS.m_uiCamera);
		}
		return list.ToArray();
	}

	public void Destroy(bool _clearSelection = true)
	{
		UnparentFromGizmo();
		if (_clearSelection)
		{
			for (int i = 0; i < PsState.m_selection.Count; i++)
			{
				PsState.m_selection[i].m_selected = false;
			}
			PsState.m_selection.Clear();
		}
		if (m_TC != null)
		{
			EntityManager.RemoveEntity(m_TC.p_entity, true, true);
		}
		m_TC = null;
		PsState.m_transformGizmo = null;
	}

	public void Update()
	{
		if (PsState.m_selection.Count > 0)
		{
			int count = PsState.m_selection.Count;
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				zero += PsState.m_selection[i].m_TC.transform.position;
			}
			zero /= (float)count;
			PsState.m_selectionOffsets = new Vector3[count];
			for (int j = 0; j < count; j++)
			{
				PsState.m_selectionOffsets[j] = zero - PsState.m_selection[j].m_TC.transform.position;
			}
			Vector3 rotation = Vector3.zero;
			if (PsState.m_selection.Count == 1)
			{
				rotation = PsState.m_selection[0].m_rotation;
			}
			Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(zero);
			position.x -= (float)Screen.width * 0.5f;
			position.y -= (float)Screen.height * 0.5f;
			position.z = -10f;
			List<TransformC> childs = m_worldTC.childs;
			for (int k = 0; k < childs.Count; k++)
			{
				childs[k].transform.parent = null;
			}
			TransformS.SetGlobalPosition(m_TC, position);
			TransformS.SetGlobalRotation(m_TC, rotation);
			TransformS.SetGlobalPosition(m_worldTC, zero);
			for (int l = 0; l < childs.Count; l++)
			{
				childs[l].transform.parent = m_worldTC.transform;
			}
			for (int m = 0; m < count; m++)
			{
				ParentToGizmo(PsState.m_selection[m]);
			}
		}
		else
		{
			Destroy();
		}
	}

	public void UpdatePosition()
	{
		if (PsState.m_selection.Count > 0)
		{
			int count = PsState.m_selection.Count;
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				zero += PsState.m_selection[i].m_TC.transform.position;
			}
			zero /= (float)count;
			PsState.m_selectionOffsets = new Vector3[count];
			for (int j = 0; j < count; j++)
			{
				PsState.m_selectionOffsets[j] = zero - PsState.m_selection[j].m_TC.transform.position;
			}
			Vector3 rotation = Vector3.zero;
			if (PsState.m_selection.Count == 1)
			{
				rotation = PsState.m_selection[0].m_rotation;
			}
			Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(zero);
			position.x -= (float)Screen.width * 0.5f;
			position.y -= (float)Screen.height * 0.5f;
			position.z = -10f;
			TransformS.SetGlobalPosition(m_TC, position);
			TransformS.SetGlobalRotation(m_TC, rotation);
			TransformS.SetGlobalPosition(m_worldTC, zero);
		}
	}

	public void ParentToGizmo(GraphElement _element)
	{
		for (int i = 0; i < _element.m_assembledClasses.Count; i++)
		{
			IAssembledClass assembledClass = _element.m_assembledClasses[i];
			for (int j = 0; j < assembledClass.m_assembledEntities.Count; j++)
			{
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[j]);
				for (int k = 0; k < componentsByEntity.Count; k++)
				{
					ChipmunkBodyC chipmunkBodyC = componentsByEntity[k] as ChipmunkBodyC;
					chipmunkBodyC.m_active = false;
				}
				List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[j]);
				for (int l = 0; l < componentsByEntity2.Count; l++)
				{
					TransformC transformC = componentsByEntity2[l] as TransformC;
					if (transformC.parent == null)
					{
						TransformS.ParentComponent(transformC, m_worldTC);
					}
				}
			}
		}
	}

	public void UnparentFromGizmo()
	{
		for (int i = 0; i < PsState.m_selection.Count; i++)
		{
			GraphElement graphElement = PsState.m_selection[i];
			for (int j = 0; j < graphElement.m_assembledClasses.Count; j++)
			{
				IAssembledClass assembledClass = graphElement.m_assembledClasses[j];
				for (int k = 0; k < assembledClass.m_assembledEntities.Count; k++)
				{
					List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[k]);
					for (int l = 0; l < componentsByEntity.Count; l++)
					{
						ChipmunkBodyC chipmunkBodyC = componentsByEntity[l] as ChipmunkBodyC;
						chipmunkBodyC.m_active = chipmunkBodyC.m_wasActive;
					}
					List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[k]);
					for (int m = 0; m < componentsByEntity2.Count; m++)
					{
						TransformC transformC = componentsByEntity2[m] as TransformC;
						if (transformC.parent == m_worldTC)
						{
							TransformS.UnparentComponent(transformC);
						}
					}
				}
			}
		}
	}

	private void TouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchIsSecondary[0] || _touchCount != 1)
		{
			return;
		}
		if (_touchArea.m_name == "Copy")
		{
			if (_touchPhases[0] == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list = new List<GraphElement>();
				for (int i = 0; i < PsState.m_selection.Count; i++)
				{
					GraphElement graphElement = PsState.m_selection[i].DeepCopy();
					graphElement.m_id = LevelManager.GetUniqueId();
					graphElement.m_position += Vector3.right * 10f + Vector3.up * -10f;
					LevelManager.m_currentLevel.m_currentLayer.AddElement(graphElement);
					graphElement.Assemble();
					list.Add(graphElement);
				}
				new CopyUndoAction(PsState.m_selection.ToArray(), list.ToArray());
				Destroy();
				PsState.m_selection = list;
				PsState.m_transformGizmo = new TransformGizmo(true);
			}
		}
		else if (_touchArea.m_name == "Remove")
		{
			if (_touchPhases[0] == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list2 = new List<GraphElement>();
				while (PsState.m_selection.Count > 0)
				{
					int index = PsState.m_selection.Count - 1;
					list2.Add(PsState.m_selection[index].DeepCopy());
					PsState.m_selection[index].Dispose();
					PsState.m_selection.RemoveAt(index);
				}
				Update();
				new RemoveUndoAction(list2.ToArray());
			}
		}
		else if (_touchArea.m_name == "Flip")
		{
			if (_touchPhases[0] == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list3 = new List<GraphElement>();
				for (int j = 0; j < PsState.m_selection.Count; j++)
				{
					PsState.m_selection[j].Flip();
					list3.Add(PsState.m_selection[j]);
				}
				Update();
				new FlipUndoAction(list3.ToArray());
			}
		}
		else if (_touchArea.m_name == "Move")
		{
			if (_touchPhases[0] == TouchAreaPhase.Began || _touchPhases[0] == TouchAreaPhase.DragStart)
			{
				if (!m_readyToMove)
				{
					Vector3 vector = _touches[0].m_currentPosition;
					vector.x -= (float)Screen.width * 0.5f;
					vector.y -= (float)Screen.height * 0.5f;
					vector.z = -10f;
					m_touchOffset = m_TC.transform.position - vector;
					m_readyToMove = true;
					m_startPositions = new List<Vector3>();
					for (int k = 0; k < PsState.m_selection.Count; k++)
					{
						m_startPositions.Add(PsState.m_selection[k].m_position);
					}
				}
			}
			else if (m_readyToMove && (_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.MoveOut || _touchPhases[0] == TouchAreaPhase.StationaryIn || _touchPhases[0] == TouchAreaPhase.StationaryOut))
			{
				Vector3 vector2 = (Vector3)_touches[0].m_currentPosition + m_touchOffset;
				vector2.x -= (float)Screen.width * 0.5f;
				vector2.y -= (float)Screen.height * 0.5f;
				vector2.z = -10f;
				if (_touchArea.m_isDragged)
				{
					float num = (float)Screen.height * 0.2f;
					Vector3 position = EditorBaseState.m_camTarget.TC.transform.position;
					bool flag = false;
					if (vector2.x < (float)Screen.width * -0.5f + num || vector2.x > (float)Screen.width * 0.5f - num)
					{
						if (vector2.x < 0f)
						{
							position.x -= (num - (vector2.x + (float)Screen.width * 0.5f)) * 0.25f;
						}
						else
						{
							position.x += (num - ((float)Screen.width * 0.5f - vector2.x)) * 0.25f;
						}
						position.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f, position.x));
						flag = true;
					}
					if (vector2.y < (float)Screen.height * -0.5f + num || vector2.y > (float)Screen.height * 0.5f - num)
					{
						if (vector2.y < 0f)
						{
							position.y -= (num - (vector2.y + (float)Screen.height * 0.5f)) * 0.25f;
						}
						else
						{
							position.y += (num - ((float)Screen.height * 0.5f - vector2.y)) * 0.25f;
						}
						position.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, position.y));
						flag = true;
					}
					if (flag)
					{
						EditorBaseState.m_camTarget.TC.transform.position = position;
					}
				}
				Vector3 vector3 = vector2 - m_TC.transform.position;
				Vector3 step = vector3 * 0.382f;
				TransformS.GlobalMove(m_TC, step);
				Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_TC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
				touchWorldPos.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f - 50f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f + 50f, touchWorldPos.x));
				touchWorldPos.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f - 50f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 50f, touchWorldPos.y));
				TransformS.SetGlobalPosition(m_worldTC, touchWorldPos);
				for (int l = 0; l < PsState.m_selection.Count; l++)
				{
					GraphElement graphElement2 = PsState.m_selection[l];
					Vector3 position2 = touchWorldPos + PsState.m_selectionOffsets[l];
					TransformS.SetGlobalPosition(graphElement2.m_TC, position2);
					graphElement2.m_position = position2;
					for (int m = 0; m < graphElement2.m_assembledClasses.Count; m++)
					{
						IAssembledClass assembledClass = graphElement2.m_assembledClasses[m];
						for (int n = 0; n < assembledClass.m_assembledEntities.Count; n++)
						{
							List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[n]);
							for (int num2 = 0; num2 < componentsByEntity.Count; num2++)
							{
								ChipmunkBodyC chipmunkBodyC = componentsByEntity[num2] as ChipmunkBodyC;
								ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position);
							}
						}
					}
				}
			}
			else if (_touchPhases[0] == TouchAreaPhase.DragEnd)
			{
				new MoveUndoAction(PsState.m_selection, m_startPositions);
			}
			else if (_touchPhases[0] == TouchAreaPhase.ReleaseIn || _touchPhases[0] == TouchAreaPhase.ReleaseOut)
			{
				m_readyToMove = false;
			}
		}
		else
		{
			if (!(_touchArea.m_name == "RotateZ"))
			{
				return;
			}
			Vector3 touchWorldPos2 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_TC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			Vector3 vector4 = _touches[0].m_currentPosition;
			Vector3 touchWorldPos3 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, vector4);
			if (_touchPhases[0] == TouchAreaPhase.Began)
			{
				m_startRotations = new List<Vector3>();
				for (int num3 = 0; num3 < PsState.m_selection.Count; num3++)
				{
					m_startRotations.Add(PsState.m_selection[num3].m_rotation);
				}
				Vector2 vector5 = touchWorldPos3 - touchWorldPos2;
				float startAngle = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
				m_startAngle = startAngle;
			}
			else if (_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.MoveOut || _touchPhases[0] == TouchAreaPhase.StationaryIn || _touchPhases[0] == TouchAreaPhase.StationaryOut)
			{
				Vector2 vector6 = touchWorldPos3 - touchWorldPos2;
				float num4 = Mathf.Atan2(vector6.y, vector6.x) * 57.29578f;
				for (int num5 = 0; num5 < PsState.m_selection.Count; num5++)
				{
					GraphElement graphElement3 = PsState.m_selection[num5];
					graphElement3.m_rotation = Vector3.forward * (num4 - m_startAngle) + m_startRotations[num5];
					for (int num6 = 0; num6 < graphElement3.m_assembledClasses.Count; num6++)
					{
						IAssembledClass assembledClass2 = graphElement3.m_assembledClasses[num6];
						for (int num7 = 0; num7 < assembledClass2.m_assembledEntities.Count; num7++)
						{
							List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass2.m_assembledEntities[num7]);
							for (int num8 = 0; num8 < componentsByEntity2.Count; num8++)
							{
								ChipmunkBodyC chipmunkBodyC2 = componentsByEntity2[num8] as ChipmunkBodyC;
								ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC2.body, ToolBox.getCappedAngle(graphElement3.m_rotation.z) * ((float)Math.PI / 180f));
							}
							List<IComponent> componentsByEntity3 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass2.m_assembledEntities[num7]);
							for (int num9 = 0; num9 < componentsByEntity3.Count; num9++)
							{
								TransformC c = componentsByEntity3[num9] as TransformC;
								TransformS.SetGlobalRotation(c, graphElement3.m_rotation);
							}
						}
					}
				}
			}
			else if (_touchPhases[0] == TouchAreaPhase.DragEnd)
			{
				new RotateUndoAction(PsState.m_selection, m_startRotations);
			}
		}
	}
}
