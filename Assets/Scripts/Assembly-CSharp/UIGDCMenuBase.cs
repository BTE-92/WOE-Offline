using UnityEngine;

public class UIGDCMenuBase : UIPagedCanvas
{
	public UICanvas m_container;

	public UICanvas m_right;

	public float m_spacing;

	public float m_actualSpacing;

	public RelativeTo m_spacingRelativeTo;

	public UIGDCMenuBase(UIComponent _parent, string _tag)
		: base(null, _tag)
	{
		RemoveDrawHandler();
		m_container = new UICanvas(_parent, _tag + "Container", null, string.Empty);
		m_container.RemoveTouchAreas();
		m_container.RemoveDrawHandler();
		m_container.SetWidth(1f, RelativeTo.ParentWidth);
		m_container.SetHeight(1f, RelativeTo.ParentHeight);
		SetContainerSpacing(0.02f, RelativeTo.ParentHeight);
		m_right = new UICanvas(m_container, _tag + "Right", null, string.Empty);
		m_right.DisableTouchAreas();
		m_right.RemoveDrawHandler();
		m_right.SetHorizontalAlign(1f);
		m_right.SetWidth(0.3f, RelativeTo.ParentHeight);
		m_container.Update();
		base.Parent(m_container);
		base.SetWidth(1f, RelativeTo.ParentWidth);
		base.SetHeight(1f, RelativeTo.ParentHeight);
	}

	public virtual void SetContainerSpacing(float _value, RelativeTo _relativeTo)
	{
		m_spacing = _value;
		m_spacingRelativeTo = _relativeTo;
	}

	protected virtual void UpdateContainerSpacing()
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
			num = m_parent.m_actualWidth;
			break;
		case RelativeTo.ParentHeight:
			num = m_parent.m_actualHeight;
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

	public override void Update()
	{
		UpdateContainerSpacing();
		float num = m_container.m_actualWidth - m_container.m_actualMargins.l - m_container.m_actualMargins.r;
		float num2 = m_right.m_actualWidth + m_actualSpacing;
		float actualWidth = m_right.m_actualWidth;
		base.SetWidth(1f - num2 / num, RelativeTo.ParentWidth);
		base.SetHorizontalAlign(0.5f - actualWidth * 0.5f / num2);
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
