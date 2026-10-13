using System;
using UnityEngine;

// The original class was an empty stub (no shape, no visual). This gives it a working quarter-pipe ramp:
// a static curved surface made of segment shapes, drawn with debug lines. It can be rotated in the editor.
public class ProtoRampCurved : Unit
{
	private const float RADIUS = 300f;

	private const int SEGMENTS = 16;

	private ChipmunkBodyC m_cmb;

	public ProtoRampCurved(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		Vector2[] points = new Vector2[SEGMENTS + 1];
		for (int i = 0; i <= SEGMENTS; i++)
		{
			float t = (float)i / (float)SEGMENTS * ((float)Math.PI / 2f);
			// concave quarter circle: starts flat at the bottom-left, ends vertical at the right
			points[i] = new Vector2(Mathf.Sin(t) * RADIUS - RADIUS / 2f, RADIUS - Mathf.Cos(t) * RADIUS - RADIUS / 2f);
		}
		ucpShape[] shapes = new ucpShape[SEGMENTS];
		for (int j = 0; j < SEGMENTS; j++)
		{
			shapes[j] = new ucpSegmentShape(points[j], points[j + 1], 6f, Vector2.zero, 1f, 0.25f, 0.9f, (ucpCollisionType)4);
		}
		m_cmb = ChipmunkProS.AddStaticBody(transformC, shapes, m_unitC);
		for (int k = 0; k < SEGMENTS; k++)
		{
			DebugDraw.CreateLine(CameraS.m_mainCamera, transformC, points[k], points[k + 1]);
		}
		CreateEditorTouchArea(RADIUS, RADIUS);
		m_graphElement.m_isRotateable = true;
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
