public class UISavePopupFooter : UIShrinkToContentCanvas
{
	public UITextButton m_cancelButton;

	public UITextButton m_saveButton;

	public UITextButton m_publishButton;

	public UISavePopupFooter(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetMargins(0.05f, 0.05f, 0.05f, 0f);
		RemoveDrawHandler();
		m_cancelButton = new UITextButton(this, "SavePanelCancelButton", "Cancel", "Fonts/HurmeBold", 0.0375f);
		m_cancelButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_cancelButton.SetHorizontalAlign(0f);
		m_cancelButton.SetDrawHandler(UIDrawHandlers.NegativeButton);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, _tag);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_saveButton = new UITextButton(uIHorizontalList, "SavePanelSaveButton", "Save", "Fonts/HurmeBold", 0.0375f);
		m_saveButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_saveButton.SetHorizontalAlign(1f);
		m_saveButton.SetDrawHandler(UIDrawHandlers.PositiveButton);
		m_publishButton = new UITextButton(uIHorizontalList, "SavePanelPublishButton", "<color=#000000>Publish</color>", "Fonts/HurmeBold", 0.0375f);
		m_publishButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_publishButton.SetHorizontalAlign(1f);
		m_publishButton.SetDrawHandler(UIDrawHandlers.NeutralButton);
	}
}
