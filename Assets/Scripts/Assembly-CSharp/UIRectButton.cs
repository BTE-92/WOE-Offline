using UnityEngine;

public class UIRectButton : UIComponent
{
	public static float m_defaultWidth = 0.2f;

	public static float m_defaultHeight = 0.09f;

	public static cpBB m_defaultMargins = new cpBB(0.01f, 0.01f, 0.01f, 0.01f);

	public UIRectButton(UIComponent _parent, string _tag, Camera _camera)
		: base(_parent, true, _tag, _camera, null, string.Empty)
	{
		SetWidth(m_defaultWidth, RelativeTo.ScreenWidth);
		SetHeight(m_defaultHeight, RelativeTo.ScreenHeight);
		SetMargins(m_defaultMargins);
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth, m_actualHeight, Vector2.zero, false);
		Color color = new Color(0.4f, 0.4f, 0.4f);
		uint num = DebugDraw.ColorToUInt(color);
		if (m_highlight)
		{
			color = new Color(0.3f, 1f, 0.3f);
		}
		Camera camera = CameraS.m_uiCamera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, camera, true);
	}
}
