public class UIPublishPopupFooter : UIShrinkToContentCanvas
{
	public UITextButton m_cancelButton;

	public UITextButton m_publishButton;

	public UIPublishPopupFooter(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetMargins(0.05f, 0.05f, 0.05f, 0f);
		RemoveDrawHandler();
		m_cancelButton = new UITextButton(this, "SavePanelCancelButton", "Cancel", "Fonts/HurmeBold", 0.0375f);
		m_cancelButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_cancelButton.SetHorizontalAlign(0f);
		m_cancelButton.SetDrawHandler(UIDrawHandlers.NegativeButton);
		m_publishButton = new UITextButton(this, "SavePanelSaveButton", "Publish", "Fonts/HurmeBold", 0.0375f);
		m_publishButton.SetMargins(0.025f, 0.025f, 0.02f, 0.015f);
		m_publishButton.SetHorizontalAlign(1f);
		m_publishButton.SetDrawHandler(UIDrawHandlers.PositiveButton);
	}
}
