using System;
using UnityEngine;

public class Keyboard
{
    public static Keyboard s_activeKeyboard;

#if !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
	public TouchScreenKeyboard m_touchKeyboard;
	private int m_framesActive;
#endif

    public string m_originalText;
    public string m_text;
    private int m_maxCharacters;
    public bool m_done;

    private bool m_numericInput;
    private bool m_multiline;
    private string m_fieldName;

    public event Action<string> keyboardPressed;
    public event Action<string> keyboardClosed;

    public Keyboard(string _text, int _maxCharacters, bool _numericInput = false, bool _multiline = false, string _fieldName = "")
    {
        m_maxCharacters = _maxCharacters;
        m_text = _text ?? string.Empty;
        m_originalText = m_text;
        m_numericInput = _numericInput;
        m_multiline = _multiline;
        m_fieldName = _fieldName;

        Open();
    }

    public void Open()
    {
        // If another field is currently open, close it first
        if (s_activeKeyboard != null && s_activeKeyboard != this)
        {
            s_activeKeyboard.Close();
        }

        // Claim exclusive focus
        s_activeKeyboard = this;
        m_done = false;

#if !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
		m_framesActive = 0;
		m_touchKeyboard = TouchScreenKeyboard.Open(
			m_text,
			m_numericInput ? TouchScreenKeyboardType.NumbersAndPunctuation : TouchScreenKeyboardType.Default,
			false,
			m_multiline,
			false,
			false,
			m_fieldName
		);
#endif
    }

    public void Close()
    {
#if !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
		if (m_touchKeyboard != null)
		{
			m_touchKeyboard.active = false;
			m_touchKeyboard = null;
		}
#endif
        if (s_activeKeyboard == this)
        {
            s_activeKeyboard = null;
        }

        if (!m_done)
        {
            m_done = true;
            if (keyboardClosed != null)
            {
                keyboardClosed(m_text);
            }
        }
    }

    public void Update()
    {
        // Only the currently focused keyboard can process updates
        if (m_done || s_activeKeyboard != this)
        {
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // Desktop / Editor: Process hardware keyboard input
        HandleHardwareKeyboardInput();
#else
		// Mobile Device: Process TouchScreenKeyboard input
		if (m_touchKeyboard != null)
		{
			m_framesActive++;
			string currentKeyboardText = m_touchKeyboard.text;

			if (m_touchKeyboard.status == TouchScreenKeyboard.Status.Done)
			{
				m_done = true;
			}
			else if (m_touchKeyboard.status == TouchScreenKeyboard.Status.Canceled)
			{
				currentKeyboardText = m_originalText;
				m_done = true;
			}
			else if (m_touchKeyboard.status == TouchScreenKeyboard.Status.LostFocus)
			{
				m_done = true;
			}
			// Buffer 5 frames to prevent the Unity 2018 early-close bug on Android
			else if (m_framesActive > 5 && !m_touchKeyboard.active)
			{
				m_done = true;
			}

			if (currentKeyboardText != null && currentKeyboardText.Length > m_maxCharacters)
			{
				currentKeyboardText = currentKeyboardText.Substring(0, m_maxCharacters);
			}

			if (currentKeyboardText != null && !m_text.Equals(currentKeyboardText))
			{
				m_text = currentKeyboardText;
				if (keyboardPressed != null)
				{
					keyboardPressed(m_text);
				}
			}

			if (m_done)
			{
				if (s_activeKeyboard == this)
				{
					s_activeKeyboard = null;
				}

				if (keyboardClosed != null)
				{
					keyboardClosed(m_text);
				}
			}
		}
#endif
    }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
    private void HandleHardwareKeyboardInput()
    {
        bool textChanged = false;

        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (m_text.Length > 0)
                {
                    m_text = m_text.Substring(0, m_text.Length - 1);
                    textChanged = true;
                }
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                if (m_multiline)
                {
                    if (m_text.Length < m_maxCharacters)
                    {
                        m_text += "\n";
                        textChanged = true;
                    }
                }
                else
                {
                    m_done = true;
                }
            }
            else if (c == (char)27) // Escape
            {
                m_text = m_originalText;
                m_done = true;
            }
            else if (c >= 32) // Printable character
            {
                if (m_numericInput)
                {
                    if (char.IsDigit(c) || c == '.' || c == '-' || c == ',')
                    {
                        if (m_text.Length < m_maxCharacters)
                        {
                            m_text += c;
                            textChanged = true;
                        }
                    }
                }
                else
                {
                    if (m_text.Length < m_maxCharacters)
                    {
                        m_text += c;
                        textChanged = true;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_text = m_originalText;
            m_done = true;
        }
        else if (!m_multiline && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            m_done = true;
        }

        if (textChanged && keyboardPressed != null)
        {
            keyboardPressed(m_text);
        }

        if (m_done)
        {
            if (s_activeKeyboard == this)
            {
                s_activeKeyboard = null;
            }

            if (keyboardClosed != null)
            {
                keyboardClosed(m_text);
            }
        }
    }
#endif

    public static void CloseActive()
    {
        if (s_activeKeyboard != null)
        {
            s_activeKeyboard.Close();
        }
    }
}