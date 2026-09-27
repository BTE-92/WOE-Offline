using System.Collections.Generic;
using UnityEngine;

public class UIFittedSprite : UIComponent
{
	public SpriteSheet m_spriteSheet;

	public Frame m_frame;

	public SpriteC m_sprite;

	public TransformC m_spriteTC;

	public bool m_adjustWidthToFitSprite;

	public bool m_convertToPrefab;

	public List<PrefabC> m_prefabs;

	public UIFittedSprite(UIComponent _parent, bool _touchable, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true, bool _adjustWidthToFitSprite = true)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetMargins(0f);
		m_spriteSheet = _spriteSheet;
		m_frame = _frame;
		m_convertToPrefab = _convertToPrefab;
		m_adjustWidthToFitSprite = _adjustWidthToFitSprite;
		m_spriteTC = TransformS.AddComponent(m_TC.p_entity, "Sprite");
		TransformS.ParentComponent(m_spriteTC, m_TC, Vector3.zero);
		m_sprite = SpriteS.AddComponent(m_spriteTC, m_frame, m_spriteSheet);
		m_prefabs = new List<PrefabC>();
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		if (m_sprite == null)
		{
			m_sprite = SpriteS.AddComponent(m_spriteTC, m_frame, m_spriteSheet);
		}
		if (!m_convertToPrefab)
		{
			SpriteS.SetSortValue(m_sprite, m_depthOffset);
		}
		float num = (m_actualHeight - m_actualMargins.t - m_actualMargins.b) / m_sprite.height;
		float num2 = (m_actualWidth - m_actualMargins.l - m_actualMargins.r) / m_sprite.width;
		float scale = Mathf.Min(num, num2);
		SpriteS.SetDimensionScale(m_sprite, scale);
		TransformS.SetPosition(m_spriteTC, new Vector2(m_actualMargins.l - m_actualMargins.r, m_actualMargins.b - m_actualMargins.t) * 0.5f);
		float num3 = m_frame.height / m_frame.width;
		if (num < num2)
		{
			SetWidth(m_actualHeight / num3 / (float)Screen.width, RelativeTo.ScreenWidth);
		}
		else
		{
			SetHeight(m_actualWidth * num3 / (float)Screen.height, RelativeTo.ScreenHeight);
		}
		CalculateReferenceSizes();
		UpdateSize();
		UpdateAlign();
		if (m_convertToPrefab)
		{
			while (m_prefabs.Count > 0)
			{
				int index = m_prefabs.Count - 1;
				PrefabS.RemoveComponent(m_prefabs[index]);
				m_prefabs.RemoveAt(index);
			}
			m_prefabs = SpriteS.ConvertSpritesToPrefabComponent(m_spriteTC, m_camera, true);
			m_sprite = null;
		}
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void DrawHandler(UIComponent _c)
	{
	}
}
