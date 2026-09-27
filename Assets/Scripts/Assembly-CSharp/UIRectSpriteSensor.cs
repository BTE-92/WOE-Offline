using UnityEngine;

public class UIRectSpriteSensor : UIFittedSprite
{
	public UIRectSpriteSensor(UIComponent _parent, string _tag, SpriteSheet _spriteSheet, Frame _frame, bool _convertToPrefab = true)
		: base(_parent, true, _tag, _spriteSheet, _frame, _convertToPrefab)
	{
		m_TAC.m_allowSecondary = true;
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

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		m_began = true;
		Highlight(true);
	}

	protected override void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		m_end = true;
		Highlight(false);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
	}
}
