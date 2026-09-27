using System;
using System.Collections.Generic;
using UnityEngine;

public static class TweenS
{
	public static DynamicArray<TweenC> m_components;

	private static List<TweenC> m_removeList;

	public static void Initialize()
	{
		m_components = new DynamicArray<TweenC>();
		m_removeList = new List<TweenC>();
	}

	public static void AddTweenEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		if (_tweenComponent.delegatedCount == 0)
		{
			_tweenComponent.tweenEventDelegate = _tweenEventHandler;
		}
		else
		{
			_tweenComponent.tweenEventDelegate = (TweenEventDelegate)Delegate.Combine(_tweenComponent.tweenEventDelegate, _tweenEventHandler);
		}
		_tweenComponent.delegatedCount++;
	}

	public static void RemoveTweenEventListener(TweenC _tweenComponent, TweenEventDelegate _tweenEventHandler)
	{
		if (_tweenComponent.delegatedCount > 0)
		{
			_tweenComponent.tweenEventDelegate = (TweenEventDelegate)Delegate.Remove(_tweenComponent.tweenEventDelegate, _tweenEventHandler);
			_tweenComponent.delegatedCount--;
		}
		if (_tweenComponent.delegatedCount == 0)
		{
			_tweenComponent.tweenEventDelegate = (TweenEventDelegate)Delegate.Combine(_tweenComponent.tweenEventDelegate, new TweenEventDelegate(TempDelegate));
		}
	}

	public static void TempDelegate(TweenC _c)
	{
	}

	public static TweenC AddTween(TweenStyle _style, float _startValue, float _endValue, float _duration, float _delay)
	{
		return AddTransformTween(null, TweenedProperty.None, _style, Vector3.one * _startValue, Vector3.one * _endValue, _duration, _delay);
	}

	public static TweenC AddTween(TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay)
	{
		return AddTransformTween(null, TweenedProperty.Position, _style, _startValue, _endValue, _duration, _delay);
	}

	public static void ReInitialize(TweenC _c, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay)
	{
		_c.m_active = true;
		_c.currentTweenStyle = _style;
		_c.startValue = _startValue;
		_c.endValue = _endValue;
		_c.currentValue = _startValue;
		_c.duration = _duration;
		_c.delay = _delay;
		_c.startTime = Main.m_gameTime + _delay;
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _endValue, float _duration, float _delay)
	{
		return AddTransformTween(_tc, _component, _style, _endValue, _duration, _delay, false);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _endValue, float _duration, float _delay, bool _globalTarget)
	{
		Vector3 startValue = Vector3.zero;
		switch (_component)
		{
		case TweenedProperty.Position:
			startValue = _tc.transform.localPosition;
			break;
		case TweenedProperty.Rotation:
			startValue = ((!_tc.forceRotation) ? _tc.transform.localRotation.eulerAngles : _tc.forcedRotation.eulerAngles);
			break;
		case TweenedProperty.Scale:
			startValue = ((!_tc.forceScale) ? _tc.transform.localScale : _tc.forcedScale);
			break;
		}
		return AddTransformTween(_tc, _component, _style, startValue, _endValue, _duration, _delay, _globalTarget);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay)
	{
		return AddTransformTween(_tc, _component, _style, _startValue, _endValue, _duration, _delay, false);
	}

	public static TweenC AddTransformTween(TransformC _tc, TweenedProperty _component, TweenStyle _style, Vector3 _startValue, Vector3 _endValue, float _duration, float _delay, bool _globalTarget)
	{
		TweenC tweenC = m_components.AddItem();
		tweenC.p_TC = _tc;
		tweenC.tweenedProperty = _component;
		tweenC.currentTweenStyle = _style;
		tweenC.startValue = _startValue;
		if (_globalTarget)
		{
			_endValue = _tc.transform.position + (_endValue - _tc.transform.position);
		}
		tweenC.endValue = _endValue;
		tweenC.currentValue = _startValue;
		tweenC.duration = _duration;
		tweenC.delay = _delay;
		tweenC.startTime = Main.m_gameTime + _delay;
		if (_tc != null)
		{
			EntityManager.AddComponentToEntity(_tc.p_entity, tweenC);
		}
		else
		{
			tweenC.m_active = true;
			tweenC.m_wasActive = true;
		}
		return tweenC;
	}

	public static void RemoveComponent(TweenC _c)
	{
		if (_c.tweenEventDelegate != null)
		{
			Delegate[] invocationList = _c.tweenEventDelegate.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				_c.tweenEventDelegate = (TweenEventDelegate)Delegate.Remove(_c.tweenEventDelegate, (TweenEventDelegate)obj);
			}
			_c.delegatedCount = 0;
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void RemoveTweensFromTransformComponent(TransformC _tc)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Tween, _tc.p_entity);
		while (componentsByEntity.Count > 0)
		{
			RemoveComponent(componentsByEntity[0] as TweenC);
			componentsByEntity.RemoveAt(0);
		}
	}

	public static void SetAdditionalTweenProperties(TweenC _c, int _repeatCount, bool _mirror, TweenStyle _mirroredTweenStyle)
	{
		_c.repeats = _repeatCount;
		_c.mirrored = _mirror;
		_c.mirroredTweenStyle = _mirroredTweenStyle;
		if (_mirror && _c.repeats != -1)
		{
			_c.repeats = _c.repeats * 2 + 1;
		}
	}

	public static void SetRemoveEntityAtFinish(TweenC _c, bool _remove)
	{
		_c.removeEntityAtFinish = _remove;
	}

	public static void Update()
	{
		int aliveCount = m_components.m_aliveCount;
		for (int num = aliveCount - 1; num > -1; num--)
		{
			TweenC tweenC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (tweenC.m_active)
			{
				float num2 = Mathf.Max(Main.m_gameTime - tweenC.startTime, 0f);
				if (num2 >= tweenC.duration)
				{
					if (tweenC.currentRepeat < tweenC.repeats || tweenC.repeats == -1)
					{
						num2 -= tweenC.duration;
						tweenC.startTime += tweenC.duration;
						tweenC.currentRepeat++;
						if (tweenC.mirrored)
						{
							Vector3 startValue = tweenC.startValue;
							tweenC.startValue = tweenC.endValue;
							tweenC.endValue = startValue;
							TweenStyle currentTweenStyle = tweenC.currentTweenStyle;
							tweenC.currentTweenStyle = tweenC.mirroredTweenStyle;
							tweenC.mirroredTweenStyle = currentTweenStyle;
						}
					}
					else
					{
						tweenC.m_active = false;
						tweenC.currentValue = tweenC.endValue;
						tweenC.hasFinished = true;
						if (tweenC.tweenEventDelegate != null && tweenC.delegatedCount > 0)
						{
							tweenC.tweenEventDelegate(tweenC);
						}
					}
				}
				if (tweenC.p_TC == null)
				{
					if (tweenC.m_active)
					{
						tweenC.currentValue.x = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.x, tweenC.endValue.x - tweenC.startValue.x);
						if (tweenC.tweenedProperty != TweenedProperty.None)
						{
							tweenC.currentValue.y = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.y, tweenC.endValue.y - tweenC.startValue.y);
							tweenC.currentValue.z = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.z, tweenC.endValue.z - tweenC.startValue.z);
						}
					}
					else if (tweenC.removeComponentAtFinish)
					{
						m_removeList.Add(tweenC);
					}
				}
				else
				{
					TransformC p_TC = tweenC.p_TC;
					if (tweenC.tweenedProperty == TweenedProperty.Position)
					{
						if (tweenC.m_active)
						{
							tweenC.currentValue.x = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.x, tweenC.endValue.x - tweenC.startValue.x);
							tweenC.currentValue.y = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.y, tweenC.endValue.y - tweenC.startValue.y);
							tweenC.currentValue.z = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.z, tweenC.endValue.z - tweenC.startValue.z);
							p_TC.transform.localPosition = tweenC.currentValue;
						}
						else
						{
							p_TC.transform.localPosition = tweenC.endValue;
							tweenC.currentValue = tweenC.endValue;
							if (tweenC.removeComponentAtFinish)
							{
								m_removeList.Add(tweenC);
							}
						}
						p_TC.updatePosition = true;
					}
					else if (tweenC.tweenedProperty == TweenedProperty.Rotation)
					{
						if (tweenC.m_active)
						{
							tweenC.currentValue.x = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.x, tweenC.endValue.x - tweenC.startValue.x);
							tweenC.currentValue.y = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.y, tweenC.endValue.y - tweenC.startValue.y);
							tweenC.currentValue.z = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.z, tweenC.endValue.z - tweenC.startValue.z);
							if (p_TC.forceRotation)
							{
								p_TC.forcedRotation = Quaternion.Euler(tweenC.currentValue);
							}
							else
							{
								p_TC.transform.localRotation = Quaternion.Euler(tweenC.currentValue);
							}
						}
						else
						{
							if (p_TC.forceRotation)
							{
								p_TC.forcedRotation = Quaternion.Euler(tweenC.endValue);
							}
							else
							{
								p_TC.transform.localRotation = Quaternion.Euler(tweenC.endValue);
							}
							tweenC.currentValue = tweenC.endValue;
							if (tweenC.removeComponentAtFinish)
							{
								m_removeList.Add(tweenC);
							}
						}
						p_TC.updateRotation = true;
					}
					else if (tweenC.tweenedProperty == TweenedProperty.Scale)
					{
						if (tweenC.m_active)
						{
							tweenC.currentValue.x = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.x, tweenC.endValue.x - tweenC.startValue.x);
							tweenC.currentValue.y = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.y, tweenC.endValue.y - tweenC.startValue.y);
							tweenC.currentValue.z = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.z, tweenC.endValue.z - tweenC.startValue.z);
							if (p_TC.forceScale)
							{
								p_TC.forcedScale = tweenC.currentValue;
							}
							else
							{
								p_TC.transform.localScale = tweenC.currentValue;
							}
						}
						else
						{
							if (p_TC.forceScale)
							{
								p_TC.forcedScale = tweenC.endValue;
							}
							else
							{
								p_TC.transform.localScale = tweenC.endValue;
							}
							tweenC.currentValue = tweenC.endValue;
							if (tweenC.removeComponentAtFinish)
							{
								m_removeList.Add(tweenC);
							}
						}
						p_TC.updateScale = true;
						p_TC.updateRotation = true;
					}
					else if (tweenC.tweenedProperty == TweenedProperty.Alpha)
					{
						if (tweenC.m_active)
						{
							tweenC.currentValue.x = tween(tweenC.currentTweenStyle, num2, tweenC.duration, tweenC.startValue.x, tweenC.endValue.x - tweenC.startValue.x);
						}
						else
						{
							tweenC.currentValue = tweenC.endValue;
							if (tweenC.removeComponentAtFinish)
							{
								m_removeList.Add(tweenC);
							}
						}
						SpriteS.SetAlphaByTransformComponent(tweenC.p_TC, tweenC.currentValue.x);
					}
				}
			}
		}
		while (m_removeList.Count > 0)
		{
			int index = m_removeList.Count - 1;
			TweenC tweenC2 = m_removeList[index];
			if (tweenC2.removeEntityAtFinish)
			{
				EntityManager.RemoveEntity(tweenC2.p_entity);
			}
			else
			{
				RemoveComponent(tweenC2);
			}
			m_removeList.RemoveAt(index);
		}
	}

	public static float tween(TweenStyle _style, float _currentTime, float _duration, float _start, float _difference)
	{
		switch (_style)
		{
		case TweenStyle.Linear:
			return linear(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadIn:
			return easeInQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadOut:
			return easeOutQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.QuadInOut:
			return easeInOutQuad(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicIn:
			return easeInCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicOut:
			return easeOutCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.CubicInOut:
			return easeInOutCubic(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoIn:
			return easeInExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoOut:
			return easeOutExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.ExpoInOut:
			return easeOutExpo(_currentTime, _start, _duration, _difference);
		case TweenStyle.BackOut:
			return easeOutBack(_currentTime, _start, _duration, _difference);
		case TweenStyle.ElasticOut:
			return easeOutElastic(_currentTime, _start, _duration, _difference);
		case TweenStyle.BounceOut:
			return easeOutBounce(_currentTime, _start, _duration, _difference);
		default:
			return 0f;
		}
	}

	public static float easeInQuad(float t, float b, float d, float c)
	{
		return c * (t /= d) * t + b;
	}

	public static float easeOutQuad(float t, float b, float d, float c)
	{
		return (0f - c) * (t /= d) * (t - 2f) + b;
	}

	public static float easeInOutQuad(float t, float b, float d, float c)
	{
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * t * t + b;
		}
		return (0f - c) / 2f * ((t -= 1f) * (t - 2f) - 1f) + b;
	}

	public static float easeInCubic(float t, float b, float d, float c)
	{
		return c * (t /= d) * t * t + b;
	}

	public static float easeOutCubic(float t, float b, float d, float c)
	{
		return c * ((t = t / d - 1f) * t * t + 1f) + b;
	}

	public static float easeInOutCubic(float t, float b, float d, float c)
	{
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * t * t * t + b;
		}
		return c / 2f * ((t -= 2f) * t * t + 2f) + b;
	}

	public static float easeInExpo(float t, float b, float d, float c)
	{
		return (t != 0f) ? (c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b - c * 0.001f) : b;
	}

	public static float easeOutExpo(float t, float b, float d, float c)
	{
		return (t != d) ? (c * 1.001f * (0f - Mathf.Pow(2f, -10f * t / d) + 1f) + b) : (b + c);
	}

	public static float easeInOutExpo(float t, float b, float d, float c)
	{
		if (t == 0f)
		{
			return b;
		}
		if (t == d)
		{
			return b + c;
		}
		if ((t /= d / 2f) < 1f)
		{
			return c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b - c * 0.0005f;
		}
		return c / 2f * 1.0005f * (0f - Mathf.Pow(2f, -10f * (t -= 1f)) + 2f) + b;
	}

	public static float easeOutBack(float t, float b, float d, float c)
	{
		float num = 1.70158f;
		return c * ((t = t / d - 1f) * t * ((num + 1f) * t + num) + 1f) + b;
	}

	public static float easeOutElastic(float t, float b, float d, float c)
	{
		float num = (float)Math.PI * 2f;
		float num2 = 0.5f;
		float num3 = 0.5f;
		if (t == 0f)
		{
			return b;
		}
		if ((t /= d) == 1f)
		{
			return b + c;
		}
		if (num2 != 0f)
		{
			num2 = d * 0.3f;
		}
		float num4;
		if (num3 != 0f || (c > 0f && num3 < c) || (c < 0f && num3 < 0f - c))
		{
			num3 = c;
			num4 = num2 / 4f;
		}
		else
		{
			num4 = num2 / num * Mathf.Asin(c / num3);
		}
		return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - num4) * num / num2) + c + b;
	}

	public static float easeOutBounce(float t, float b, float d, float c)
	{
		if ((t /= d) < 0.36363637f)
		{
			return c * (7.5625f * t * t) + b;
		}
		if (t < 0.72727275f)
		{
			return c * (7.5625f * (t -= 0.54545456f) * t + 0.75f) + b;
		}
		if (t < 0.90909094f)
		{
			return c * (7.5625f * (t -= 0.8181818f) * t + 0.9375f) + b;
		}
		return c * (7.5625f * (t -= 21f / 22f) * t + 63f / 64f) + b;
	}

	public static float linear(float t, float b, float d, float c)
	{
		return c * t / d + b;
	}
}
