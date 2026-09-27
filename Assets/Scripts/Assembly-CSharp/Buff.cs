public class Buff
{
	public bool m_isDebuff;

	public float m_interval;

	public float m_duration;

	public Damage m_beganEffect;

	public Damage m_tickEffect;

	public Damage m_endEffect;

	public StatModifier m_shieldModifier;

	public StatModifier m_armorModifier;

	public Buff()
	{
		m_isDebuff = true;
		m_interval = 1f;
		m_duration = 10f;
		m_beganEffect = null;
		m_tickEffect = null;
		m_endEffect = null;
		m_shieldModifier = null;
		m_armorModifier = null;
	}
}
