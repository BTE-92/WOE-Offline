using UnityEngine;

public class UIFloatPropertyButton : UIVerticalListButton
{
	public float m_minValue;

	public float m_maxValue;

	protected bool m_changeValue;

	public UIPropertyLabel m_label;

	public UIPropertyValueLabel m_value;

	public UIFloatPropertyButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, string _label, UIModel _model, string _FLOATfieldName, float _minValue, float _maxValue)
		: base(_parent, _thisPage, _scrollingCanvas, _tag, _model, _FLOATfieldName)
	{
		m_minValue = _minValue;
		m_maxValue = _maxValue;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_label = new UIPropertyLabel(this, _tag, _label);
		string text = ((float)GetValue()).ToString("0.0");
		m_value = new UIPropertyValueLabel(this, _tag, text);
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		Vector2 vector = _touch.m_currentPosition - _touch.m_startPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			FreezeVerticalScroll(false);
			m_changeValue = true;
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValue)
		{
			float x = _touch.m_deltaPosition.x;
			float num = (float)GetValue();
			float num2 = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, num + x / (Mathf.Max(m_actualWidth, (m_maxValue - m_minValue) * 10f) / (m_maxValue - m_minValue))));
			SetValue(float.Parse(num2.ToString("0.0")));
			m_value.m_text = num2.ToString("0.0");
			m_value.d_Draw(m_value);
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
	}

	public override void OnValueChange(object _value)
	{
		float num = float.Parse(_value.ToString());
		m_value.m_text = num.ToString("0.0");
		if (m_value.d_Draw != null)
		{
			m_value.d_Draw(m_value);
		}
	}
}
