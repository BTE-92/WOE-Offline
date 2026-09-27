using FMOD;
using FMOD.Studio;
using UnityEngine;

public static class FMODManager
{
	public static FMOD_StudioSystem m_system;

	public static MixerStrip m_mixer;

	public static void Initialize()
	{
		m_system = FMOD_StudioSystem.instance;
		GUID guid = default(GUID);
		ERRCHECK(m_system.System.lookupID("bus:/", out guid));
		ERRCHECK(m_system.System.getMixerStrip(guid, LOADING_MODE.BEGIN_NOW, out m_mixer));
	}

	public static void PlayOneShot(string _event, Vector3 _position, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
		FMOD.Studio._3D_ATTRIBUTES attributes = _position.to3DAttributes();
		ERRCHECK(eventInstance.set3DAttributes(attributes));
		ERRCHECK(eventInstance.setVolume(_volume));
		ERRCHECK(eventInstance.start());
		ERRCHECK(eventInstance.release());
	}

	public static void PlayOneShotWithParameter(string _event, Vector3 _position, string _parameter, float _value, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
		SetParameter(eventInstance, _parameter, _value);
        FMOD.Studio._3D_ATTRIBUTES attributes = _position.to3DAttributes();
		ERRCHECK(eventInstance.set3DAttributes(attributes));
		ERRCHECK(eventInstance.setVolume(_volume));
		ERRCHECK(eventInstance.start());
		ERRCHECK(eventInstance.release());
	}

	public static EventInstance NewEvent(string _event, Vector3 _position, bool _startImmediately = true, float _volume = 1f)
	{
		EventInstance eventInstance = m_system.GetEvent(_event);
        FMOD.Studio._3D_ATTRIBUTES attributes = _position.to3DAttributes();
		ERRCHECK(eventInstance.set3DAttributes(attributes));
		ERRCHECK(eventInstance.setVolume(_volume));
		if (_startImmediately)
		{
			ERRCHECK(eventInstance.start());
		}
		return eventInstance;
	}

	public static void ReleaseEvent(EventInstance _eventInstance)
	{
		if (_eventInstance != null)
		{
			ERRCHECK(_eventInstance.stop());
			ERRCHECK(_eventInstance.release());
		}
	}

	public static void SetParameter(EventInstance _eventInstance, string _parameterName, float _value)
	{
		ParameterInstance instance = null;
		ERRCHECK(_eventInstance.getParameter(_parameterName, out instance));
		ERRCHECK(instance.setValue(_value));
	}

	public static void MixerSetPause(bool _pause)
	{
		ERRCHECK(m_mixer.setPaused(_pause));
	}

	public static void ERRCHECK(RESULT result)
	{
		if (result != RESULT.OK)
		{
			Debug.LogError("FMOD Error (" + result.ToString() + "): " + Error.String(result));
		}
	}
}
