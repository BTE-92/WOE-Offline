using UnityEngine;

public class CameraTargetC : BasicComponent
{
	public TransformC TC;

	public cpBB bb;

	public Vector2 lowVelocity;

	public Vector2 highVelocity;

	public Vector2 lowScaleVelocity;

	public Vector2 highScaleVelocity;

	public float horizontalOffset;

	public float verticalOffset;

	public float safeFrame;

	public float velocityScale;

	public Vector3 prevPos;

	public Vector3 offset;

	public float scale;

	public float maxOffsetChange;

	public float maxScaleChange;

	public Vector2 maxAngleChange;

	public float horizontalAngle;

	public float verticalAngle;

	public CameraTargetC()
		: base(ComponentType.CameraTarget)
	{
	}

	public override void Reset()
	{
		base.Reset();
		lowVelocity = new Vector2(0.5f, 2f);
		highVelocity = new Vector2(3f, 5f);
		lowScaleVelocity = new Vector2(2f, 2f);
		highScaleVelocity = new Vector2(15f, 15f);
		horizontalOffset = 0f;
		verticalOffset = -0.15f;
		safeFrame = 0.25f;
		velocityScale = 1f;
		prevPos = Vector3.zero;
		offset = Vector3.zero;
		maxOffsetChange = 2f;
		maxScaleChange = 0.1f;
		scale = 1f;
		horizontalAngle = 0f;
		verticalAngle = 10f;
		maxAngleChange = Vector2.one * 0.25f;
	}
}
