public class EditorPublishPopupState : BasicState
{
	public UIModel m_model;

	public string m_levelName;

	private UICanvas m_publishPopupContainer;

	private UIPublishPopupFooter m_footer;

	public override void Enter(IStatedObject _parent)
	{
		m_levelName = LevelManager.m_currentLevel.m_name;
		m_model = new UIModel(this);
		m_publishPopupContainer = new UICanvas(null, "PublishPopupContainer", null, string.Empty);
		m_publishPopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_publishPopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_publishPopupContainer, "PublishPopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "PublishPopupHeader", "publish", "Enter a name and press Publish");
		UIPopupContentArea parent = new UIPopupContentArea(uIVerticalList, "PublishPopupContentArea");
		new UITextField(parent, "PublishPopupNameField", "Minigame Name", string.Empty, null, m_model, "m_levelName");
		m_footer = new UIPublishPopupFooter(uIVerticalList, "PublishPopupFooter");
		m_publishPopupContainer.Update();
	}

	public override void Execute()
	{
		if (m_footer.m_publishButton.m_hit)
		{
			LevelManager.m_currentLevel.m_name = m_levelName;
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishState());
		}
		else if (m_footer.m_cancelButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	public override void Exit()
	{
		if (m_publishPopupContainer != null)
		{
			m_publishPopupContainer.Destroy();
			m_publishPopupContainer = null;
		}
	}
}
