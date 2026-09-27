using UnityEngine;

public class UIBoolPropertyButton : UIVerticalListButton
{
	protected string[] m_valueLabels;

	public float m_minValue;

	public float m_maxValue;

	protected float m_currentFloatValue;

	protected bool m_changeValue;

	protected bool m_changeValueByDragging;

	protected bool m_valueChangedByDragging;

	public UIPropertyLabel m_label;

	public UIPropertyValueLabel m_value;

	public UIBoolPropertyButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, string _label, UIModel _model, string _BOOLfieldName, string[] _valueLabels)
		: base(_parent, _thisPage, _scrollingCanvas, _tag, _model, _BOOLfieldName)
	{
		m_minValue = 0f;
		m_maxValue = 1f;
		m_valueLabels = _valueLabels;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_label = new UIPropertyLabel(this, _tag, _label);
		m_currentFloatValue = ((!(bool)GetValue()) ? 0.4f : 0.6f);
		int num = Mathf.RoundToInt(m_currentFloatValue);
		string text = m_valueLabels[num];
		m_value = new UIPropertyValueLabel(this, _tag, text);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		m_changeValue = true;
		m_changeValueByDragging = false;
		m_valueChangedByDragging = false;
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		Vector2 vector = _touch.m_currentPosition - _touch.m_startPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			FreezeVerticalScroll(false);
			m_changeValueByDragging = true;
			m_currentFloatValue = ((!(bool)GetValue()) ? 0.4f : 0.6f);
		}
		else
		{
			m_changeValue = false;
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValueByDragging)
		{
			bool flag = (bool)GetValue();
			float x = _touch.m_deltaPosition.x;
			float currentFloatValue = m_currentFloatValue;
			float num = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, currentFloatValue + x / (Mathf.Max(m_actualWidth, m_maxValue - m_minValue) / (m_maxValue - m_minValue))));
			m_currentFloatValue = Mathf.Max(0.4f, Mathf.Min(0.6f, num));
			int num2 = Mathf.RoundToInt(num);
			bool flag2 = num2 == 1;
			SetValue(flag2);
			m_value.m_text = m_valueLabels[num2];
			if (m_value.d_Draw != null)
			{
				m_value.d_Draw(m_value);
			}
			if (flag != flag2)
			{
				m_valueChangedByDragging = true;
			}
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_changeValue && !m_valueChangedByDragging)
		{
			bool flag = (bool)GetValue();
			flag = !flag;
			SetValue(flag);
			int num = (flag ? 1 : 0);
			m_value.m_text = m_valueLabels[num];
			if (m_value.d_Draw != null)
			{
				m_value.d_Draw(m_value);
			}
		}
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
		m_changeValueByDragging = false;
		m_valueChangedByDragging = false;
	}

	public override void OnValueChange(object _value)
	{
		int num = (bool.Parse(_value.ToString()) ? 1 : 0);
		m_value.m_text = m_valueLabels[num];
		if (m_value.d_Draw != null)
		{
			m_value.d_Draw(m_value);
		}
	}
}
