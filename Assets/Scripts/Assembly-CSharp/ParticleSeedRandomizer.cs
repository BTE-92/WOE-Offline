using UnityEngine;

public class ParticleSeedRandomizer : MonoBehaviour
{
	private void Start()
	{
		if ((bool)base.gameObject)
		{
			ParticleSystem ps = base.gameObject.GetComponent<ParticleSystem>();
			bool wasPlaying = ps.isPlaying;
			if (wasPlaying)
			{
				ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			ps.useAutoRandomSeed = false;
			ps.randomSeed = (uint)Random.Range(0, 99999);
			if (wasPlaying)
			{
				ps.Play(false);
			}
		}
	}

	private void Update()
	{
	}
}