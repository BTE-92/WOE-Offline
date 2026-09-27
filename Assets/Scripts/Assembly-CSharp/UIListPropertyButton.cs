public class UIListPropertyButton : UIVerticalListButton
{
	protected string[] m_valueLabels;

	protected string[] m_shortValueLabels;

	protected Frame[] m_valueFrames;

	protected SpriteSheet m_spriteSheet;

	protected SpriteSheet m_selectorSheet;

	protected UIIconLabel m_iconLabel;

	public UIVerticalList m_selectionPage;

	public UIPropertyLabel m_label;

	public UIPropertyValueLabel m_value;

	public UIListPropertyButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, string _label, UIModel _model, string _INTfieldName, string[] _valueLabels, SpriteSheet _spriteSheet, Frame[] _valueframes)
		: base(_parent, _thisPage, _scrollingCanvas, _tag, _model, _INTfieldName)
	{
		m_valueLabels = _valueLabels;
		m_spriteSheet = _spriteSheet;
		m_valueFrames = _valueframes;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_label = new UIPropertyLabel(this, _tag + "Label", _label);
		int num = (int)GetValue();
		Frame frame = m_valueFrames[num];
		m_iconLabel = new UIIconLabel(this, _tag + "Icon", _spriteSheet, frame, Align.Right, Align.Center);
	}

	public UIListPropertyButton(UIComponent _parent, UIVerticalList _thisPage, UIPagedCanvas _scrollingCanvas, string _tag, string _label, UIModel _model, string _INTfieldName, string[] _valueLabels, string[] _shortValueLabels)
		: base(_parent, _thisPage, _scrollingCanvas, _tag, _model, _INTfieldName)
	{
		m_valueLabels = _valueLabels;
		m_shortValueLabels = _shortValueLabels;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_label = new UIPropertyLabel(this, _tag, _label);
		int num = (int)GetValue();
		string text = "null";
		if (num < m_shortValueLabels.Length)
		{
			text = m_shortValueLabels[num];
		}
		m_value = new UIPropertyValueLabel(this, _tag, text);
	}

	public override void OnValueChange(object _value)
	{
		int num = (int)_value;
		if (m_valueFrames != null)
		{
			SpriteS.SetFrame(m_iconLabel.m_sprite.p_spriteSheet, m_iconLabel.m_sprite, m_valueFrames[num]);
			return;
		}
		string text = "null";
		if (num < m_shortValueLabels.Length)
		{
			text = m_shortValueLabels[num];
		}
		m_value.m_text = text;
		if (m_value.d_Draw != null)
		{
			m_value.d_Draw(m_value);
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (m_pagedCanvas.IsChangingPage())
		{
			return;
		}
		base.OnTouchRelease(_touch, _inside);
		if (!_inside)
		{
			return;
		}
		m_selectionPage = new UIVerticalList(m_thisPage.m_parent, "VerticalArea");
		m_selectionPage.SetVerticalAlign(1f);
		if (m_valueFrames != null)
		{
			if (m_selectorSheet == null)
			{
				m_selectorSheet = SpriteS.AddSpriteSheet(m_selectionPage.m_camera, m_spriteSheet.m_material, 1f);
			}
			for (int i = 0; i < m_valueLabels.Length; i++)
			{
				new UIListSelectorButton(m_selectionPage, m_selectionPage, m_pagedCanvas, "ListSelectorButton", m_model, m_fieldName, m_valueLabels[i], m_selectorSheet, m_valueFrames[i], i);
			}
		}
		else
		{
			for (int j = 0; j < m_valueLabels.Length; j++)
			{
				new UIListSelectorButton(m_selectionPage, m_selectionPage, m_pagedCanvas, "ListSelectorButton", m_model, m_fieldName, m_valueLabels[j], m_shortValueLabels[j], j);
			}
		}
		m_pagedCanvas.NextPage();
		m_pagedCanvas.Update();
		m_pagedCanvas.DisableTouchAreas();
	}

	public override void Destroy()
	{
		if (m_selectorSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_selectorSheet);
			m_selectorSheet = null;
		}
		base.Destroy();
	}
}
