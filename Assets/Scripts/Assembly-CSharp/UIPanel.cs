public class UIPanel : UIPagedCanvas
{
	public UICanvas m_container;

	public UIComponent m_header;

	public UIComponent m_footer;

	public UIPanel(UIComponent _parent, string _tag, UIComponent _header, UIComponent _footer)
		: base(null, _tag)
	{
		m_maxScrollInertialX = 50f;
		m_maxScrollInertialY = 0f;
		m_container = new UICanvas(_parent, _tag + "Container", null, string.Empty);
		m_container.SetWidth(1f, RelativeTo.ParentWidth);
		if (_header != null)
		{
			m_header = _header;
			m_header.Parent(m_container);
			m_header.SetWidth(1f, RelativeTo.ParentWidth);
			m_header.SetVerticalAlign(1f);
		}
		if (_footer != null)
		{
			m_footer = _footer;
			m_footer.Parent(m_container);
			m_footer.SetWidth(1f, RelativeTo.ParentWidth);
			m_footer.SetVerticalAlign(0f);
		}
		base.Parent(m_container);
		base.SetWidth(1f, RelativeTo.ParentWidth);
		base.SetHeight(1f, RelativeTo.ParentHeight);
	}

	public override void Update()
	{
		float num = m_container.m_actualHeight - m_container.m_actualMargins.t - m_container.m_actualMargins.b;
		float num2 = 0f;
		float num3 = 0f;
		if (m_header != null)
		{
			num2 = m_header.m_actualHeight;
		}
		if (m_footer != null)
		{
			num3 = m_footer.m_actualHeight;
		}
		float num4 = num2 + num3;
		float num5 = num2 - num3;
		base.SetHeight(1f - num4 / num, RelativeTo.ParentHeight);
		base.SetVerticalAlign(0.5f - num5 * 0.5f / num4);
		base.Update();
	}

	public new virtual void Parent(UIComponent _parent)
	{
		m_container.Parent(_parent);
	}

	public new virtual void DetachFromParent()
	{
		m_container.DetachFromParent();
	}

	public new virtual void DetachFromParent(bool _addToManager)
	{
		m_container.DetachFromParent(_addToManager);
	}

	public new virtual void Destroy()
	{
		m_container.Destroy();
	}

	public new virtual void SetSize(float _widthRatio, float _heightRatio, RelativeTo _relativeTo)
	{
		m_container.SetSize(_widthRatio, _heightRatio, _relativeTo);
	}

	public new virtual void SetWidth(float _widthRatio, RelativeTo _relativeTo)
	{
		m_container.SetWidth(_widthRatio, _relativeTo);
	}

	public new virtual void SetHeight(float _heightRatio, RelativeTo _relativeTo)
	{
		m_container.SetHeight(_heightRatio, _relativeTo);
	}

	public new virtual void SetAlign(float _horizontal, float _vertical)
	{
		m_container.SetAlign(_horizontal, _vertical);
	}

	public new virtual void SetHorizontalAlign(float _horizontal)
	{
		m_container.SetHorizontalAlign(_horizontal);
	}

	public new virtual void SetVerticalAlign(float _vertical)
	{
		m_container.SetVerticalAlign(_vertical);
	}
}
