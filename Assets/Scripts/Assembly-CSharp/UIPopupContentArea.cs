public class UIPopupContentArea : UIVerticalList
{
	public UIPopupContentArea(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetMargins(0.075f, 0.075f, 0.05f, 0.05f);
		SetSpacing(0.05f, RelativeTo.ScreenHeight);
		SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
	}
}
