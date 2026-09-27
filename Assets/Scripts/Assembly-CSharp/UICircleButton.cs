using System;
using UnityEngine;

public class UICircleButton : UIComponent
{
	public static float m_defaultRadius = 0.1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public UICircleButton(UIComponent _parent, string _tag, Camera _camera)
		: base(_parent, false, _tag, _camera, null, string.Empty)
	{
		SetWidth(m_defaultRadius, m_defaultWidthRelativeTo);
		SetHeight(m_defaultRadius, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_TAC = TouchAreaS.AddCircleArea(m_TC, _tag, m_defaultRadius, m_camera);
		TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		float radius = Math.Min(m_actualWidth, m_actualHeight) * 0.5f;
		Vector2[] circle = DebugDraw.GetCircle(radius, 16, Vector2.zero, true);
		Color color = new Color(0.4f, 0.4f, 0.4f);
		if (m_highlight)
		{
			color = new Color(0.3f, 1f, 0.3f);
		}
		Camera camera = CameraS.m_uiCamera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		PrefabS.CreateLinePrefabComponentFromVectorArray(m_TC, Vector3.zero, circle, 2f, color, ResourceManager.GetMaterial("Framework/SolidMat"), camera, Position.Center);
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, camera, true);
	}
}
