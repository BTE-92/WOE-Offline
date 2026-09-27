using System.Collections.Generic;
using UnityEngine;

public class ParticleTester : MonoBehaviour
{
	private enum SystemType
	{
		None = 0,
		Explosions = 1,
		Flares = 2,
		Flames = 3
	}

	private class FlareSystem
	{
		public string name;

		public GameObject particleObject;

		public ParticleSystem[] particleSystems;

		public bool toggleFlag;

		public bool savedToggleFlag;
	}

	private SystemType systemType;

	public Object[] particleSystems;

	public Object[] loadFlareSystems;

	public Object[] loadDirectionalSystems;

	private List<FlareSystem> flareSystems;

	private List<FlareSystem> directionalSystems;

	private bool expBool;

	private bool flrBool;

	private bool flmBool;

	private Vector2 scrollPosition;

	private SystemType savedSystemType;

	private void Awake()
	{
		expBool = (flrBool = false);
		systemType = SystemType.None;
		savedSystemType = SystemType.None;
		particleSystems = Resources.LoadAll("Explosions", typeof(GameObject));
		loadFlareSystems = Resources.LoadAll("Flares", typeof(GameObject));
		loadDirectionalSystems = Resources.LoadAll("Directional", typeof(GameObject));
		flareSystems = new List<FlareSystem>();
		directionalSystems = new List<FlareSystem>();
		Object[] array = loadFlareSystems;
		foreach (Object obj in array)
		{
			FlareSystem flareSystem = new FlareSystem();
			flareSystem.name = obj.name;
			flareSystem.particleObject = Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
			flareSystem.particleSystems = flareSystem.particleObject.GetComponentsInChildren<ParticleSystem>();
			flareSystem.particleObject.SetActive(false);
			flareSystem.toggleFlag = false;
			flareSystem.particleObject.transform.parent = base.transform;
			flareSystems.Add(flareSystem);
		}
		Object[] array2 = loadDirectionalSystems;
		foreach (Object obj2 in array2)
		{
			FlareSystem flareSystem2 = new FlareSystem();
			flareSystem2.name = obj2.name;
			flareSystem2.particleObject = Object.Instantiate(obj2, Vector3.zero, Quaternion.identity) as GameObject;
			flareSystem2.particleSystems = flareSystem2.particleObject.GetComponentsInChildren<ParticleSystem>();
			flareSystem2.particleObject.SetActive(false);
			flareSystem2.toggleFlag = false;
			flareSystem2.particleObject.transform.parent = base.transform;
			directionalSystems.Add(flareSystem2);
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Toggle(expBool, "Explosions"))
		{
			expBool = SetBool();
			systemType = SystemType.Explosions;
		}
		if (GUILayout.Toggle(flrBool, "Flares"))
		{
			flrBool = SetBool();
			systemType = SystemType.Flares;
		}
		if (GUILayout.Toggle(flmBool, "Flames"))
		{
			flmBool = SetBool();
			systemType = SystemType.Flames;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(250f), GUILayout.Height(550f));
		switch (systemType)
		{
		case SystemType.Explosions:
		{
			Object[] array3 = particleSystems;
			for (int k = 0; k < array3.Length; k++)
			{
				GameObject gameObject = (GameObject)array3[k];
				if (GUILayout.Button(gameObject.name))
				{
					GameObject obj = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
					Object.Destroy(obj, 10f);
				}
			}
			break;
		}
		case SystemType.Flares:
			foreach (FlareSystem flareSystem in flareSystems)
			{
				flareSystem.toggleFlag = GUILayout.Toggle(flareSystem.toggleFlag, flareSystem.name);
				if (flareSystem.toggleFlag == flareSystem.savedToggleFlag)
				{
					continue;
				}
				flareSystem.particleObject.SetActive(flareSystem.toggleFlag);
				if (flareSystem.toggleFlag)
				{
					ParticleSystem[] array2 = flareSystem.particleSystems;
					foreach (ParticleSystem particleSystem2 in array2)
					{
						particleSystem2.Clear();
						particleSystem2.Play();
					}
					flareSystem.toggleFlag = SetFlareBool();
				}
				flareSystem.savedToggleFlag = flareSystem.toggleFlag;
			}
			break;
		case SystemType.Flames:
			foreach (FlareSystem directionalSystem in directionalSystems)
			{
				directionalSystem.toggleFlag = GUILayout.Toggle(directionalSystem.toggleFlag, directionalSystem.name);
				if (directionalSystem.toggleFlag == directionalSystem.savedToggleFlag)
				{
					continue;
				}
				directionalSystem.particleObject.SetActive(directionalSystem.toggleFlag);
				if (directionalSystem.toggleFlag)
				{
					ParticleSystem[] array = directionalSystem.particleSystems;
					foreach (ParticleSystem particleSystem in array)
					{
						particleSystem.Clear();
						particleSystem.Play();
					}
					directionalSystem.toggleFlag = SetDirBool();
				}
				directionalSystem.savedToggleFlag = directionalSystem.toggleFlag;
			}
			break;
		}
		GUILayout.EndScrollView();
	}

	private void Update()
	{
		if (systemType != savedSystemType)
		{
			SetNoFlares();
			savedSystemType = systemType;
		}
	}

	private bool SetBool()
	{
		expBool = (flrBool = (flmBool = false));
		return true;
	}

	private bool SetFlareBool()
	{
		foreach (FlareSystem flareSystem in flareSystems)
		{
			flareSystem.toggleFlag = false;
		}
		return true;
	}

	private bool SetDirBool()
	{
		foreach (FlareSystem directionalSystem in directionalSystems)
		{
			directionalSystem.toggleFlag = false;
		}
		return true;
	}

	private void SetNoFlares()
	{
		foreach (FlareSystem flareSystem in flareSystems)
		{
			flareSystem.toggleFlag = false;
			flareSystem.savedToggleFlag = false;
			flareSystem.particleObject.SetActive(false);
		}
		foreach (FlareSystem directionalSystem in directionalSystems)
		{
			directionalSystem.toggleFlag = false;
			directionalSystem.savedToggleFlag = false;
			directionalSystem.particleObject.SetActive(false);
		}
	}
}
