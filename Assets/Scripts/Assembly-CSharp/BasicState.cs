public class BasicState : IState
{
	private StateMachine _stateMachine;

	public StateMachine p_stateMachine
	{
		get
		{
			return _stateMachine;
		}
		set
		{
			_stateMachine = value;
		}
	}

	public virtual void Enter(IStatedObject _parent)
	{
	}

	public virtual void Execute()
	{
	}

	public virtual void Exit()
	{
	}
}
