using UnityEngine;

public class UITextButton : UITextbox
{
	public UITextButton(UIComponent _parent, string _tag, string _text, string _fontResource = "Fonts/HurmeRegular", float _fontSize = 0.04f, RelativeTo _fontSizeRelativeTo = RelativeTo.ScreenHeight, bool _adjustWidthToTextWidth = true)
		: base(_parent, true, _tag, _text, _fontResource, _fontSize, _fontSizeRelativeTo, _adjustWidthToTextWidth, Align.Center, Align.Middle)
	{
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		float textboxWidth = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
		float textboxHeight = m_actualHeight - m_actualMargins.t - m_actualMargins.b;
		if (m_tmc == null)
		{
			m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, m_fontResourcePath, textboxWidth, textboxHeight, m_fontSize, m_textHorizontalAlign, m_textVerticalAlign, m_camera, "Text");
		}
		int fontSize = Mathf.FloorToInt(m_fontSize * m_fontSizeRelative);
		m_tmc.m_textMesh.fontSize = fontSize;
		Vector2 textSize = TextMeshS.GetTextSize(m_tmc, m_text);
		if (m_adjustWidthToTextWidth)
		{
			SetWidth((textSize.x + m_actualMargins.l + m_actualMargins.r) / (float)Screen.width, RelativeTo.ScreenWidth);
		}
		SetHeight((textSize.y + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
		CalculateReferenceSizes();
		UpdateSize();
		TextMeshS.SetTextToTextbox(m_tmc, m_actualWidth - m_actualMargins.l - m_actualMargins.r, m_actualHeight - m_actualMargins.t - m_actualMargins.b, m_text);
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		UpdateAlign();
		UpdateMargins();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}
}
