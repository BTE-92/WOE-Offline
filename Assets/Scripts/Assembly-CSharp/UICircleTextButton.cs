using UnityEngine;

public class UICircleTextButton : UICircleButton
{
	public UIFittedText m_uiFittedText;

	public UICircleTextButton(UIComponent _parent, string _tag, string _text, string _fontResourcePath, Camera _camera)
		: base(_parent, _tag, _camera)
	{
		m_uiFittedText = new UIFittedText(this, false, _tag, _text, _fontResourcePath, true);
		m_uiFittedText.RemoveDrawHandler();
	}
}
