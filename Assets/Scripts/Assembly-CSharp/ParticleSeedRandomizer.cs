using UnityEngine;

public class ParticleSeedRandomizer : MonoBehaviour
{
	private void Start()
	{
		if ((bool)base.gameObject)
		{
			int randomSeed = Random.Range(0, 99999);
			base.gameObject.particleSystem.randomSeed = (uint)randomSeed;
		}
	}

	private void Update()
	{
	}
}
