using System.Collections.Generic;
using UnityEngine;

public class UIProfileImage : UIComponent
{
	public TransformC m_prefabTC;

	public Texture2D m_texture;

	public List<PrefabC> m_prefabs;

	public UIProfileImage(UIComponent _parent, bool _touchable, string _tag, string _facebookId, string _gameCenterId)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetMargins(0f);
		m_prefabTC = TransformS.AddComponent(m_TC.p_entity, "PrefabTC");
		TransformS.ParentComponent(m_prefabTC, m_TC, Vector3.zero);
		SocialManager.GetPicture(_facebookId, _gameCenterId, ScreenshotOK);
	}

	private void ScreenshotOK(Texture2D _texture)
	{
		m_texture = _texture;
		if (m_prefabTC != null)
		{
			PrefabS.RemoveComponentsByEntity(m_prefabTC.p_entity);
			Material material = Object.Instantiate(ResourceManager.GetMaterial("UI/ProfileMat")) as Material;
			material.mainTexture = m_texture;
			float width = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
			float height = m_actualHeight - m_actualMargins.b - m_actualMargins.t;
			Vector2[] rect = DebugDraw.GetRect(width, height, Vector2.zero);
			m_prefabs = PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_prefabTC, Vector3.forward * -1f, rect, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), material, m_camera, "Profile", UVRect.Normal());
			m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, Vector3.forward * -2f, rect, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), m_camera, Position.Center, true));
			m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, Vector3.zero, rect, 8f, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		TransformS.SetPosition(m_prefabTC, new Vector2(m_actualMargins.l - m_actualMargins.r, m_actualMargins.b - m_actualMargins.t) * 0.5f);
		if (m_prefabs != null)
		{
			PrefabS.RemoveComponentsByEntity(m_prefabTC.p_entity);
		}
		Material material = Object.Instantiate(ResourceManager.GetMaterial("UI/ProfileMat")) as Material;
		material.mainTexture = m_texture;
		float width = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
		float height = m_actualHeight - m_actualMargins.b - m_actualMargins.t;
		Vector2[] rect = DebugDraw.GetRect(width, height, Vector2.zero);
		m_prefabs = PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_prefabTC, Vector3.forward * -1f, rect, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), material, m_camera, "Profile", UVRect.Normal());
		m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, Vector3.forward * -2f, rect, 4f, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), m_camera, Position.Center, true));
		m_prefabs.Add(PrefabS.CreatePathPrefabComponentFromVectorArray(m_prefabTC, Vector3.zero, rect, 8f, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true));
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void Destroy()
	{
		base.Destroy();
		m_prefabs.Clear();
		m_prefabTC = null;
	}
}
