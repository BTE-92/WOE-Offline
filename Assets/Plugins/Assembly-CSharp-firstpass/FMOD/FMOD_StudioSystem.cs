using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FMOD
{
	namespace Studio
	{
		public static class UnityUtil
		{	
			static public VECTOR toFMODVector(this Vector3 vec)
			{
				VECTOR temp;
				temp.x = vec.x;
				temp.y = vec.y;
				temp.z = vec.z;
				
				return temp;
			}
			
			static public _3D_ATTRIBUTES to3DAttributes(this Vector3 pos)
			{
				FMOD.Studio._3D_ATTRIBUTES attributes = new FMOD.Studio._3D_ATTRIBUTES();
				attributes.forward = toFMODVector(Vector3.forward);
				attributes.up = toFMODVector(Vector3.up);
				attributes.position = toFMODVector(pos);
				
				return attributes;
			}
			
			static public _3D_ATTRIBUTES to3DAttributes(this GameObject go)
			{
				FMOD.Studio._3D_ATTRIBUTES attributes = new FMOD.Studio._3D_ATTRIBUTES();
				attributes.forward = toFMODVector(go.transform.forward);
				attributes.up = toFMODVector(go.transform.up);
				attributes.position = toFMODVector(go.transform.position);
		
				if (go.rigidbody)
					attributes.velocity = toFMODVector(go.rigidbody.velocity);
				
				return attributes;
			}
			
			static public void Log(string msg)
			{
#if FMOD_DEBUG
				Debug.Log(msg);
#endif
			}
			
			static public void LogWarning(string msg)
			{
				Debug.LogWarning(msg);
			}
			
			static public void LogError(string msg)
			{
				Debug.LogError(msg);
			}
			
			static public bool ForceLoadLowLevelBinary()
			{
				// This is a hack that forces Android to load the .so libraries in the correct order
#if UNITY_ANDROID && !UNITY_EDITOR
				FMOD.Studio.UnityUtil.Log("loading binaries: " + FMOD.Studio.STUDIO_VERSION.dll + " and " + FMOD.VERSION.dll);
				AndroidJavaClass jSystem = new AndroidJavaClass("java.lang.System");
				jSystem.CallStatic("loadLibrary", FMOD.VERSION.dll);
				jSystem.CallStatic("loadLibrary", FMOD.Studio.STUDIO_VERSION.dll);
#endif

				// Hack: force the low level binary to be loaded before accessing Studio API
#if !UNITY_IPHONE || UNITY_EDITOR
				FMOD.Studio.UnityUtil.Log("Attempting to call Memory_GetStats");
				int temp1, temp2;
				if (!ERRCHECK(FMOD.Memory.GetStats(out temp1, out temp2)))
				{
					FMOD.Studio.UnityUtil.LogError("Memory_GetStats returned an error");
					return false;
				}
		
				FMOD.Studio.UnityUtil.Log("Calling Memory_GetStats succeeded!");
#endif
		
				return true;
			}
			
			public static bool ERRCHECK(FMOD.RESULT result)
			{
				if (result != FMOD.RESULT.OK)
				{
					LogWarning("FMOD Error (" + result.ToString() + "): " + FMOD.Error.String(result));
				}
				
				return (result == FMOD.RESULT.OK);
			}
		}
	}
}

public class FMOD_StudioSystem : MonoBehaviour 
{
	FMOD.Studio.System system;
	Dictionary<string, FMOD.Studio.EventDescription> eventDescriptions = new Dictionary<string, FMOD.Studio.EventDescription>();
	bool isInitialized = false;
	
	static FMOD_StudioSystem sInstance;
	public static FMOD_StudioSystem instance
	{
		get
		{
			if (sInstance == null)
			{
				var go = new GameObject("FMOD_StudioSystem");
				sInstance = go.AddComponent<FMOD_StudioSystem>();
				
				if (!FMOD.Studio.UnityUtil.ForceLoadLowLevelBinary()) // do these hacks before calling ANY fmod functions!
				{
					FMOD.Studio.UnityUtil.LogError("Unable to load low level binary!");
					return sInstance;
				}
				sInstance.Init();
			}
			return sInstance;
		}
	}
	
	public FMOD.Studio.EventInstance GetEvent(FMODAsset asset)
	{
		return GetEvent(asset.id);
	}
	
