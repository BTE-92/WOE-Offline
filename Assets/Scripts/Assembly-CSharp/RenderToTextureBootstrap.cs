using UnityEngine;

public class RenderToTextureBootstrap : MonoBehaviour
{
	public UIRenderTextureCanvas m_uiRenderTextureCanvas;

	public bool m_waitingForCallback;

	private void OnPreRender()
	{
		if (m_waitingForCallback)
		{
			m_uiRenderTextureCanvas.PreRenderCallback();
		}
	}

	private void OnPostRender()
	{
		if (m_waitingForCallback)
		{
			m_uiRenderTextureCanvas.PostRenderCallback();
			m_waitingForCallback = false;
		}
	}
}
