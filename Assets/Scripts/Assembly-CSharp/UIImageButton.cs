using UnityEngine;

public class UIImageButton : UIComponent
{
	public static float m_defaultRadius = 0.1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ScreenShortest;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	private SpriteSheet m_spriteSheet;

	private TransformC m_spriteTC;

	public UILabel m_label;

	public UIImageButton(UIComponent _parent, string frameName, Camera _camera, float _scale = 1f, float _touchAreaScale = 1.2f, string _text = null, string _tag = "")
		: base(_parent, true, _tag, _camera, null, string.Empty)
	{
		m_spriteSheet = PsState.m_uiSheet;
		Frame frame = m_spriteSheet.m_atlas.GetFrame(frameName);
		float widthRatio = frame.GetWidthRelativeTo(1536f) * _scale * _touchAreaScale * m_spriteSheet.m_globalSpriteScale;
		float heightRatio = frame.GetHeightRelativeTo(1536f) * _scale * _touchAreaScale * m_spriteSheet.m_globalSpriteScale;
		m_spriteTC = TransformS.AddComponent(m_TC.p_entity);
		TransformS.ParentComponent(m_spriteTC, m_TC);
		SpriteS.AddComponent(m_spriteTC, frame, m_spriteSheet);
		SetWidth(widthRatio, m_defaultWidthRelativeTo);
		SetHeight(heightRatio, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		if (_text != null)
		{
			m_label = new UILabel(this, _tag, _text, Align.Center, Align.Center);
		}
	}

	protected void ArrangeSprites()
	{
		TransformS.SetScale(m_spriteTC, (float)Screen.height / 1536f);
	}

	public override void DrawHandler(UIComponent _c)
	{
		ArrangeSprites();
		TweenS.RemoveTweensFromTransformComponent(m_TC);
		if (m_highlight)
		{
			TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1.2f, 1.2f, 1.2f), 0.1f, 0f);
		}
		else
		{
			TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f);
		}
	}
}
