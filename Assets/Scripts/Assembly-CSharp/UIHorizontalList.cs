using UnityEngine;

public class UIHorizontalList : UICanvas
{
	public float m_spacing;

	public float m_actualSpacing;

	public RelativeTo m_spacingRelativeTo;

	public UIHorizontalList(UIComponent _parent, string _tag)
		: base(_parent, _tag, null, string.Empty, false)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
	}

	public virtual void SetSpacing(float _value, RelativeTo _relativeTo)
	{
		m_spacing = _value;
		m_spacingRelativeTo = _relativeTo;
	}

	protected virtual void UpdateSpacing()
	{
		RelativeTo relativeTo = m_spacingRelativeTo;
		if (m_spacingRelativeTo == RelativeTo.ParentShortest)
		{
			relativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_spacingRelativeTo == RelativeTo.ParentLongest)
		{
			relativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_spacingRelativeTo == RelativeTo.ScreenShortest)
		{
			relativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		else if (m_spacingRelativeTo == RelativeTo.ScreenLongest)
		{
			relativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		float num = 0f;
		switch (relativeTo)
		{
		case RelativeTo.ParentWidth:
			num = ((m_parent == null) ? ((float)Screen.width) : m_parent.m_actualWidth);
			break;
		case RelativeTo.ParentHeight:
			num = ((m_parent == null) ? ((float)Screen.height) : m_parent.m_actualHeight);
			break;
		case RelativeTo.ScreenHeight:
			num = Screen.height;
			break;
		case RelativeTo.ScreenWidth:
			num = Screen.width;
			break;
		case RelativeTo.OwnHeight:
			num = m_actualHeight;
			break;
		case RelativeTo.OwnWidth:
			num = m_actualWidth;
			break;
		}
		m_actualSpacing = m_spacing * num;
	}

	public override void ArrangeContents()
	{
		float num = 99999f;
		float num2 = -99999f;
		m_contentX = m_actualWidth * -0.5f + m_actualMargins.l;
		float num3 = 0f;
		for (int i = 0; i < m_childs.Count; i++)
		{
			UIComponent uIComponent = m_childs[i];
			if (i > 0)
			{
				m_contentX += m_actualSpacing;
			}
			Vector3 localPosition = uIComponent.m_TC.transform.localPosition;
			localPosition.x = m_contentX + uIComponent.m_actualWidth * 0.5f;
			TransformS.SetPosition(uIComponent.m_TC, localPosition);
			uIComponent.UpdateVerticalAlign();
			m_contentX += uIComponent.m_actualWidth;
			if (uIComponent.m_actualHeight > num3)
			{
				num3 = uIComponent.m_actualHeight;
			}
			float num4 = uIComponent.m_TC.transform.localPosition.x - uIComponent.m_actualWidth * 0.5f;
			float num5 = uIComponent.m_TC.transform.localPosition.x + uIComponent.m_actualWidth * 0.5f;
			if (num5 > num2)
			{
				num2 = num5;
			}
			if (num4 < num)
			{
				num = num4;
			}
		}
		m_contentWidth = num2 - num;
		m_contentHeight = num3;
		m_contentCenterX = num + m_contentWidth * 0.5f;
		m_contentCenterY = 0f;
		m_contentY = m_actualHeight * 0.5f - m_actualMargins.t - m_contentHeight - m_actualMargins.b;
		m_contentX += m_actualMargins.l;
		for (int j = 0; j < m_childs.Count; j++)
		{
			if (m_childs[j].m_uniqueCamera)
			{
				m_childs[j].UpdateUniqueCamera();
			}
		}
	}

	public override void Update()
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateSpacing();
		UpdateChildren();
		ArrangeContents();
		float num = m_contentWidth + m_actualMargins.l + m_actualMargins.r;
		float widthRatio = num / m_tempReferenceWidth;
		SetWidth(widthRatio, RelativeTo.ParentWidth);
		float num2 = m_contentHeight + m_actualMargins.t + m_actualMargins.b;
		float heightRatio = num2 / m_tempReferenceHeight;
		SetHeight(heightRatio, RelativeTo.ParentHeight);
		UpdateSize();
		UpdateAlign();
		ArrangeContents();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
	}
}
