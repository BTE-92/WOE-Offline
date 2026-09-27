using UnityEngine;

public class UIVerticalListButton : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.07f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected UIVerticalList m_thisPage;

	protected UIPagedCanvas m_pagedCanvas;

	public UIVerticalListButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, UIModel _model, string _fieldName)
		: base(_parent, true, _tag, null, _model, _fieldName)
	{
		m_thisPage = _thisPage;
		m_pagedCanvas = _scrollingCanvas;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth, m_actualHeight * 0.95f, Vector2.zero);
		Color color = new Color(0.4f, 0.4f, 0.4f);
		if (m_highlight)
		{
			color = new Color(0.3f, 0.3f, 0.3f);
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
