public class UIShrinkToContentCanvas : UICanvas
{
	public UIShrinkToContentCanvas(UIComponent _parent, string _tag)
		: base(_parent, _tag, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
	}

	public override void Update()
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateChildren();
		ArrangeContents();
		float num = m_contentWidth + m_actualMargins.l + m_actualMargins.r;
		float widthRatio = num / m_tempReferenceWidth;
		SetWidth(widthRatio, RelativeTo.ParentWidth);
		float num2 = m_contentHeight + m_actualMargins.t + m_actualMargins.b;
		float heightRatio = num2 / m_tempReferenceHeight;
		SetHeight(heightRatio, RelativeTo.ParentHeight);
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		UpdateChildrenAlign();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
	}
}
