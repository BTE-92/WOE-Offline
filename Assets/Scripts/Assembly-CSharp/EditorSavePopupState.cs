public class EditorSavePopupState : BasicState
{
	public UIModel m_model;

	public string m_levelName;

	public string m_levelDescription;

	private UICanvas m_savePopupContainer;

	private UITextField m_nameField;

	private UITextArea m_descriptionField;

	private UISavePopupFooter m_footer;

	public EditorSavePopupState()
	{
		Debug.Log(LevelManager.m_currentLevel.m_description);
	}

	public override void Enter(IStatedObject _parent)
	{
		m_levelName = LevelManager.m_currentLevel.m_name;
		m_levelDescription = LevelManager.m_currentLevel.m_description;
		m_model = new UIModel(this);
		m_savePopupContainer = new UICanvas(null, "SavePopupContainer", null, string.Empty);
		m_savePopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_savePopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_savePopupContainer, "SavePopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "save", "Enter a name and press Save");
		UIPopupContentArea parent = new UIPopupContentArea(uIVerticalList, "SavePopupContentArea");
		m_nameField = new UITextField(parent, "SavePopupNameField", "Minigame Name", string.Empty, null, m_model, "m_levelName");
		m_descriptionField = new UITextArea(parent, "SavePopupDescriptionField", "Minigame Description", string.Empty, null, m_model, "m_levelDescription");
		m_footer = new UISavePopupFooter(uIVerticalList, "SavePopupFooter");
		m_savePopupContainer.Update();
	}

	public override void Execute()
	{
		if (m_footer.m_saveButton.m_hit)
		{
			LevelManager.m_currentLevel.m_name = m_levelName;
			LevelManager.m_currentLevel.m_description = m_levelDescription;
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSaveState());
		}
		else if (m_footer.m_publishButton.m_hit)
		{
			LevelManager.m_currentLevel.m_name = m_levelName;
			LevelManager.m_currentLevel.m_description = m_levelDescription;
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishState());
		}
		else if (m_footer.m_cancelButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	public override void Exit()
	{
		if (m_savePopupContainer != null)
		{
			m_savePopupContainer.Destroy();
			m_savePopupContainer = null;
		}
	}
}
