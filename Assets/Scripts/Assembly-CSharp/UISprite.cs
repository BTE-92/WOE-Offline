using UnityEngine;

public class UISprite : UIComponent
{
	public SpriteSheet m_spriteSheet;

	public Frame m_frame;

	public SpriteC m_sprite;

	public TransformC m_spriteTC;

	public bool m_adjustWidthToFitSprite;

	public bool m_convertToPrefab;

	public UISprite(UIComponent _parent, bool _touchable, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		m_spriteSheet = _spriteSheet;
		m_frame = _frame;
		m_convertToPrefab = _convertToPrefab;
		m_spriteTC = TransformS.AddComponent(m_TC.p_entity, "Sprite");
		TransformS.ParentComponent(m_spriteTC, m_TC, Vector3.zero);
		m_sprite = SpriteS.AddComponent(m_spriteTC, m_frame, m_spriteSheet);
		SetWidth(m_sprite.width / (float)Screen.width, RelativeTo.ScreenWidth);
		SetHeight(m_sprite.height / (float)Screen.height, RelativeTo.ScreenHeight);
		SetMargins(0f);
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		if (m_sprite == null)
		{
			m_sprite = SpriteS.AddComponent(m_spriteTC, m_frame, m_spriteSheet);
			SetWidth(m_sprite.width / (float)Screen.width, RelativeTo.ScreenWidth);
			SetHeight(m_sprite.height / (float)Screen.height, RelativeTo.ScreenHeight);
			SetMargins(0f);
			UpdateSize();
			UpdateAlign();
		}
		d_Draw(this);
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void DrawHandler(UIComponent _c)
	{
		if (m_convertToPrefab)
		{
			PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
			SpriteS.ConvertSpritesToPrefabComponent(m_TC, m_camera, true);
			m_sprite = null;
		}
	}
}
