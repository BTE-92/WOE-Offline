using System;
using System.IO;
using UnityEngine;
public static class IpConfig //Added purely for convienience of using custom servers as the game is obviously dead
{
	private const string CONFIG_FILENAME = "ipconfig.txt";

	// Fallback used whenever the file is missing, unreadable, or contains
	// something that isn't a well-formed http(s) URL.
	private const string DEFAULT_SERVER_URL =
		"http://PlayDevLB-1049210432.us-west-2.elb.amazonaws.com";

	// Sanity cap so a runaway/garbage file can't make us read something
	// absurd into memory before we even get to validate it.
	private const int MAX_FILE_LENGTH = 2048;

	private static string s_serverUrl;
	private static bool s_loaded;

	public static string ServerUrl
	{
		get
		{
			if (!s_loaded)
			{
				Load();
			}
			return s_serverUrl;
		}
	}

    private static string ConfigDirectory
    {
        get { return Application.persistentDataPath; }
    }

    private static string ConfigPath
	{
		get { return Path.Combine(ConfigDirectory, CONFIG_FILENAME); }
	}

	/// <summary>
	/// Loads the server URL from disk. Falls back to (and rewrites) the
	/// default on any problem: missing file, IO error, oversized file,
	/// empty contents, or a malformed/non-http(s) URL. This never throws.
	/// </summary>
	public static void Load()
	{
		s_loaded = true;

		try
		{
			if (!File.Exists(ConfigPath))
			{
				Debug.Log("IpConfig: no config file found, writing default.");
				WriteDefault();
				return;
			}

			var fileInfo = new FileInfo(ConfigPath);
			if (fileInfo.Length > MAX_FILE_LENGTH)
			{
				Debug.LogWarning("IpConfig: config file is unexpectedly large (" + fileInfo.Length + " bytes), resetting to default.");
				WriteDefault();
				return;
			}

			// First non-empty line is the URL; anything after is ignored,
			// so stray trailing blank lines/newlines from hand-editing
			// don't cause a false failure.
			string raw = File.ReadAllText(ConfigPath);
			string url = null;
			foreach (string line in raw.Split('\n'))
			{
				string trimmed = line.Trim();
				if (trimmed.Length > 0)
				{
					url = trimmed;
					break;
				}
			}

			if (string.IsNullOrEmpty(url) || !IsWellFormedUrl(url))
			{
				Debug.LogWarning("IpConfig: config file did not contain a well-formed http(s) URL, resetting to default.");
				WriteDefault();
				return;
			}

			s_serverUrl = url;
			Debug.Log("IpConfig: loaded server URL from config: " + s_serverUrl);
		}
		catch (Exception e)
		{
			Debug.LogError("IpConfig: failed to read config (" + e.Message + "), resetting to default.");
			WriteDefault();
		}
	}

	private static void WriteDefault()
	{
		s_serverUrl = DEFAULT_SERVER_URL;

		try
		{
			File.WriteAllText(ConfigPath, DEFAULT_SERVER_URL);
		}
		catch (Exception e)
		{
			// Even if we can't persist it, we can still run with the
			// default in memory for this session.
			Debug.LogError("IpConfig: failed to write default config: " + e.Message);
		}
	}

	/// <summary>
	/// Deletes the config file and reverts to the default URL. Useful for
	/// a "reset network settings" debug option.
	/// </summary>
	public static void ResetToDefault()
	{
		try
		{
			if (File.Exists(ConfigPath))
			{
				File.Delete(ConfigPath);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("IpConfig: failed to delete config: " + e.Message);
		}

		WriteDefault();
		s_loaded = true;
	}

	private static bool IsWellFormedUrl(string _url)
	{
		Uri result;
		bool parsed = Uri.TryCreate(_url, UriKind.Absolute, out result);
		if (!parsed)
		{
			return false;
		}
		return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
	}
}
