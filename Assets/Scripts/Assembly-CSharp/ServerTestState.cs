public class ServerTestState : BasicState
{
	public override void Enter(IStatedObject _parent)
	{
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "Server Test");
		save();
		loadAll();
	}

	private void save()
	{
	}

	private void loadAll()
	{
	}

	private void Handler(TLTouch _t, bool _secondary)
	{
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
