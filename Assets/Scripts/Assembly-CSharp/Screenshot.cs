using UnityEngine;

public class Screenshot : MonoBehaviour
{
	private bool m_takeShot;

	private Texture2D m_screenshotTexture;

	private void Start()
	{
		m_takeShot = false;
	}

	private void Update()
	{
	}

	private void OnPostRender()
	{
		if (m_takeShot)
		{
			m_takeShot = false;
			Rect source = new Rect(0f, 0f, Screen.width, Screen.height);
			if (Screen.width > Screen.height)
			{
				source = new Rect(Mathf.RoundToInt((float)(Screen.width - Screen.height) * 0.5f), 0f, Screen.height, Screen.height);
			}
			else if (Screen.width <= Screen.height)
			{
				source = new Rect(0f, Mathf.RoundToInt((float)(Screen.height - Screen.width) * 0.5f), Screen.width, Screen.width);
			}
			m_screenshotTexture = new Texture2D(Mathf.RoundToInt(source.width), Mathf.RoundToInt(source.height), TextureFormat.RGB24, false);
			m_screenshotTexture.ReadPixels(source, 0, 0);
			m_screenshotTexture.Apply();
			TextureScale.Bilinear(m_screenshotTexture, 512, 512);
			byte[] png = m_screenshotTexture.EncodeToPNG();
			if (PsState.m_lastDownloadedLevelId != null)
			{
				PsState.m_pendingScreenshotPng = null;
				PsState.m_pendingScreenshotLevel = null;
				Server.SaveScreenshot(PsState.m_lastDownloadedLevelId, png, ScreenshotOK, ScreenshotFAIL);
			}
			else
			{
				PsState.m_pendingScreenshotPng = png;
				PsState.m_pendingScreenshotLevel = LevelManager.m_currentLevel;
			}
		}
	}

	public void TakeScreenshot()
	{
		m_takeShot = true;
	}

	private void ScreenshotOK(WWWRequest _request)
	{
	}

	private void ScreenshotFAIL(WWWRequest _request)
	{
	}
}
