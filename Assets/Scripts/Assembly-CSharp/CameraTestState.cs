public class CameraTestState : BasicState
{
	public override void Enter(IStatedObject _parent)
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
