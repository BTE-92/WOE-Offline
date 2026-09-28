using FMOD.Studio;

public class SoundC : BasicComponent
{
	public static int m_componentCount;

	public TransformC p_TC;

	public bool isPlaying;

	public bool isPaused;

	public bool forceAtListenerPosition;

	public EventInstance eventInstance;

	public SoundC()
		: base(ComponentType.Sound)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		isPlaying = false;
		isPaused = false;
		forceAtListenerPosition = false;
		eventInstance = default(EventInstance);
    }

	~SoundC()
	{
		m_componentCount--;
	}
}
