using System;
using System.Collections.Generic;
using System.Reflection;

public class UIModel
{
	private object m_object;

	private Type m_objectType;

	private Dictionary<string, FieldInfo> m_objectFields;

	private Dictionary<string, List<UIComponent>> m_boundComponents;

	public UIModel(object _object)
	{
		m_object = _object;
		m_objectType = _object.GetType();
		m_objectFields = new Dictionary<string, FieldInfo>();
		m_boundComponents = new Dictionary<string, List<UIComponent>>();
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
		FieldInfo[] fields = m_objectType.GetFields(bindingAttr);
		foreach (FieldInfo fieldInfo in fields)
		{
			m_objectFields.Add(fieldInfo.Name, fieldInfo);
			m_boundComponents.Add(fieldInfo.Name, new List<UIComponent>());
		}
	}

	public FieldInfo GetFieldInfo(string _fieldName)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_fieldName, out value))
		{
			return value;
		}
		return null;
	}

	public object GetValue(UIComponent _uiComponent)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value2);
			if (!value2.Contains(_uiComponent))
			{
				value2.Add(_uiComponent);
			}
			return value.GetValue(m_object);
		}
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		return null;
	}

	public object GetValue(UIComponent _uiComponent, out Type _fieldType)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value2);
			if (!value2.Contains(_uiComponent))
			{
				value2.Add(_uiComponent);
			}
			_fieldType = value.FieldType;
			return value.GetValue(m_object);
		}
		_fieldType = null;
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		return null;
	}

	public void SetValue(object _value, UIComponent _uiComponent)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			value.SetValue(m_object, _value);
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value2);
			for (int i = 0; i < value2.Count; i++)
			{
				UIComponent uIComponent = value2[i];
				if (uIComponent != _uiComponent)
				{
					uIComponent.OnValueChange(_value);
				}
			}
		}
		else
		{
			Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		}
	}

	public void RemoveBinding(UIComponent _uiComponent)
	{
		List<UIComponent> value;
		m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value);
		value.Remove(_uiComponent);
	}
}
