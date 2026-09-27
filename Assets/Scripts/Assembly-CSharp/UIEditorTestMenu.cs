public class UIEditorTestMenu : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_editButton;

	public UIRaceTimer m_timer;

	public UIEditorTestMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 1f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.03f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(uIHorizontalList, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"), false);
		m_restartButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
		m_restartButton.SetVerticalAlign(1f);
		m_editButton = new UIRectSpriteButton(uIHorizontalList, "Edit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_edit"), false);
		m_editButton.SetHeight(0.11f, RelativeTo.ScreenShortest);
		m_editButton.SetVerticalAlign(1f);
		m_timer = new UIRaceTimer(this, "Timer");
		m_timer.SetHorizontalAlign(1f);
		Update();
	}

	public override void Step()
	{
		if (m_editButton.m_hit)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
			Player.Remove();
			PsState.m_gameState = GameState.Edit;
			(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
			LevelManager.ResetCurrentLevel();
		}
		else if (m_restartButton.m_hit)
		{
			EditorScene.ResetGame();
			Player.OpenController();
			Player.SetAsAudioListener();
		}
		base.Step();
	}
}
