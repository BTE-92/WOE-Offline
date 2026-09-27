public class UIPropertyValueLabel : UILabel
{
	public new static float m_defaultWidth = 1f;

	public new static float m_defaultHeight = 0.04f;

	public new static cpBB m_defaultMargins = new cpBB(0.02f);

	public new static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public new static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public new static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public UIPropertyValueLabel(UIComponent _parent, string _tag, string _text)
		: base(_parent, _tag, _text, Align.Right, Align.Center)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
	}
}
