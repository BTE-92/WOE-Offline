using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TextS
{
	public static GenericArray<Font> m_fonts;

	public static Hashtable m_styles;

	private static Style m_style;

	public static DynamicArray<TextC> m_components;

	public static int m_componentCount = 800;

	public static void Initialize()
	{
		m_fonts = new GenericArray<Font>(5);
		m_styles = new Hashtable();
		m_components = new DynamicArray<TextC>();
		for (int i = 0; i < 5; i++)
		{
			m_fonts.m_array[i] = new Font();
			m_fonts.m_array[i].characters = new Char[400];
			m_fonts.m_array[i].p_spriteSheet = null;
			m_fonts.m_array[i].name = string.Empty;
		}
	}

	public static Style AddStyle(string _styleName, Font _font)
	{
		Style style = null;
		style = ((!m_styles.Contains(_styleName)) ? new Style() : (m_styles[_styleName] as Style));
		style.name = _styleName;
		style.p_font = _font;
		style.lineHeight = _font.lineHeight;
		style.baseline = _font.baseline;
		style.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		style.xScale = 1f;
		style.yScale = 1f;
		m_styles.Add(_styleName, style);
		return style;
	}

	public static void RemoveStyle(string _styleName)
	{
		m_styles.Remove(_styleName);
	}

	public static void SetStyle(string _styleName)
	{
		m_style = m_styles[_styleName] as Style;
	}

	public static Font AddFont(string _fontName, string _fontFolder, int _maxCharacters, int _textureWidth, int _textureHeight, float _globalScale, Camera _camera)
	{
		Material fontMaterial = Resources.Load(_fontFolder + _fontName + "-material") as Material;
		TextAsset properties = Resources.Load(_fontFolder + _fontName + "-properties") as TextAsset;
		return AddFont(fontMaterial, properties, _maxCharacters, _textureWidth, _textureHeight, _globalScale, _camera);
	}

	public static Font AddFont(Material _fontMaterial, TextAsset _properties, int _maxCharacters, int _textureWidth, int _textureHeight, float _globalScale, Camera _camera)
	{
		int num = m_fonts.AddItem();
		Font font = m_fonts.m_array[num];
		font.name = _fontMaterial.name;
		font.p_spriteSheet = SpriteS.AddSpriteSheet(_camera, _fontMaterial, _globalScale);
		StringReader stringReader = new StringReader(_properties.text);
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			string[] array = text.Split(' ');
			if (array[0] == "common")
			{
				string[] array2 = array[1].Split('=');
				font.lineHeight = Convert.ToInt32(array2[1]);
				string[] array3 = array[2].Split('=');
				font.baseline = Convert.ToInt32(array3[1]);
				string[] array4 = array[3].Split('=');
				font.width = Convert.ToInt32(array4[1]);
				string[] array5 = array[4].Split('=');
				font.height = Convert.ToInt32(array5[1]);
			}
			else
			{
				if (!(array[0] == "char"))
				{
					continue;
				}
				string[] array6 = array[1].Split('=');
				int num2 = Convert.ToInt32(array6[1]);
				if (num2 <= 0)
				{
					continue;
				}
				font.characters[num2] = new Char();
				for (int i = 1; i < array.Length; i++)
				{
					array6 = array[i].Split('=');
					if (array6[0] == "x")
					{
						font.characters[num2].x = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "y")
					{
						font.characters[num2].y = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "width")
					{
						font.characters[num2].width = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "height")
					{
						font.characters[num2].height = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "xoffset")
					{
						font.characters[num2].xOffset = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "yoffset")
					{
						font.characters[num2].yOffset = Convert.ToInt32(array6[1]);
					}
					else if (array6[0] == "xadvance")
					{
						font.characters[num2].xAdvance = Convert.ToInt32(array6[1]);
					}
				}
			}
		}
		Resources.UnloadAsset(_properties);
		_properties = null;
		return font;
	}

	public static Font GetFont(string _fontName, out int _index)
	{
		_index = -1;
		int aliveCount = m_fonts.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			Font font = m_fonts.m_array[m_fonts.m_aliveIndices[i]];
			if (font.name == _fontName)
			{
				_index = i;
				return font;
			}
		}
		return null;
	}

	public static void RemoveFont(string _fontName)
	{
		int _index = -1;
		Font font = GetFont(_fontName, out _index);
		if (_index > -1)
		{
			SpriteS.RemoveSpriteSheet(font.p_spriteSheet);
			m_fonts.RemoveItem(_index);
		}
	}

	public static TextC AddMultilineComponent(TransformC _tc, string _text, float _fontSize, float _textAreaWidth, float _textAreaHeight, Align _horizontalAlign, Align _verticalAlign)
	{
		return AddComponent(_tc, _text, _fontSize, true, true, 0.5f, 0.5f, _textAreaWidth, _textAreaHeight, _horizontalAlign, _verticalAlign, 0f, 0f, 0f, 0f, 0f, 0f);
	}

	public static TextC AddMultilineComponent(TransformC _tc, string _text, float _fontSize, float _textAreaWidth, float _textAreaHeight)
	{
		return AddComponent(_tc, _text, _fontSize, true, true, 0.5f, 0.5f, _textAreaWidth, _textAreaHeight, Align.Left, Align.Top, 0f, 0f, 0f, 0f, 0f, 0f);
	}

	public static TextC AddMultilineComponent(TransformC _tc, string _text, float _fontSize, bool _isDynamic, float _textAreaAlignX, float _textAreaAlignY, float _textAreaWidth, float _textAreaHeight, Align _textHorizontalAlign, Align _textVerticalAlign, float _marginLeft, float _marginRight, float _marginTop, float _marginBottom)
	{
		return AddComponent(_tc, _text, _fontSize, _isDynamic, true, _textAreaAlignX, _textAreaAlignY, _textAreaWidth, _textAreaHeight, _textHorizontalAlign, _textVerticalAlign, _marginLeft, _marginRight, _marginTop, _marginBottom, 0f, 0f);
	}

	public static TextC AddComponent(TransformC _tc, string _text, float _fontSize, bool _isDynamic, bool _isMultiline, float _textAreaAlignX, float _textAreaAlignY, float _textAreaWidth, float _textAreaHeight, Align _textHorizontalAlign, Align _textVerticalAlign, float _marginLeft, float _marginRight, float _marginTop, float _marginBottom, float _offsetX, float _offsetY)
	{
		TextC textC = m_components.AddItem();
		textC.text = _text;
		textC.fontSize = _fontSize;
		textC.textAreaAlignX = _textAreaAlignX;
		textC.textAreaAlignY = _textAreaAlignY;
		textC.textAreaWidth = _textAreaWidth;
		textC.textAreaHeight = _textAreaHeight;
		textC.textHorizontalAlign = _textHorizontalAlign;
		textC.textVerticalAlign = _textVerticalAlign;
		textC.isDynamic = _isDynamic;
		textC.isMultiline = _isMultiline;
		textC.marginLeft = _marginLeft;
		textC.marginRight = _marginRight;
		textC.marginTop = _marginTop;
		textC.marginBottom = _marginBottom;
		textC.update = false;
		textC.TC = _tc;
		textC.textAreaOffsetX = _offsetX;
		textC.textAreaOffsetY = _offsetY;
		textC.contentTC = TransformS.AddComponent(_tc.p_entity);
		TransformS.ParentComponent(textC.contentTC, textC.TC);
		TransformS.SetPosition(textC.contentTC, Vector3.zero);
		textC.textWidth = 0f;
		textC.textHeight = 0f;
		if (!textC.isMultiline)
		{
			CreateSingleLineText(textC);
		}
		else
		{
			CreateText(textC);
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, textC);
		return textC;
	}

	public static TextC AddSingleLineComponent(TransformC _tc, string _text)
	{
		return AddComponent(_tc, _text, 1f, true, false, 0.5f, 0.5f, 0f, 0f, Align.Center, Align.Center, 0f, 0f, 0f, 0f, 0f, 0f);
	}

	public static TextC AddSingleLineComponent(TransformC _tc, string _text, float _fontSize, Align _hAlign, Align _vAlign)
	{
		return AddComponent(_tc, _text, _fontSize, true, false, 0.5f, 0.5f, 0f, 0f, _hAlign, _vAlign, 0f, 0f, 0f, 0f, 0f, 0f);
	}

	public static void RemoveComponent(TextC _t)
	{
		RemoveComponent(_t, false);
	}

	public static void RemoveComponent(TextC _t, bool _clear)
	{
		if (_t.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_t.TC = null;
		if (_t.gameObject != null)
		{
			UnityEngine.Object.Destroy(_t.gameObject);
		}
		if (_clear)
		{
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _t.p_entity);
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				SpriteS.RemoveComponent(componentsByEntity[i] as SpriteC);
			}
		}
		EntityManager.RemoveComponentFromEntity(_t);
		m_components.RemoveItem(_t);
	}

	public static void RemoveComponentsByEntity(Entity _e)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Text, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponent(componentsByEntity[index] as TextC);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void ClearTextComponent(TextC _t)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _t.p_entity);
		for (int i = 0; i < componentsByEntity.Count; i++)
		{
			if ((componentsByEntity[i] as SpriteC).p_TC == _t.contentTC)
			{
				SpriteS.RemoveComponent(componentsByEntity[i] as SpriteC);
			}
		}
	}

	public static void ChangeText(TextC _t, string _text)
	{
		_t.text = _text;
		if (!_t.isMultiline)
		{
			CreateSingleLineText(_t);
		}
		else
		{
			CreateText(_t);
		}
	}

	public static void SetTextAreaOffset(TextC _t, float _offsetX, float _offsetY)
	{
		_t.textAreaOffsetX = _offsetX;
		_t.textAreaOffsetY = _offsetY;
		ClearTextComponent(_t);
		CreateText(_t);
	}

	private static void CreateSingleLineText(TextC _t)
	{
		List<SpriteC> spritesByTransform = SpriteS.GetSpritesByTransform(_t.contentTC);
		if (m_style == null)
		{
			Debug.LogError("set style first");
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int i = 0; i < _t.text.Length; i++)
		{
			int num = _t.text[i];
			if (num < 256)
			{
				Font p_font = m_style.p_font;
				zero2.x += (float)p_font.characters[num].xAdvance * m_style.xScale * _t.fontSize;
			}
		}
		if (_t.textHorizontalAlign == Align.Right)
		{
			zero.x -= zero2.x;
		}
		else if (_t.textHorizontalAlign == Align.Center)
		{
			zero.x -= zero2.x * 0.5f;
		}
		if (_t.textVerticalAlign == Align.Center)
		{
			TransformS.SetPosition(_t.contentTC, Vector3.up * m_style.p_font.lineHeight * m_style.yScale * _t.fontSize * 0.5f);
		}
		else if (_t.textVerticalAlign == Align.Top)
		{
			TransformS.SetPosition(_t.contentTC, Vector3.up * (m_style.p_font.lineHeight - m_style.p_font.baseline) * m_style.yScale * _t.fontSize);
		}
		else if (_t.textVerticalAlign == Align.Bottom)
		{
			TransformS.SetPosition(_t.contentTC, Vector3.up * m_style.p_font.lineHeight * m_style.yScale * _t.fontSize);
		}
		for (int j = 0; j < _t.text.Length; j++)
		{
			int num2 = _t.text[j];
			if (num2 >= 256)
			{
				continue;
			}
			Font p_font2 = m_style.p_font;
			Vector3 zero3 = Vector3.zero;
			zero3.x = (float)p_font2.characters[num2].xOffset * _t.fontSize + (float)p_font2.characters[num2].width * 0.5f;
			zero3.y = (float)(-p_font2.characters[num2].yOffset) * _t.fontSize - (float)p_font2.characters[num2].height * _t.fontSize * 0.5f;
			zero3.x *= m_style.xScale * _t.fontSize;
			Frame frame = new Frame(p_font2.characters[num2].x, p_font2.characters[num2].y, p_font2.characters[num2].width, p_font2.characters[num2].height);
			SpriteC spriteC = null;
			if (spritesByTransform.Count > 0)
			{
				spriteC = spritesByTransform[spritesByTransform.Count - 1];
				while (spritesByTransform.Count > 0 && spriteC.p_TC != _t.contentTC)
				{
					SpriteS.RemoveComponent(spriteC);
					spritesByTransform.RemoveAt(spritesByTransform.Count - 1);
					spriteC = ((spritesByTransform.Count <= 0) ? null : spritesByTransform[spritesByTransform.Count - 1]);
				}
			}
			if (spriteC == null)
			{
				spriteC = SpriteS.AddComponent(_t.contentTC, frame, p_font2.p_spriteSheet);
			}
			else
			{
				spritesByTransform.RemoveAt(spritesByTransform.Count - 1);
				SpriteS.SetFrame(spriteC.p_spriteSheet, spriteC, frame);
				SpriteS.SetVisibility(spriteC, true);
				spriteC.m_active = true;
				spriteC.m_wasActive = true;
			}
			SpriteS.SetDimensions(spriteC, frame.width * m_style.xScale * _t.fontSize, frame.height * m_style.yScale * _t.fontSize);
			SpriteS.SetColor(spriteC, m_style.color);
			SpriteS.SetOffset(spriteC, zero + zero3, 0f);
			zero.x += (float)p_font2.characters[num2].xAdvance * m_style.xScale * _t.fontSize;
		}
		zero.y -= m_style.lineHeight * m_style.yScale * _t.fontSize;
		_t.textWidth = zero2.x;
		_t.textHeight = 0f - zero.y;
		while (spritesByTransform.Count > 0)
		{
			int index = spritesByTransform.Count - 1;
			SpriteS.RemoveComponent(spritesByTransform[index]);
			spritesByTransform.RemoveAt(index);
		}
	}

	private static void CreateText(TextC _t)
	{
		int num = 0;
		List<SpriteC> spritesByTransform = SpriteS.GetSpritesByTransform(_t.contentTC);
		if (m_style == null)
		{
			Debug.LogError("set style first");
		}
		string[] array = _t.text.Split('\n');
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 vector = new Vector3(_t.textAreaOffsetX, _t.textAreaOffsetY, 0f);
		float x = (0f - _t.textAreaWidth) * _t.textAreaAlignX + _t.marginLeft;
		float num2 = _t.textAreaHeight * (1f - _t.textAreaAlignY) - _t.marginTop;
		if (_t.textVerticalAlign == Align.Center)
		{
			float num3 = _t.textAreaHeight - _t.marginTop - _t.marginBottom;
			float num4 = (float)array.Length * m_style.lineHeight * m_style.yScale * _t.fontSize;
			num2 -= (num3 - num4) * 0.5f;
		}
		else if (_t.textVerticalAlign == Align.Bottom)
		{
			float num5 = _t.textAreaHeight - _t.marginTop - _t.marginBottom;
			float num6 = (float)array.Length * m_style.lineHeight * m_style.yScale * _t.fontSize;
			num2 -= num5 - num6;
		}
		zero.y = num2;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num7 = 0;
			bool flag = false;
			while (!flag)
			{
				flag = true;
				zero.x = x;
				zero2.x = _t.marginLeft;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				string text2 = string.Empty;
				float x2 = 0f;
				float num11 = 0f;
				float num12 = 0f;
				float num13 = 0f;
				for (int j = num7; j < text.Length; j++)
				{
					int num14 = text[j];
					if (num14 >= 256)
					{
						continue;
					}
					Font p_font = m_style.p_font;
					if (num14 == 32)
					{
						num9 = num8;
						x2 = zero2.x;
						num10++;
					}
					num12 = num11;
					num11 = zero2.x;
					zero2.x += (float)p_font.characters[num14].xAdvance * m_style.xScale * _t.fontSize;
					if (zero2.x > _t.textAreaWidth - _t.marginRight)
					{
						if (num10 > 0)
						{
							zero2.x = x2;
							text2 = text.Substring(num7, num9);
							num7 += num9 + 1;
							num10--;
						}
						else
						{
							zero2.x = num12;
							text2 = text.Substring(num7, num8 - 1);
							num7 += num8 - 1;
						}
						flag = false;
						break;
					}
					num8++;
				}
				if (flag)
				{
					text2 = text.Substring(num7, num8);
				}
				if (_t.textHorizontalAlign == Align.Right)
				{
					zero.x += _t.textAreaWidth - _t.marginRight - zero2.x;
				}
				else if (_t.textHorizontalAlign == Align.Center)
				{
					zero.x += (_t.textAreaWidth - _t.marginRight - zero2.x) * 0.5f;
				}
				else if (_t.textHorizontalAlign == Align.Justified && !flag)
				{
					num13 = ((num10 != 0) ? ((_t.textAreaWidth - _t.marginRight - zero2.x) / (float)num10) : ((_t.textAreaWidth - _t.marginRight - zero2.x) / (float)(num8 - 1)));
				}
				string text3 = text2;
				foreach (int num15 in text3)
				{
					if (num15 >= 256)
					{
						continue;
					}
					Font p_font2 = m_style.p_font;
					Vector3 zero3 = Vector3.zero;
					zero3.x = (float)p_font2.characters[num15].xOffset * _t.fontSize + (float)p_font2.characters[num15].width * 0.5f;
					zero3.y = (float)(-p_font2.characters[num15].yOffset) * _t.fontSize - (float)p_font2.characters[num15].height * _t.fontSize * 0.5f;
					zero3.x *= m_style.xScale * _t.fontSize;
					Frame frame = new Frame(p_font2.characters[num15].x, p_font2.characters[num15].y, p_font2.characters[num15].width, p_font2.characters[num15].height);
					SpriteC spriteC = null;
					if (spritesByTransform.Count > 0)
					{
						spriteC = spritesByTransform[spritesByTransform.Count - 1];
						while (spritesByTransform.Count > 0 && spriteC.p_TC != _t.contentTC)
						{
							SpriteS.RemoveComponent(spriteC);
							spritesByTransform.RemoveAt(spritesByTransform.Count - 1);
							spriteC = ((spritesByTransform.Count <= 0) ? null : spritesByTransform[spritesByTransform.Count - 1]);
						}
					}
					if (spriteC == null)
					{
						spriteC = SpriteS.AddComponent(_t.contentTC, frame, p_font2.p_spriteSheet);
					}
					else
					{
						spritesByTransform.RemoveAt(spritesByTransform.Count - 1);
						SpriteS.SetFrame(spriteC.p_spriteSheet, spriteC, frame);
						SpriteS.SetVisibility(spriteC, true);
						spriteC.m_active = true;
						spriteC.m_wasActive = true;
						num++;
					}
					SpriteS.SetDimensions(spriteC, frame.width * m_style.xScale * _t.fontSize, frame.height * m_style.yScale * _t.fontSize);
					SpriteS.SetColor(spriteC, m_style.color);
					SpriteS.SetOffset(spriteC, zero + zero3 + vector, 0f);
					zero.x += (float)p_font2.characters[num15].xAdvance * m_style.xScale * _t.fontSize;
					if (num10 == 0)
					{
						zero.x += num13;
					}
					else if (num15 == 32)
					{
						zero.x += num13;
					}
				}
				zero.y -= m_style.lineHeight * m_style.yScale * _t.fontSize;
				_t.textWidth = zero.x;
				_t.textHeight = 0f - zero.y;
			}
		}
		while (spritesByTransform.Count > 0)
		{
			int index = spritesByTransform.Count - 1;
			SpriteS.RemoveComponent(spritesByTransform[index]);
			spritesByTransform.RemoveAt(index);
		}
	}

	public static void Update()
	{
	}
}
