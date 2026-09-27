using System;
using System.Collections.Generic;
using UnityEngine;

public static class SocialManager
{
	public static Dictionary<string, PlayerData> m_friendData = new Dictionary<string, PlayerData>();

	public static void GetPicture(string _facebookId, string _gameCenterId, Action<Texture2D> _callback)
	{
		if (_facebookId != null)
		{
			FacebookManager.GetPicture(_facebookId, _callback);
		}
		else
		{
			_callback(ResourceManager.GetTexture("UI/AnonymousProfile") as Texture2D);
		}
	}
}
