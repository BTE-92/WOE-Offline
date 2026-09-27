using UnityEngine;

public class UIHidingVerticalListBar : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public Vector3 m_originalPosition;

	public bool m_posSaved;

	public float m_lowestCameraPosition;

	public float m_highestCameraPosition;

	public float m_previousCameraPosition;

	public UIHidingVerticalListBar(UIVerticalList _parent, string _tag = "")
		: base(_parent, true, _tag, null, null, null)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_depthOffset = -5f;
	}

	public override void Update()
	{
		base.Update();
		m_posSaved = false;
	}

	public override void Step()
	{
		base.Step();
		if (!m_posSaved)
		{
			m_originalPosition = m_TC.transform.localPosition;
			m_posSaved = true;
		}
		float y = m_camera.transform.parent.localPosition.y;
		if (y < m_lowestCameraPosition)
		{
			m_lowestCameraPosition = y;
			if (m_highestCameraPosition - y > m_actualHeight)
			{
				m_highestCameraPosition = y + m_actualHeight;
			}
		}
		else if (y > m_highestCameraPosition)
		{
			m_highestCameraPosition = y;
			if (m_lowestCameraPosition - y < m_actualHeight)
			{
				m_lowestCameraPosition = y;
			}
		}
		if (m_lowestCameraPosition > 0f)
		{
			m_highestCameraPosition = y;
		}
		float num = y + m_highestCameraPosition - y;
		Vector3 originalPosition = m_originalPosition;
		originalPosition.y += num;
		TransformS.SetPosition(m_TC, originalPosition);
		m_previousCameraPosition = y;
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth + m_parent.m_margins.l + m_parent.m_margins.r, m_actualHeight + m_parent.m_margins.t + m_parent.m_margins.b, Vector2.zero);
		Color color = new Color(0.4f, 0.3f, 0.4f);
		if (m_highlight)
		{
			color = new Color(0.5f, 0.3f, 0.3f);
		}
		Camera camera = CameraS.m_uiCamera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		uint num = DebugDraw.ColorToUInt(color);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, "UIComponent: Prefab");
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, camera, true);
	}
}
