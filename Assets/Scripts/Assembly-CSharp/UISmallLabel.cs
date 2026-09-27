using System;
using UnityEngine;

public class UISmallLabel : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.05f;

	public static cpBB m_defaultMargins = new cpBB(0.005f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public string m_text;

	public TextC m_TXC;

	public Align m_horizontalLabelAlign;

	public Align m_verticalLabelAlign;

	public UISmallLabel(UIComponent _parent, string _tag, string _text, Align _horizontalAlign, Align _verticalAlign)
		: base(_parent, false, _tag, null, null, string.Empty)
	{
		m_horizontalLabelAlign = _horizontalAlign;
		m_verticalLabelAlign = _verticalAlign;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		m_text = _text;
		m_TXC = TextS.AddSingleLineComponent(m_TC, m_text);
		SetDrawHandler(DrawHandler);
	}

	protected void ArrangeText()
	{
		float val = m_actualHeight / m_TXC.textHeight * 0.382f;
		float val2 = m_actualWidth / m_TXC.textWidth * 0.382f;
		float num = Math.Min(val, val2);
		TransformS.SetScale(m_TXC.contentTC, num);
		float x = 0f;
		if (m_horizontalLabelAlign == Align.Left)
		{
			x = (m_actualWidth - m_actualMargins.l - m_actualMargins.r - m_TXC.textWidth * num) * -0.5f + (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		}
		else if (m_horizontalLabelAlign == Align.Right)
		{
			x = (m_actualWidth - m_actualMargins.l - m_actualMargins.r - m_TXC.textWidth * num) * 0.5f + (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		}
		float num2 = 0f;
		if (m_verticalLabelAlign == Align.Bottom)
		{
			num2 = (m_actualHeight - m_actualMargins.b - m_actualMargins.t - m_TXC.textHeight * num) * -0.5f + (m_actualMargins.b - m_actualMargins.t) * 0.5f;
		}
		else if (m_verticalLabelAlign == Align.Top)
		{
			num2 = (m_actualHeight - m_actualMargins.b - m_actualMargins.t - m_TXC.textHeight * num) * 0.5f + (m_actualMargins.b - m_actualMargins.t) * 0.5f;
		}
		num2 += m_TXC.textHeight * num * 0.5f;
		TransformS.SetPosition(_position: new Vector3(x, num2, 0f), _c: m_TXC.contentTC);
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		TextS.ChangeText(m_TXC, m_text);
		ArrangeText();
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, m_camera, true);
	}
}
