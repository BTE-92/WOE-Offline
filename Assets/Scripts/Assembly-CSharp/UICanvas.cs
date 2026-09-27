public class UICanvas : UIComponent
{
	public float m_contentX;

	public float m_contentY;

	public float m_contentWidth;

	public float m_contentHeight;

	public float m_contentCenterX;

	public float m_contentCenterY;

	public UICanvas(UIComponent _parent, string _tag, UIModel _model = null, string _fieldName = "", bool _touchable = true)
		: base(_parent, _touchable, _tag, null, _model, _fieldName)
	{
	}

	public override void ArrangeContents()
	{
		float num = 99999f;
		float num2 = -99999f;
		float num3 = 99999f;
		float num4 = -99999f;
		for (int i = 0; i < m_childs.Count; i++)
		{
			UIComponent uIComponent = m_childs[i];
			float num5 = uIComponent.m_TC.transform.localPosition.x - uIComponent.m_actualWidth * 0.5f;
			float num6 = uIComponent.m_TC.transform.localPosition.x + uIComponent.m_actualWidth * 0.5f;
			float num7 = uIComponent.m_TC.transform.localPosition.y - uIComponent.m_actualHeight * 0.5f;
			float num8 = uIComponent.m_TC.transform.localPosition.y + uIComponent.m_actualHeight * 0.5f;
			if (num6 > num2)
			{
				num2 = num6;
			}
			if (num5 < num)
			{
				num = num5;
			}
			if (num8 > num4)
			{
				num4 = num8;
			}
			if (num7 < num3)
			{
				num3 = num7;
			}
		}
		m_contentWidth = num2 - num;
		m_contentHeight = num4 - num3;
		m_contentCenterX = num + m_contentWidth * 0.5f;
		m_contentCenterY = num3 + m_contentHeight * 0.5f;
		m_contentX = m_actualWidth * -0.5f + m_actualMargins.l + m_contentWidth + m_actualMargins.r;
		m_contentY = m_actualHeight * 0.5f - m_actualMargins.t - m_contentHeight - m_actualMargins.b;
		base.ArrangeContents();
	}

	public override void DrawHandler(UIComponent _c)
	{
		base.DrawHandler(_c);
	}
}
