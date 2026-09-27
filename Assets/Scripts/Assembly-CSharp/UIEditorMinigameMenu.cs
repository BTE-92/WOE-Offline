public class UIEditorMinigameMenu : UICanvas
{
	public UIRectSpriteButton m_settingsButton;

	public UIEditorMinigameMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(0.14f, RelativeTo.ParentHeight);
		SetVerticalAlign(1f);
		RemoveTouchAreas();
		RemoveDrawHandler();
		float num = 0.7f;
		UIShrinkToContentCanvas uIShrinkToContentCanvas = new UIShrinkToContentCanvas(this, string.Empty);
		uIShrinkToContentCanvas.SetHorizontalAlign(0.02f);
		uIShrinkToContentCanvas.SetVerticalAlign(1f);
		uIShrinkToContentCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIShrinkToContentCanvas, string.Empty);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ParentHeight);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		m_settingsButton = new UIRectSpriteButton(uIHorizontalList, "SettingsButton", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_key_5"), false);
		m_settingsButton.SetHeight(0.65f, RelativeTo.ParentHeight);
		m_settingsButton.SetVerticalAlign(1f);
		m_settingsButton.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_settingsButton, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_settings"), false);
		uIFittedSprite.SetWidth(1f, RelativeTo.ParentWidth);
		uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		uIFittedSprite.SetMargins(0.25f * num, 0.2f * num, 0.2f, 0.2f, RelativeTo.ParentHeight);
		uIFittedSprite.SetDepthOffset(-10f);
		uIFittedSprite.SetVerticalAlign(1f);
		Update();
	}

	public override void Step()
	{
		if (m_settingsButton.m_hit)
		{
			PsState.m_editorIsLefty = !PsState.m_editorIsLefty;
			EditorBaseState editorBaseState = Main.m_currentGame.m_currentScene.m_stateMachine.GetCurrentState() as EditorBaseState;
			editorBaseState.m_editorMenu.CloseFileMenu();
			editorBaseState.ApplyLeftySettings();
			EditorBaseState.RemoveTransformGizmo();
		}
		base.Step();
	}
}
