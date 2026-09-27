using UnityEngine;

public class UIListSelectorButton : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.07f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected UIVerticalList m_thisPage;

	protected UIPagedCanvas m_scrollingCanvas;

	protected UIPropertyLabel m_label;

	protected UIIconLabel m_iconLabel;

	protected int m_valueIndex;

	protected bool m_changeValue;

	public UIListSelectorButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, UIModel _model, string _INTfieldName, string _label, SpriteSheet _spriteSheet, Frame _valueFrame, int _valueIndex)
		: base(_parent, true, _tag, null, _model, _INTfieldName)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_thisPage = _thisPage;
		m_scrollingCanvas = _scrollingCanvas;
		m_valueIndex = _valueIndex;
		m_iconLabel = new UIIconLabel(this, "Icon", _spriteSheet, _valueFrame, Align.Left, Align.Center);
		m_label = new UIPropertyLabel(this, "Label", _label);
		m_label.m_margins.l = 0.09f;
	}

	public UIListSelectorButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, UIModel _model, string _INTfieldName, string _label, string _valueLabel, int _valueIndex)
		: base(_parent, true, _tag, null, _model, _INTfieldName)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_thisPage = _thisPage;
		m_scrollingCanvas = _scrollingCanvas;
		m_valueIndex = _valueIndex;
		m_label = new UIPropertyLabel(this, _tag, _label);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		m_changeValue = true;
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		m_changeValue = false;
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_changeValue && _inside)
		{
			SetValue(m_valueIndex);
			m_scrollingCanvas.PreviousPage();
			m_parent.Destroy();
		}
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
