using UnityEngine;

public class UITextField : UIVerticalList
{
	private UITextbox m_label;

	private UITextButton m_value;

	private UITextbox m_tip;

	private Keyboard m_keyboard;

	public UITextField(UIComponent _parent, string _tag, string _label, string _tip, Camera _camera, UIModel _model, string _STRINGfieldName)
		: base(_parent, _tag)
	{
		m_model = _model;
		m_fieldName = _STRINGfieldName;
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, _tag);
		uIVerticalList.SetSpacing(-0.01f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_label = new UITextbox(uIVerticalList, false, _tag, "<color=#464646>" + _label + "</color>", "Fonts/HurmeSemiBold", 0.0225f, RelativeTo.ScreenHeight, true, Align.Left, Align.Middle);
		m_label.SetMargins(0.025f, 0.025f, 0.01f, 0.01f);
		m_label.SetHorizontalAlign(0f);
		m_label.SetDrawHandler(UIDrawHandlers.Textfield);
		UIShrinkToContentCanvas uIShrinkToContentCanvas = new UIShrinkToContentCanvas(uIVerticalList, _tag);
		uIShrinkToContentCanvas.SetMargins(0.0125f, 0f, 0f, 0f);
		uIShrinkToContentCanvas.RemoveDrawHandler();
		m_value = new UITextButton(uIShrinkToContentCanvas, _tag, "<color=#464646>" + (string)GetValue() + "</color>", "Fonts/HurmeSemiBold", 0.04f, RelativeTo.ScreenHeight, false);
		m_value.SetMargins(0.0375f, 0.0375f, 0.025f, 0.025f);
		m_value.SetWidth(1f, RelativeTo.ParentWidth);
		m_value.SetHorizontalAlign(0f);
		m_value.SetDrawHandler(UIDrawHandlers.Textfield);
		TextMeshS.SetAlign(m_value.m_tmc, Align.Left, Align.Middle);
		if (_tip != string.Empty)
		{
			m_tip = new UITextbox(this, false, _tag, _tip, "Fonts/HurmeRegular", 0.02f, RelativeTo.ScreenHeight);
			m_tip.SetMargins(0.025f, 0f, 0.015f, 0f);
			m_tip.RemoveDrawHandler();
		}
	}

	public override void Step()
	{
		if (m_keyboard != null)
		{
			m_keyboard.Update();
		}
		if (m_value.m_hit)
		{
			if (m_keyboard == null)
			{
				m_keyboard = new Keyboard((string)GetValue(), 32, false, false, m_label.m_text);
				m_keyboard.keyboardPressed += delegate(string obj)
				{
					SetValue(obj);
					m_value.SetText("<color=#464646>" + obj + "</color>");
				};
				m_keyboard.keyboardClosed += delegate(string obj)
				{
					SetValue(obj);
					m_value.SetText("<color=#464646>" + obj + "</color>");
				};
			}
			else if (m_keyboard.m_done)
			{
				m_keyboard.Open();
			}
			else
			{
				m_keyboard.Close();
			}
		}
		base.Step();
	}
}
