#if !(UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL)
#define KEYBOARD_NATIVE
#endif

using System;
using UnityEngine;

public class Keyboard
{
    public static Keyboard s_activeKeyboard;

    // Time-based grace period so behaviour is identical at 30 and 60 fps
    // (works around the Unity 2018 early-close bug on Android).
    private const float NativeGraceSeconds = 0.3f;

#if KEYBOARD_NATIVE
    public TouchScreenKeyboard m_touchKeyboard;
    private float m_openTime;
#endif

    public string m_originalText;
    public string m_text;
    public bool m_done;

    // True if the keyboard closed via cancel (Escape / Android back / Canceled status).
    // Read this inside keyboardClosed to decide whether to commit or discard.
    public bool m_cancelled;

    private int m_maxCharacters;
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
        // Hand focus over: close whatever is active first. The loop guards against
        // the old field's close handler opening yet another keyboard.
        int guard = 0;
        while (s_activeKeyboard != null && s_activeKeyboard != this && guard++ < 8)
        {
            s_activeKeyboard.Finish(false);
        }

        s_activeKeyboard = this;
        m_done = false;
        m_cancelled = false;

#if KEYBOARD_NATIVE
        m_openTime = Time.realtimeSinceStartup;
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

    /// <summary>Close and commit the current text.</summary>
    public void Close()
    {
        Finish(false);
    }

    /// <summary>
    /// Call this when the owning UI component is destroyed. Drops all listeners
    /// and releases focus WITHOUT firing events into dead UI.
    /// </summary>
    public void Destroy()
    {
        keyboardPressed = null;
        keyboardClosed = null;
        Finish(true);
    }

    public static void CloseActive()
    {
        if (s_activeKeyboard != null)
        {
            s_activeKeyboard.Finish(false);
        }
    }

    // Single exit point. Safe to call repeatedly; keyboardClosed fires at most once.
    private void Finish(bool cancelled)
    {
        if (m_done) return;

        m_done = true;
        m_cancelled = cancelled;
        if (cancelled)
        {
            m_text = m_originalText;
        }

#if KEYBOARD_NATIVE
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

        Action<string> handler = keyboardClosed;
        if (handler != null)
        {
            // A broken listener must not block focus handover to the next field.
            try { handler(m_text); }
            catch (Exception e) { Debug.LogError(e); }
        }
    }

    private void RaisePressed()
    {
        Action<string> handler = keyboardPressed;
        if (handler != null)
        {
            handler(m_text);
        }
    }

    public void Update()
    {
        // Only the focused keyboard processes input
        if (m_done || s_activeKeyboard != this)
        {
            return;
        }

#if KEYBOARD_NATIVE
        UpdateNative();
#else
        HandleHardwareKeyboardInput();
#endif
    }

#if KEYBOARD_NATIVE
    private string Sanitize(string s)
    {
        if (s == null) return string.Empty;

        if (s.Length > m_maxCharacters)
        {
            s = s.Substring(0, m_maxCharacters);
        }
        return s;
    }

    private void UpdateNative()
    {
        // Open() can return null if the platform refuses; don't hold focus forever.
        if (m_touchKeyboard == null)
        {
            Finish(false);
            return;
        }

        // Back button / Canceled: close but KEEP what the user typed.
        bool keepLastText = false;
        bool finished = false;

        switch (m_touchKeyboard.status)
        {
            case TouchScreenKeyboard.Status.Done:
            case TouchScreenKeyboard.Status.LostFocus:
                finished = true;
                break;
            case TouchScreenKeyboard.Status.Canceled:
                finished = true;
                keepLastText = true;
                break;
            default:
                if (Time.realtimeSinceStartup - m_openTime > NativeGraceSeconds && !m_touchKeyboard.active)
                {
                    finished = true;
                }
                break;
        }

        string newText;
        if (keepLastText)
        {
            // Don't trust the native text after a cancel; use the last polled value.
            newText = m_text;
        }
        else
        {
            string raw = m_touchKeyboard.text;
            newText = Sanitize(raw);

            // Push the corrected text back so the native field matches m_text
            if (!finished && raw != null && newText != raw)
            {
                m_touchKeyboard.text = newText;
            }
        }

        if (newText != m_text)
        {
            m_text = newText;
            RaisePressed();
        }

        if (finished)
        {
            Finish(false);
        }
    }
#else
    private void HandleHardwareKeyboardInput()
    {
        string before = m_text;
        bool finish = false;

        string input = Input.inputString;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '\b') // Backspace
            {
                if (m_text.Length > 0)
                {
                    m_text = m_text.Substring(0, m_text.Length - 1);
                }
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                if (m_multiline)
                {
                    if (m_text.Length < m_maxCharacters)
                    {
                        m_text += "\n";
                    }
                }
                else
                {
                    finish = true;
                    break; // ignore anything typed after Enter this frame
                }
            }
            else if (c == (char)27) // Escape: close, keep text
            {
                finish = true;
                break;
            }
            else if (c >= 32) // Printable
            {
                if (m_text.Length < m_maxCharacters)
                {
                    m_text += c;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)
            || (!m_multiline && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))))
        {
            finish = true;
        }

        if (m_text != before)
        {
            RaisePressed();
        }

        if (finish)
        {
            Finish(false);
        }
    }
#endif
}