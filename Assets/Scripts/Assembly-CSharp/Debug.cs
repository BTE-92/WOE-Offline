using UnityEngine;

public static class Debug
{
	public static bool m_filterLog = true;

	public static bool m_filterLogError = true;

	public static bool m_filterLogWarning = true;

	public static bool m_filterLogInfo = true;

	public static void Initialize(bool _log, bool _error, bool _warning, bool _info)
	{
		m_filterLog = _log;
		m_filterLogError = _error;
		m_filterLogWarning = _warning;
		m_filterLogInfo = _info;
	}

	public static void Log(object message)
	{
		if (m_filterLog)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public static void LogError(object message)
	{
		if (m_filterLogError)
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	public static void LogWarning(object message)
	{
		if (m_filterLogWarning)
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public static void LogInfo(object message)
	{
		if (m_filterLogInfo)
		{
			UnityEngine.Debug.Log(message);
		}
	}
}
