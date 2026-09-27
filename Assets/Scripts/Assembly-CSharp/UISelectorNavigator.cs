public class UISelectorNavigator : UIHorizontalList
{
	public UITextButton m_cancelButton;

	public UITextButton[] m_parentCategoryButtons;

	public UITextButton m_currentCategoryButton;

	private string[] m_parentCategories;

	public UISelectorNavigator(UIComponent _parent, string _tag, string[] _parentCategories, string _currentCategory)
		: base(_parent, _tag)
	{
		m_parentCategories = _parentCategories;
		SetMargins(0.05f, 0.05f, 0.03f, 0f);
		SetSpacing(0.03f, RelativeTo.ScreenShortest);
		SetAlign(0f, 1f);
		RemoveDrawHandler();
		UIShrinkToContentCanvas uIShrinkToContentCanvas = new UIShrinkToContentCanvas(this, string.Empty);
		uIShrinkToContentCanvas.SetMargins(0f, 0f, 0f, 0.02f);
		uIShrinkToContentCanvas.RemoveDrawHandler();
		m_cancelButton = new UITextButton(uIShrinkToContentCanvas, "CancelButton", "Cancel", "Fonts/HurmeBold", 0.025f);
		m_cancelButton.SetMargins(0.025f, 0.025f, 0.015f, 0.015f);
		m_cancelButton.SetDrawHandler(UIDrawHandlers.SelectorCancelButton);
		m_parentCategoryButtons = new UITextButton[_parentCategories.Length];
		for (int i = 0; i < _parentCategories.Length; i++)
		{
			UIShrinkToContentCanvas uIShrinkToContentCanvas2 = new UIShrinkToContentCanvas(this, string.Empty);
			uIShrinkToContentCanvas2.SetMargins(0f, 0f, 0f, 0.02f);
			uIShrinkToContentCanvas2.RemoveDrawHandler();
			m_parentCategoryButtons[i] = new UITextButton(uIShrinkToContentCanvas2, _parentCategories[i], _parentCategories[i], "Fonts/HurmeBold", 0.025f);
			m_parentCategoryButtons[i].SetMargins(0.025f, 0.025f, 0.015f, 0.015f);
			m_parentCategoryButtons[i].SetDrawHandler(UIDrawHandlers.SelectorCategoryButton);
		}
		m_currentCategoryButton = new UITextButton(this, "SelectedCategory", _currentCategory, "Fonts/HurmeBold", 0.025f);
		m_currentCategoryButton.SetMargins(0.025f, 0.025f, 0.015f, 0.015f);
		m_currentCategoryButton.SetVerticalAlign(0f);
		m_currentCategoryButton.SetDrawHandler(UIDrawHandlers.SelectedCategoryButton);
	}

	public override void Step()
	{
		if (m_currentCategoryButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
		else if (m_cancelButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
		base.Step();
	}
}