	public FMOD.Studio.EventInstance GetEvent(string path)
	{
		FMOD.Studio.EventInstance instance = null;
		
		if (string.IsNullOrEmpty(path))
		{
			FMOD.Studio.UnityUtil.LogError("Empty event path!");
			return null;
		}
		
		if (eventDescriptions.ContainsKey(path))
		{
			ERRCHECK(eventDescriptions[path].createInstance(out instance));
		}
		else
		{
			FMOD.GUID id = new FMOD.GUID();
			
			if (path.StartsWith("{"))
			{
				ERRCHECK(FMOD.Studio.Util.ParseID(path, out id));
			}
			else if (path.StartsWith("event:"))
			{
				ERRCHECK(system.lookupID(path, out id));
			}
			else
			{
				FMOD.Studio.UnityUtil.LogError("Expected event path to start with 'event:/'");
			}
			
			FMOD.Studio.EventDescription desc = null;
			ERRCHECK(system.getEvent(id, FMOD.Studio.LOADING_MODE.BEGIN_NOW, out desc));
			
			if (desc != null && desc.isValid())
			{
				eventDescriptions.Add(path, desc);
				ERRCHECK(desc.createInstance(out instance));
			}
		}
		
		if (instance == null)
		{
			FMOD.Studio.UnityUtil.Log("GetEvent FAILED: \"path\"");
		}
		
		return instance;
	}
	
	public void PlayOneShot(FMODAsset asset, Vector3 position)
	{
		PlayOneShot(asset.id, position);
	}	
	
	public void PlayOneShot(string path, Vector3 position)
	{
		PlayOneShot(path, position, 1.0f);
	}
	
	void PlayOneShot(string path, Vector3 position, float volume)
	{
		var instance = GetEvent(path);
		
		var attributes = FMOD.Studio.UnityUtil.to3DAttributes(position);
		ERRCHECK( instance.set3DAttributes(attributes) );
		//TODO ERRCHECK( instance.setVolume(volume) );
		ERRCHECK( instance.start() );
		ERRCHECK( instance.release() );
	}
	
	public FMOD.Studio.System System
	{
		get { return system; }
	}
	
	void Init() 
	{
		FMOD.Studio.UnityUtil.Log("FMOD_StudioSystem: Initialize");
		
		if (isInitialized)
		{
			return;
		}
		
		FMOD.Studio.UnityUtil.Log("FMOD_StudioSystem: System_Create");
        ERRCHECK(FMOD.Studio.Factory.System_Create(out system));
		
		FMOD.Studio.INITFLAGS flags = FMOD.Studio.INITFLAGS.NORMAL;

#if FMOD_LIVEUPDATE
		flags |= FMOD.Studio.INITFLAGS.LIVEUPDATE;
#endif

		FMOD.Studio.UnityUtil.Log("FMOD_StudioSystem: system.init");
		FMOD.RESULT result = FMOD.RESULT.OK;
		result = system.init(1024, flags, FMOD.INITFLAGS.NORMAL, global::System.IntPtr.Zero);
		
		if (result == FMOD.RESULT.ERR_NET_SOCKET_ERROR)
		{
#if false && FMOD_LIVEUPDATE //FIXME: test this
			FMOD.Studio.UnityUtil.LogWarning("LiveUpdate disabled: socket in already in use");
			flags &= ~FMOD.Studio.INITFLAGS.LIVEUPDATE;
        	result = system.init(64, flags, FMOD.INITFLAGS.NORMAL, (System.IntPtr)null);
#else
			FMOD.Studio.UnityUtil.LogWarning("Unable to initalize with LiveUpdate: socket is already in use");			
#endif
		}
		ERRCHECK(result);
		
		isInitialized = true;
	}
	
	void OnApplicationPause(bool pauseStatus)
	{
		FMOD.System sys;
		ERRCHECK(system.getLowLevelSystem(out sys));
		
		FMOD.Studio.UnityUtil.Log("Pause state changed to: " + pauseStatus);
		
		if (pauseStatus)
		{
			ERRCHECK(sys.mixerSuspend());
		}
		else
		{
			ERRCHECK(sys.mixerResume());
		}
	}
	
	void Update() 
	{
		if (isInitialized)
		{
			ERRCHECK(system.update());
		}
	}
	
	void OnDisable()
	{
		if (isInitialized)
		{
			FMOD.Studio.UnityUtil.Log("__ SHUT DOWN FMOD SYSTEM __");
			ERRCHECK(system.release());
		}
	}
	
	static bool ERRCHECK(FMOD.RESULT result)
	{
		return FMOD.Studio.UnityUtil.ERRCHECK(result);
	}
}
