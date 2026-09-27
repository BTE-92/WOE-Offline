using System;

public class PostCommand
{
	public string m_command;

	public Action<WWWRequest> m_successEventHandler;

	public Action<WWWRequest> m_failEventHandler;

	public PostCommand(string _command, Action<WWWRequest> _successEventHandler = null, Action<WWWRequest> _failEventHandler = null)
	{
		m_command = _command;
		m_successEventHandler = _successEventHandler;
		m_failEventHandler = _failEventHandler;
	}
}
