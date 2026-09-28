using UnityEngine;

public class DynamicAnimation : MonoBehaviour
{
	private AnimationCurve xcurve;

	private AnimationCurve ycurve;

	private AnimationCurve zcurve;

	private AnimationCurve wcurve;

	private void Start()
	{
		AnimationClip animationClip = new AnimationClip();
		animationClip.wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().AddClip(animationClip, "test");
		base.GetComponent<Animation>().Play("test");
		Quaternion quaternion = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		xcurve = new AnimationCurve(new Keyframe(0f, quaternion.x), new Keyframe(1f, quaternion.x));
		ycurve = new AnimationCurve(new Keyframe(0f, quaternion.y), new Keyframe(1f, quaternion.y));
		zcurve = new AnimationCurve(new Keyframe(0f, quaternion.z), new Keyframe(1f, quaternion.z));
		wcurve = new AnimationCurve(new Keyframe(0f, quaternion.w), new Keyframe(1f, quaternion.w));
	}

	private void Update()
	{
		Quaternion quaternion = Quaternion.Euler(new Vector3(0f, 180f, Random.Range(-180, 180)));
		xcurve.MoveKey(0, new Keyframe(0f, quaternion.x));
		xcurve.MoveKey(1, new Keyframe(1f, quaternion.x));
		ycurve.MoveKey(0, new Keyframe(0f, quaternion.y));
		ycurve.MoveKey(1, new Keyframe(1f, quaternion.y));
		zcurve.MoveKey(0, new Keyframe(0f, quaternion.z));
		zcurve.MoveKey(1, new Keyframe(1f, quaternion.z));
		wcurve.MoveKey(0, new Keyframe(0f, quaternion.w));
		wcurve.MoveKey(1, new Keyframe(1f, quaternion.w));
		AnimationClip clip = base.GetComponent<Animation>().GetClip("test");
		clip.SetCurve(string.Empty, typeof(Transform), "localRotation.x", xcurve);
		clip.SetCurve(string.Empty, typeof(Transform), "localRotation.y", ycurve);
		clip.SetCurve(string.Empty, typeof(Transform), "localRotation.z", zcurve);
		clip.SetCurve(string.Empty, typeof(Transform), "localRotation.w", wcurve);
	}
}
