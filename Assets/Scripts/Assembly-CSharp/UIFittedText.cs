using UnityEngine;

public class UIFittedText : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ParentHeight;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public TextMeshC m_tmc;

	public string m_text;

	public bool m_fitTextToWidth;

	public UIFittedText(UIComponent _parent, bool _touchable, string _tag, string _text, string _fontResourcePath, bool _fitTextToWidth, Align _horizontalAlign = Align.Center, Align _verticalAlign = Align.Middle)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		m_fitTextToWidth = _fitTextToWidth;
		m_text = _text;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, _fontResourcePath, 0f, 0f, 20f, _horizontalAlign, _verticalAlign, m_camera, _tag + "Text");
	}

	public void SetText(string _text)
	{
		m_text = _text;
		TextMeshS.SetText(m_tmc, _text);
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		TextMeshS.FitTextToTextbox(m_tmc, m_actualWidth, m_actualHeight, m_text, m_fitTextToWidth);
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}
}
