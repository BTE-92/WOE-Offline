using UnityEngine;

public class UINotificationBanner : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.07f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected NotificationData m_notificationData;

	public UILabel m_label;

	public UINotificationBanner(UIVerticalList _parent, string _tag, NotificationData _notificationData)
		: base(_parent, true, _tag, null, null, null)
	{
		m_notificationData = _notificationData;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_label = new UILabel(this, _tag, m_notificationData.message, Align.Left, Align.Center);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (_inside && !_touch.m_primaryArea.m_wasDragged)
		{
			string[] propertyKeys = new string[1] { "friendData" };
			object[] propertyValues = new object[1] { m_notificationData };
			EventS.Dispatch("NOTIFICATION_BANNER_EVENT", propertyKeys, propertyValues, false);
		}
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		Highlight(false);
		HighlightSecondary(false);
		m_end = true;
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
