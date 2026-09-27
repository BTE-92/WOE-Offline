using System.Collections;
using UnityEngine;

public class Controller
{
	private struct ControllerButton
	{
		public ControllerButtonType type;

		public UIComponent component;

		public Vector2 dir;

		public KeyCode keyCode;
	}

	public bool m_open;

	private Hashtable m_buttons = new Hashtable();

	public void AddButton(string _name, UIComponent _component, ControllerButtonType _type, KeyCode _keyCode = KeyCode.None)
	{
		ControllerButton controllerButton = new ControllerButton
		{
			component = _component,
			type = _type,
			dir = Vector2.zero,
			keyCode = _keyCode
		};
		m_buttons.Add(_name, controllerButton);
	}

	public void RemoveButton(string _name)
	{
		m_buttons.Remove(_name);
	}

	public void RemoveAllButtons()
	{
		m_buttons.Clear();
	}

	public ControllerButtonState GetButtonState(string _name)
	{
		ControllerButton controllerButton = (ControllerButton)m_buttons[_name];
		if (controllerButton.component.m_isDown || Input.GetKey(controllerButton.keyCode))
		{
			return ControllerButtonState.ON;
		}
		return ControllerButtonState.OFF;
	}

	public Vector2 GetControllerJoystickDir(string _name)
	{
		ControllerButton controllerButton = (ControllerButton)m_buttons[_name];
		if (controllerButton.type == ControllerButtonType.JOYSTICK)
		{
			return controllerButton.dir;
		}
		return Vector2.zero;
	}

	public virtual void Open()
	{
	}

	public virtual void Close()
	{
	}
}
