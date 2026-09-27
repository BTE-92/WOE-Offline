using System;
using UnityEngine;

public class Keyboard
{
	public TouchScreenKeyboard m_touchKeyboard;

	public string m_originalText;

	public string m_text;

	private int m_maxCharacters;

	public bool m_done;

	public event Action<string> keyboardPressed;

	public event Action<string> keyboardClosed;

	public Keyboard(string _text, int _maxCharacters, bool _numericInput = false, bool _multiline = false, string _fieldName = "")
	{
		m_maxCharacters = _maxCharacters;
		m_text = _text;
		m_originalText = _text;
		m_touchKeyboard = TouchScreenKeyboard.Open(_text, _numericInput ? TouchScreenKeyboardType.NumbersAndPunctuation : TouchScreenKeyboardType.Default, false, _multiline, false, false, _fieldName);
		Open();
	}

	public void Close()
	{
		if (m_touchKeyboard != null)
		{
			m_touchKeyboard.active = false;
		}
		m_done = true;
	}

	public void Open()
	{
		if (m_touchKeyboard != null)
		{
			m_touchKeyboard.active = true;
		}
		m_done = false;
	}

	public void Update()
	{
		if (m_done)
		{
			return;
		}
		string text = m_text;
		if (m_touchKeyboard != null)
		{
			text = m_touchKeyboard.text;
			if (m_touchKeyboard.done)
			{
				m_done = true;
			}
			if (m_touchKeyboard.wasCanceled)
			{
				text = m_originalText;
				m_done = true;
			}
			if (!m_touchKeyboard.active)
			{
				m_done = true;
			}
		}
		if (text.Length > m_maxCharacters)
		{
			text = text.Substring(0, m_maxCharacters);
		}
		if (!m_text.Equals(text))
		{
			m_text = text;
			if (keyboardPressed != null)
			{
				keyboardPressed(text);
			}
		}
		if (m_done && keyboardClosed != null)
		{
			keyboardClosed(text);
		}
	}
}
