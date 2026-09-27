public class UndoAction
{
	public virtual void ApplyUndo()
	{
		Debug.Log("undo");
	}

	public virtual void ApplyRedo()
	{
		Debug.Log("redo");
	}
}
