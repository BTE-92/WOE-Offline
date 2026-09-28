using UnityEngine;

public class UIRenderTextureCanvas : UIScrollableCanvas
{
	private Camera m_renderTextureCamera;

	private bool m_renderTextureCameraIsRegisteredAtCameraS;

	private RenderToTextureBootstrap m_rtbs;

	private RenderTexture m_renderTexture;

	private Texture2D m_finalTexture;

	private PrefabC m_drawRect;

	public UIRenderTextureCanvas(UIComponent _parent, string _tag, Camera _renderTextureCamera = null)
		: base(_parent, _tag)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetMargins(0f);
		m_maxScrollInertialX = 0f;
		m_maxScrollInertialY = 0f;
		m_renderTextureCamera = _renderTextureCamera;
		if (m_renderTextureCamera == null)
		{
			m_renderTextureCameraIsRegisteredAtCameraS = true;
			m_renderTextureCamera = CameraS.AddCamera("RenderToTextureCamera", true);
			m_renderTextureCamera.transform.parent = m_scrollTC.transform;
			m_renderTextureCamera.transform.localPosition = Vector3.zero;
			m_renderTextureCamera.enabled = false;
			m_renderTextureCamera.clearFlags = CameraClearFlags.Color;
			m_renderTextureCamera.backgroundColor = new Color(1f, 1f, 1f, 1f);
		}
		m_rtbs = m_renderTextureCamera.gameObject.AddComponent<RenderToTextureBootstrap>() as RenderToTextureBootstrap;
		m_rtbs.m_uiRenderTextureCanvas = this;
	}

	public void UpdateRenderCamera()
	{
		m_renderTextureCamera.orthographicSize = m_actualHeight * 0.5f;
		Vector3 position = m_TC.transform.position;
		m_renderTextureCamera.transform.localPosition = Vector3.forward * -500f;
		Rect rect = new Rect((position.x - m_actualWidth * 0.5f) / (float)Screen.width + 0.5f, (position.y - m_actualHeight * 0.5f) / (float)Screen.height + 0.5f, m_actualWidth / (float)Screen.width, m_actualHeight / (float)Screen.height);
		m_renderTextureCamera.rect = rect;
	}

	public override void DrawHandler(UIComponent _c)
	{
		UpdateRenderCamera();
		m_rtbs.m_waitingForCallback = true;
		m_renderTextureCamera.enabled = true;
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth, m_actualHeight, Vector2.zero, false);
		DebugDraw.AddRadialRandom(rect, m_actualHeight * 0.15f);
		Color color = DebugDraw.GetColor(234f, 70f, 49f);
		uint num = DebugDraw.ColorToUInt(color);
		if (m_highlight)
		{
			color = new Color(0.3f, 0.3f, 0.3f);
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), m_renderTextureCamera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), m_renderTextureCamera, Position.Center, true);
	}

	public virtual void PreRenderCallback()
	{
		m_renderTexture = RenderTexture.GetTemporary(Mathf.CeilToInt(m_actualWidth), Mathf.CeilToInt(m_actualHeight), 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
		m_renderTextureCamera.targetTexture = m_renderTexture;
		m_renderTexture.DiscardContents();
		RenderTexture.active = m_renderTexture;
		Debug.Log("pre");
	}

	public virtual void PostRenderCallback()
	{
		if (m_finalTexture != null)
		{
			Object.Destroy(m_finalTexture);
		}
		m_finalTexture = new Texture2D(Mathf.CeilToInt(m_actualWidth), Mathf.CeilToInt(m_actualHeight), TextureFormat.ARGB32, false);
		m_finalTexture.ReadPixels(new Rect(0f, 0f, m_renderTexture.width, m_renderTexture.height), 0, 0);
		m_finalTexture.Apply();
		m_renderTextureCamera.targetTexture = null;
		RenderTexture.ReleaseTemporary(m_renderTexture);
		RenderTexture.active = null;
		m_renderTexture = null;
		m_renderTextureCamera.enabled = false;
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		m_drawRect = PrefabS.CreateRect(m_TC, Vector3.zero, Mathf.CeilToInt(m_actualWidth), Mathf.CeilToInt(m_actualHeight), Color.white, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera);
		m_drawRect.p_gameObject.GetComponent<Renderer>().material.mainTexture = m_finalTexture;
		Debug.Log("post");
	}

	public override void Destroy()
	{
		Object.DestroyImmediate(m_rtbs);
		m_rtbs = null;
		if (m_renderTextureCameraIsRegisteredAtCameraS)
		{
			CameraS.RemoveCamera(m_renderTextureCamera);
			m_renderTextureCamera = null;
		}
		if (m_finalTexture != null)
		{
			Object.Destroy(m_finalTexture);
			m_finalTexture = null;
		}
		base.Destroy();
	}
}
