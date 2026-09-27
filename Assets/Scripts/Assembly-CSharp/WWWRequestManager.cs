using UnityEngine;

public static class WWWRequestManager
{
	public static void RemoveRequestsWithTag(string _tag)
	{
		object[] array = Object.FindObjectsOfType(typeof(WWWRequestThread));
		if (array.Length > 0)
		{
			Debug.Log("Removing " + array.Length + " WWWRequests.");
		}
		for (int i = 0; i < array.Length; i++)
		{
			WWWRequestThread wWWRequestThread = array[i] as WWWRequestThread;
			if (wWWRequestThread.m_downloader.m_tag.Equals(_tag))
			{
				wWWRequestThread.m_downloader.Destroy();
			}
		}
	}
}
