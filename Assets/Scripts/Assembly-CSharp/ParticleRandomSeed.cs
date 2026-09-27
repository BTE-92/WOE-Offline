using UnityEngine;

public class ParticleRandomSeed : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.particleSystem.randomSeed = (uint)Random.Range(-99999, 99999);
	}

	private void Update()
	{
	}
}
