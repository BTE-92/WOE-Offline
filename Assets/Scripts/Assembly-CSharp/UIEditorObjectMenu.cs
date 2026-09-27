public class UIEditorObjectMenu : UIShrinkToContentCanvas
{
	public UIRectSpriteButton m_objectMenuButton;

	public UIEditorObjectMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 0f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		m_objectMenuButton = new UIRectSpriteButton(this, "Add Object", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_object_add"), false);
		m_objectMenuButton.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		Update();
	}

	public override void Step()
	{
		if (m_objectMenuButton.m_hit)
		{
			EditorBaseState.RemoveTransformGizmo();
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorItem());
		}
		base.Step();
	}
}
