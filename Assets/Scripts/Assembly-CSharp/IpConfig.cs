using System;

public static class IpConfig
{
	// Hardcoded: every server request is answered by the built-in OfflineBackend (see WWWResult).
	public const string ServerUrl = "offline://local";

	public static bool IsOfflineUrl(string _url)
	{
		return _url != null && _url.StartsWith("offline://", StringComparison.Ordinal);
	}
}
