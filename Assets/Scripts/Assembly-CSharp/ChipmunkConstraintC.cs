using System;

public class ChipmunkConstraintC : BasicComponent
{
	public IntPtr constraint;

	public ucpConstraintType type;

	public ChipmunkBodyC bodyA;

	public ChipmunkBodyC bodyB;

	public TransformC grooveA;

	public TransformC grooveB;

	public TransformC anchor1;

	public TransformC anchor2;

	public ChipmunkConstraintC()
		: base(ComponentType.ChipmunkConstraint)
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
		grooveA = null;
		grooveB = null;
		anchor1 = null;
		anchor2 = null;
	}

	~ChipmunkConstraintC()
	{
	}
}
