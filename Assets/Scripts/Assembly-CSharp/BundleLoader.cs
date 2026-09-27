using System;
using System.IO;
using UnityEngine;

public static class BundleLoader
{
	private static string[] m_bundles;

	private static WWWRequest m_bundleDownloader;

	private static int m_nextBundleToLoad;

	public static bool m_allBundlesLoaded = true;

	private static bool m_asyncLoad;

	private static AssetBundle m_bundle;

	public static AssetBundleRequest m_bundleRequest;

	private static string m_bundleName;

	private static string[] m_bundleAssetNames;

	private static int m_nextBundleAssetToLoad;

	public static event Action AllBundlesLoaded;

	public static void LoadBundles(string bundlesToLoad, bool asyncLoad = false)
	{
		LoadBundles(new string[1] { bundlesToLoad }, asyncLoad);
	}

	public static void LoadBundles(string[] bundlesToLoad, bool asyncLoad = false)
	{
		if (m_allBundlesLoaded)
		{
			Debug.LogInfo("Loading bundles -- ");
			m_bundles = bundlesToLoad;
			m_asyncLoad = asyncLoad;
			m_nextBundleToLoad = 0;
			m_allBundlesLoaded = false;
		}
		else
		{
			Debug.LogError("Old bundle loads pending!!");
		}
	}

	public static bool NeedsToWait()
	{
		if (!m_asyncLoad && !m_allBundlesLoaded)
		{
			return true;
		}
		return false;
	}

	public static void Update()
	{
		if (!m_allBundlesLoaded && m_bundleDownloader == null)
		{
			if (m_nextBundleToLoad < m_bundles.Length)
			{
				string path = m_bundles[m_nextBundleToLoad] + ".assetbundle";
				string text = Path.Combine(Application.streamingAssetsPath, path);
				string text2 = "file://";
				if (text.Contains("://"))
				{
					text2 = string.Empty;
				}
				m_bundleDownloader = new GetRequest(text2 + text, m_nextBundleToLoad.ToString(), false, 5f);
				m_bundleDownloader.requestComplete += BundleDownloadComplete;
				m_bundleDownloader.requestFailed += BundleDownloadFailed;
				m_nextBundleToLoad++;
			}
			else if (m_bundle == null)
			{
				m_allBundlesLoaded = true;
				if (AllBundlesLoaded != null)
				{
					Debug.LogInfo("BUNDLE LOAD COMPLETE: " + m_bundleName);
					AllBundlesLoaded();
				}
			}
		}
		if (!(m_bundle != null))
		{
			return;
		}
		if (m_bundleRequest == null)
		{
			if (m_nextBundleAssetToLoad < m_bundleAssetNames.Length)
			{
				if (m_asyncLoad)
				{
					m_bundleRequest = m_bundle.LoadAsync(m_bundleAssetNames[m_nextBundleAssetToLoad], typeof(UnityEngine.Object));
					m_nextBundleAssetToLoad++;
					return;
				}
				Debug.LogInfo("LOADING ALL BUNDLE ASSETS (Synchronous)");
				for (int i = 0; i < m_bundleAssetNames.Length; i++)
				{
					AddBundleAssetToResources(m_bundle.Load(m_bundleAssetNames[m_nextBundleAssetToLoad]), m_bundleName);
					m_nextBundleAssetToLoad++;
				}
			}
			else
			{
				m_bundle.Unload(false);
				UnityEngine.Object.DestroyImmediate(m_bundle);
				m_bundle = null;
				m_bundleDownloader.Destroy();
				m_bundleDownloader = null;
			}
		}
		else if (m_bundleRequest.isDone)
		{
			UnityEngine.Object asset = m_bundleRequest.asset;
			AddBundleAssetToResources(asset, m_bundleName);
			m_bundleRequest = null;
		}
	}

	private static void AddBundleAssetToResources(UnityEngine.Object asset, string assetGroup)
	{
		if (asset != null)
		{
			UnityResource resource = new UnityResource(asset, assetGroup);
			ResourceManager.AddResourceToGroup(assetGroup, resource);
		}
	}

	private static void BundleDownloadComplete(WWWRequest downloader)
	{
		int num = int.Parse(downloader.m_tag);
		m_bundleName = m_bundles[num];
		Debug.LogInfo("Bundle downloaded: " + m_bundleName);
		m_nextBundleAssetToLoad = 0;
		if (downloader.m_WWW != null)
		{
			if ((bool)downloader.m_WWW.assetBundle)
			{
				m_bundle = downloader.m_WWW.assetBundle;
				string text = (m_bundle.Load("object_names", typeof(TextAsset)) as TextAsset).text;
				m_bundleAssetNames = text.Split(',');
				ResourceManager.AddResourceGroup(m_bundleName);
			}
			else
			{
				Debug.LogError("No assetbundle!");
			}
		}
	}

	private static void BundleDownloadFailed(WWWRequest downloader)
	{
		Debug.LogError("Bundle load error! " + downloader.m_WWW.error);
		m_bundleDownloader.Destroy();
		m_bundleDownloader = null;
	}
}
