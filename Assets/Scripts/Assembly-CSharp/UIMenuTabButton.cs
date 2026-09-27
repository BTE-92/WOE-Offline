public class UIMenuTabButton : UIFittedText
{
	public UIMenuTabButton(UIComponent _parent, string _tag, string _text)
		: base(_parent, true, _tag, _text, "Fonts/HurmeRegular", true)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f / 6f, RelativeTo.ScreenHeight);
	}
}
