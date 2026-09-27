using UnityEngine;

public class TweenC : BasicComponent
{
	public TransformC p_TC;

	public TweenedProperty tweenedProperty;

	public bool mirrored;

	public int currentRepeat;

	public int repeats;

	public TweenStyle currentTweenStyle;

	public TweenStyle mirroredTweenStyle;

	public Vector3 startValue;

	public Vector3 endValue;

	public Vector3 currentValue;

	public float delay;

	public float duration;

	public float startTime;

	public int delegatedCount;

	public TweenEventDelegate tweenEventDelegate;

	public bool removeEntityAtFinish;

	public bool removeComponentAtFinish;

	public bool hasFinished;

	public TweenC()
		: base(ComponentType.Tween)
	{
	}

	public override void Reset()
	{
		base.Reset();
		p_TC = null;
		duration = 0f;
		currentRepeat = 0;
		repeats = 0;
		delay = 0f;
		mirrored = false;
		removeEntityAtFinish = false;
		removeComponentAtFinish = true;
		hasFinished = false;
	}
}
