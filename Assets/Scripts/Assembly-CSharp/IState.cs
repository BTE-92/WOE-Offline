public interface IState
{
	StateMachine p_stateMachine { get; set; }

	void Enter(IStatedObject _parent);

	void Execute();

	void Exit();
}
