public class EditorTestEndState : BasicState
{
	public UIHorizontalList m_area;

	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_editButton;

	public override void Enter(IStatedObject _parent)
	{
		m_area = new UIHorizontalList(null, string.Empty);
		m_area.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		m_area.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(m_area, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"), false);
		m_restartButton.SetHeight(0.14f, RelativeTo.ScreenShortest);
		m_restartButton.SetVerticalAlign(1f);
		m_editButton = new UIRectSpriteButton(m_area, "Edit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_edit"), false);
		m_editButton.SetHeight(0.14f, RelativeTo.ScreenShortest);
		m_editButton.SetVerticalAlign(1f);
		m_area.Update();
	}

	public override void Execute()
	{
		if (m_restartButton.m_hit)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorTestState());
			EditorScene.ResetGame();
			Player.OpenController();
			Player.SetAsAudioListener();
		}
		else if (m_editButton.m_hit)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
			Player.Remove();
			PsState.m_gameState = GameState.Edit;
			(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
			LevelManager.ResetCurrentLevel();
		}
	}

	public override void Exit()
	{
		m_area.Destroy();
	}
}
