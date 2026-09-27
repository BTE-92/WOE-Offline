public class CameraBorderC : BasicComponent
{
	public TransformC TC;

	public float leftOffset;

	public float rightOffset;

	public float topOffset;

	public float bottomOffset;

	public bool hasLeft;

	public bool hasRight;

	public bool hasTop;

	public bool hasBottom;

	public CameraBorderC()
		: base(ComponentType.CameraBorder)
	{
	}

	public override void Reset()
	{
		base.Reset();
		hasLeft = false;
		hasRight = false;
		hasTop = false;
		hasBottom = false;
	}
}
