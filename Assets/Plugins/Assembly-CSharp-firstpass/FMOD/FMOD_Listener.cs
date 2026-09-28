#if FMOD_LIVEUPDATE
#  define RUN_IN_BACKGROUND
#endif

using System;
using System.Collections;
using System.IO;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public class FMOD_Listener : MonoBehaviour 
{
	public string[] pluginPaths;
	
	static FMOD_Listener sListener = null;

    public GameObject m_listener;

    void OnEnable()
	{		
		Initialize();
	}
	
	void loadBank(string fileName)
	{
		string bankPath = getStreamingAsset(fileName);
		
		FMOD.Studio.Bank bank = null;
		FMOD.RESULT result = FMOD_StudioSystem.instance.System.loadBankFile(bankPath, LOAD_BANK_FLAGS.NORMAL, out bank);
		if (result == FMOD.RESULT.ERR_VERSION)
		{
			FMOD.Studio.UnityUtil.LogError("These banks were built with an incompatible version of FMOD Studio.");
		}
		
		FMOD.Studio.UnityUtil.Log("bank load: " + (bank != null ? "suceeded" : "failed!!"));
	}
	
	string getStreamingAsset(string fileName)
	{
		string bankPath = "";
		if (Application.platform == RuntimePlatform.WindowsEditor || 
			Application.platform == RuntimePlatform.OSXEditor ||
		    Application.platform == RuntimePlatform.WindowsPlayer ||
		    Application.platform == RuntimePlatform.LinuxPlayer
#if PLATFORM_PS4
		    || Application.platform == RuntimePlatform.PS4
#endif
#if UNITY_XBOXONE
			|| Application.platform == RuntimePlatform.XboxOne
#endif
		    )
		{
			bankPath = Application.dataPath + "/StreamingAssets";
		}
		else if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			bankPath = Application.dataPath + "/Data/StreamingAssets";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			bankPath = Application.dataPath + "/Raw";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			bankPath = "jar:file://" + Application.dataPath + "!/assets";
		}
		else
		{		
			FMOD.Studio.UnityUtil.LogError("Unknown platform!");
			return "";
		}
		
		string assetPath = bankPath + "/" + fileName;
		
#if UNITY_ANDROID && !UNITY_EDITOR
		// Unpack the compressed JAR file
		string unpackedJarPath = Application.persistentDataPath + "/" + fileName;
		
		FMOD.Studio.UnityUtil.Log("Unpacking bank from JAR file into:" + unpackedJarPath);
		
		if (File.Exists(unpackedJarPath))
		{
			FMOD.Studio.UnityUtil.Log("File already unpacked!!");
			File.Delete(unpackedJarPath);
			
			if (File.Exists(unpackedJarPath))
			{
				FMOD.Studio.UnityUtil.Log("Could NOT delete!!");				
			}
		}
		
		WWW dataStream = new WWW(assetPath);
		
		while(!dataStream.isDone) {} // FIXME: not safe
		
		
		if (!String.IsNullOrEmpty(dataStream.error))
		{
	        FMOD.Studio.UnityUtil.LogError("### WWW ERROR IN DATA STREAM:" + dataStream.error);
		}
		
		FMOD.Studio.UnityUtil.Log("Android unpacked jar path: " + unpackedJarPath);
		
		File.WriteAllBytes(unpackedJarPath, dataStream.bytes);
		
		//FileInfo fi = new FileInfo(unpackedJarPath);
		//FMOD.Studio.UnityUtil.Log("Unpacked bank size = " + fi.Length);
		
		assetPath = unpackedJarPath;
#endif

		return assetPath;
	}
	
	void Initialize()
	{
		FMOD.Studio.UnityUtil.Log("Initialize Listener");

		if (sListener != null)
		{
			FMOD.Studio.UnityUtil.LogError("Too many listeners");
		}
		
		sListener = this;
		
		LoadPlugins();
		
		const string listFileName = "FMOD_bank_list.txt";
		string bankListPath = getStreamingAsset(listFileName);
		if (!System.IO.File.Exists(bankListPath))
		{
			FMOD.Studio.UnityUtil.LogError(bankListPath + " not found, no banks loaded.");
		}
		else
		{
			FMOD.Studio.UnityUtil.Log("Loading Banks");
			var bankList = System.IO.File.ReadAllLines(bankListPath);
			foreach (var bankName in bankList)
			{
				FMOD.Studio.UnityUtil.Log("Load " + bankName);
				loadBank(bankName);
			}
		}
	}
	
	void Start()
	{
#if UNITY_EDITOR && RUN_IN_BACKGROUND
		Application.runInBackground = true; // Prevent execution pausing when editor loses focus
#endif
	}

    private void Update()
    {
        FMOD.Studio._3D_ATTRIBUTES listenerAttributes = m_listener.to3DAttributes();
        FMOD_StudioSystem.instance.System.setListenerAttributes(listenerAttributes);
    }

    void LoadPlugins()
	{
		FMOD.System sys = null;
		ERRCHECK(FMOD_StudioSystem.instance.System.getLowLevelSystem(out sys));
		
		var dir = pluginPath;
		foreach (var name in pluginPaths)
		{
			var path = dir + "/" + GetPluginFileName(name);
			
			FMOD.Studio.UnityUtil.Log("Loading plugin: " + path);
			if (!System.IO.File.Exists(path))
            {
                FMOD.Studio.UnityUtil.LogWarning("plugin not found: " + path);
            }
			
			uint handle;
			ERRCHECK(sys.loadPlugin(path, out handle));
		}
	}	
	
	string pluginPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Application.dataPath + "/Plugins/x86";
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer ||
			         Application.platform == RuntimePlatform.OSXEditor ||
			         Application.platform == RuntimePlatform.OSXPlayer ||
			         Application.platform == RuntimePlatform.LinuxPlayer
#if PLATFORM_PS4
				     || Application.platform == RuntimePlatform.PS4
#endif
#if UNITY_XBOXONE
				     || Application.platform == RuntimePlatform.XboxOne
#endif
			    	)
			{
				return Application.dataPath + "/Plugins";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				FMOD.Studio.UnityUtil.LogError("Plugins not currently supported on iOS, contact support@fmod.org for more information");
				return "";
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				var dirInfo = new System.IO.DirectoryInfo(Application.persistentDataPath);
				string packageName = dirInfo.Parent.Name;
				return "/data/data/" + packageName + "/lib";
			}
			
			FMOD.Studio.UnityUtil.LogError("Unknown platform!");
			return "";
		}
	}
	
	string GetPluginFileName(string rawName)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.WindowsPlayer
#if UNITY_XBOXONE
		    || Application.platform == RuntimePlatform.XboxOne
#endif
		    )
		{
			return rawName + ".dll";
		}
		else if (Application.platform == RuntimePlatform.OSXEditor ||
		         Application.platform == RuntimePlatform.OSXPlayer)
		{
			return rawName + ".dylib";
		}
		else if (Application.platform == RuntimePlatform.Android ||
		         Application.platform == RuntimePlatform.LinuxPlayer)
		{
			return "lib" + rawName + ".so";
		}
#if PLATFORM_PS4
		else if (Application.platform == RuntimePlatform.PS4)
		{
			return rawName + ".prx";
		}
#endif
		
		FMOD.Studio.UnityUtil.LogError("Unknown platform!");
		return "";		
	}

	void ERRCHECK(FMOD.RESULT result)
	{
		FMOD.Studio.UnityUtil.ERRCHECK(result);
	}
}
