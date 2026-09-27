using UnityEngine;

public class CameraEffectC : BasicComponent
{
	public Camera camera;

	public float begin;

	public float duration;

	public float amount;

	public float falloff;

	public float interval;

	public float last;

	public Vector3 effect;

	public CameraEffectC()
		: base(ComponentType.CameraEffect)
	{
		begin = Main.m_gameTime;
		duration = 2f;
		amount = 10f;
		falloff = 0.9f;
		interval = 0.2f;
		last = 0f;
		effect = Vector3.zero;
	}
}
