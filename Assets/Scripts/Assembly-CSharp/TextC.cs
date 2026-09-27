using UnityEngine;

public class TextC : BasicComponent
{
	public static int m_componentCount;

	public string text;

	public float fontSize;

	public bool update;

	public bool isDynamic;

	public bool isMultiline;

	public float textWidth;

	public float textHeight;

	public float textAreaWidth;

	public float textAreaHeight;

	public float textAreaOffsetX;

	public float textAreaOffsetY;

	public Align textVerticalAlign;

	public Align textHorizontalAlign;

	public float textAreaAlignX;

	public float textAreaAlignY;

	public float marginLeft;

	public float marginRight;

	public float marginTop;

	public float marginBottom;

	public GameObject gameObject;

	public TransformC TC;

	public TransformC contentTC;

	public TextC()
		: base(ComponentType.Text)
	{
		textAreaAlignX = 0f;
		textAreaAlignY = 1f;
		textAreaHeight = 100f;
		textAreaWidth = 200f;
		textHorizontalAlign = Align.Left;
		textVerticalAlign = Align.Top;
		m_componentCount++;
	}

	~TextC()
	{
		m_componentCount--;
	}
}
