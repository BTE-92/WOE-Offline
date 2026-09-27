using UnityEngine;

public class UIRectSpriteButton : UIFittedSprite
{
	public UIRectSpriteButton(UIComponent _parent, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true)
		: base(_parent, true, _tag, _spriteSheet, _frame, _convertToPrefab)
	{
	}

	public override void DrawHandler(UIComponent _c)
	{
		base.DrawHandler(_c);
		TweenS.RemoveTweensFromTransformComponent(m_spriteTC);
		if (m_highlight)
		{
			TweenS.AddTransformTween(m_spriteTC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(0.8f, 0.8f, 1f), 0.1f, 0f);
		}
		else
		{
			TweenS.AddTransformTween(m_spriteTC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(1f, 1f, 1f), 0.1f, 0f);
		}
	}
}
