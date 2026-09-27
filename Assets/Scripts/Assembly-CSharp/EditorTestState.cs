public class EditorTestState : BasicState
{
	public UIEditorTestMenu m_testMenu;

	public EditorTestState()
	{
		PsState.m_gameState = GameState.Test;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_testMenu = new UIEditorTestMenu(null, "TestMenu");
		m_testMenu.SetAlign(1f, 1f);
	}

	public override void Execute()
	{
		if (PsState.m_gameStarted && !PsState.m_gameEnded)
		{
			PsState.m_gameTicks++;
			m_testMenu.m_timer.SetTimeFromTicks(PsState.m_gameTicks);
		}
	}

	public override void Exit()
	{
		EntityManager.RemoveEntitiesByTag("GTAG_INGAME_PARTICLES");
		Player.CloseController();
		m_testMenu.Destroy();
	}
}
