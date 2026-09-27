using UnityEngine;

public class UIText : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.ParentHeight;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	public TextMeshC m_tmc;

	public string m_text;

	public float m_fontSize;

	public RelativeTo m_fontSizeRelativeTo;

	public float m_fontSizeRelative;

	public bool m_adjustWidthToTextWidth;

	public UIText(UIComponent _parent, bool _touchable, string _tag, string _text, string _fontResourcePath, float _fontSize, RelativeTo _fontSizeRelativeTo)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins.l, m_defaultMargins.r, m_defaultMargins.t, m_defaultMargins.b, m_defaultMarginsRelativeTo);
		SetAlign(0.5f, 0.5f);
		m_text = _text;
		m_fontSize = _fontSize;
		m_fontSizeRelativeTo = _fontSizeRelativeTo;
		m_tmc = TextMeshS.AddComponent(m_TC, Vector3.zero, _fontResourcePath, 0f, 0f, _fontSize, Align.Center, Align.Middle, m_camera, _tag + "Text");
	}

	public void SetText(string _text)
	{
		m_text = _text;
		TextMeshS.SetText(m_tmc, _text);
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		int fontSize = Mathf.FloorToInt(m_fontSize * m_fontSizeRelative);
		m_tmc.m_textMesh.fontSize = fontSize;
		Vector2 textSize = TextMeshS.GetTextSize(m_tmc, m_text);
		SetWidth((textSize.x + m_actualMargins.l + m_actualMargins.r) / (float)Screen.width, RelativeTo.ScreenWidth);
		SetHeight((textSize.y + m_actualMargins.t + m_actualMargins.b) / (float)Screen.height, RelativeTo.ScreenHeight);
		CalculateReferenceSizes();
		UpdateSize();
		TextMeshS.SetText(m_tmc, m_text);
		float x = (m_actualMargins.l - m_actualMargins.r) * 0.5f;
		float y = (m_actualMargins.t - m_actualMargins.b) * -0.5f;
		m_tmc.m_go.transform.Translate(new Vector3(x, y));
		UpdateAlign();
		UpdateMargins();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void CalculateReferenceSizes()
	{
		base.CalculateReferenceSizes();
		m_fontSizeRelative = Screen.height;
		if (m_fontSizeRelativeTo == RelativeTo.OwnHeight)
		{
			m_fontSizeRelative = m_actualHeight;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.OwnWidth)
		{
			m_fontSizeRelative = m_actualWidth;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentHeight)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = m_parent.m_actualHeight;
			}
			else
			{
				m_fontSizeRelative = Screen.height;
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentWidth)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = m_parent.m_actualWidth;
			}
			else
			{
				m_fontSizeRelative = Screen.width;
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentLongest)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = Mathf.Max(m_parent.m_actualHeight, m_parent.m_actualWidth);
			}
			else
			{
				m_fontSizeRelative = Mathf.Max(Screen.height, Screen.width);
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ParentShortest)
		{
			if (m_parent != null)
			{
				m_fontSizeRelative = Mathf.Min(m_parent.m_actualHeight, m_parent.m_actualWidth);
			}
			else
			{
				m_fontSizeRelative = Mathf.Min(Screen.height, Screen.width);
			}
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenHeight)
		{
			m_fontSizeRelative = Screen.height;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenWidth)
		{
			m_fontSizeRelative = Screen.width;
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenLongest)
		{
			m_fontSizeRelative = Mathf.Max(Screen.height, Screen.width);
		}
		else if (m_fontSizeRelativeTo == RelativeTo.ScreenShortest)
		{
			m_fontSizeRelative = Mathf.Min(Screen.height, Screen.width);
		}
	}

	public override void DrawHandler(UIComponent _c)
	{
	}
}
