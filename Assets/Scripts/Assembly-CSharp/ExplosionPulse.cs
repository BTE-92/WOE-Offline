using System;
using UnityEngine;

public class ExplosionPulse : MonoBehaviour
{
	public float loopduration = 2f;

	public float Expansion = 1f;

	public float EvaporationDelay = 1f;

	public float EvaporationSpeed = 0.5f;

	private float duration;

	private float rangeX = -0.25f;

	private float rangeY = 0.5f;

	public float targetSizeMultiplier = 20f;

	private Vector3 targetSize;

	private Vector3 startSize;

	private void Start()
	{
		targetSize = new Vector3(base.transform.localScale.x * targetSizeMultiplier, base.transform.localScale.y * targetSizeMultiplier, base.transform.localScale.z * targetSizeMultiplier);
		startSize = base.transform.localScale;
	}

	private void Init()
	{
		base.transform.localScale = startSize;
		targetSize = new Vector3(base.transform.localScale.x * targetSizeMultiplier, base.transform.localScale.y * targetSizeMultiplier, base.transform.localScale.z * targetSizeMultiplier);
		duration = 0f;
		rangeX = -0.25f;
		rangeY = 0.5f;
	}

	private void Update()
	{
		float num = Mathf.Sin(Time.time / loopduration * ((float)Math.PI * 2f)) * 0.5f + 0.25f;
		float num2 = Mathf.Sin((Time.time / loopduration + 1f / 3f) * 2f * (float)Math.PI) * 0.5f + 0.25f;
		float num3 = Mathf.Sin((Time.time / loopduration + 2f / 3f) * 2f * (float)Math.PI) * 0.5f + 0.25f;
		float num4 = 1f / (num + num2 + num3);
		num *= num4;
		num2 *= num4;
		num3 *= num4;
		base.renderer.material.SetVector("_ChannelFactor", new Vector4(num, num2, num3, 0f));
		base.transform.localScale = Vector3.Slerp(base.transform.localScale, targetSize, Expansion * Time.deltaTime * duration * 2f);
		base.renderer.material.SetVector("_Range", new Vector4(rangeX, rangeY, 0f, 1f));
		duration += Time.deltaTime;
		if (duration > EvaporationDelay)
		{
			rangeX += Time.deltaTime * EvaporationSpeed;
			rangeY += Time.deltaTime * EvaporationSpeed;
		}
	}
}
