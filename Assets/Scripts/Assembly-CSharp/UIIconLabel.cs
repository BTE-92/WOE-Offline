using UnityEngine;

public class UIIconLabel : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 1f;

	public static cpBB m_defaultMargins = new cpBB(0.02f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ParentHeight;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public SpriteSheet m_spriteSheet;

	public Frame m_frame;

	public SpriteC m_sprite;

	public Align m_horizontalLabelAlign;

	public Align m_verticalLabelAlign;

	public UIIconLabel(UIComponent _parent, string _tag, SpriteSheet _spriteSheet, Frame _frame, Align _horizontalAlign, Align _verticalAlign)
		: base(_parent, false, _tag, null, null, string.Empty)
	{
		m_horizontalLabelAlign = _horizontalAlign;
		m_verticalLabelAlign = _verticalAlign;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		m_spriteSheet = _spriteSheet;
		m_frame = _frame;
		TransformC transformC = TransformS.AddComponent(m_TC.p_entity, "IconLabel");
		TransformS.ParentComponent(transformC, m_TC);
		transformC.transform.gameObject.layer = m_TC.transform.gameObject.layer;
		m_sprite = SpriteS.AddComponent(transformC, m_frame, m_spriteSheet);
		SetDrawHandler(DrawHandler);
	}

	protected void ArrangeSprite()
	{
		float num = m_actualHeight / m_sprite.height * 0.618f;
		SpriteS.SetDimensionScale(m_sprite, num);
		float x = 0f;
		if (m_horizontalLabelAlign == Align.Left)
		{
			x = (m_actualWidth - m_actualMargins.l - m_actualMargins.r - m_sprite.width * num) * -0.5f + (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		}
		else if (m_horizontalLabelAlign == Align.Right)
		{
			x = (m_actualWidth - m_actualMargins.l - m_actualMargins.r - m_sprite.width * num) * 0.5f + (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		}
		float y = 0f;
		if (m_verticalLabelAlign == Align.Bottom)
		{
			y = (m_actualHeight - m_actualMargins.b - m_actualMargins.t - m_sprite.height * num) * -0.5f + (m_actualMargins.b - m_actualMargins.t) * 0.5f;
		}
		else if (m_verticalLabelAlign == Align.Top)
		{
			y = (m_actualHeight - m_actualMargins.b - m_actualMargins.t - m_sprite.height * num) * 0.5f + (m_actualMargins.b - m_actualMargins.t) * 0.5f;
		}
		TransformS.SetPosition(_position: new Vector3(x, y, 0f), _c: m_sprite.p_TC);
	}

	public override void DrawHandler(UIComponent _c)
	{
		ArrangeSprite();
	}
}
