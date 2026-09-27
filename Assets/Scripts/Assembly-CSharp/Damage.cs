using System;

[Serializable]
public class Damage
{
	public float[] m_amount;

	public Damage m_procDamage;

	public float m_procDamageChance;

	public Buff m_procBuff;

	public float m_procBuffChance;

	public Damage()
	{
		m_amount = new float[6];
		m_amount[0] = 0f;
		m_amount[1] = 0f;
		m_amount[2] = 0f;
		m_procDamage = null;
		m_procDamageChance = 0f;
		m_procBuff = null;
		m_procBuffChance = 0f;
	}

	public Damage(DamageType _damageType, float _damageAmount)
	{
		m_amount = new float[6];
		m_amount[1] = 0f;
		m_amount[0] = 0f;
		m_amount[2] = 0f;
		m_amount[(int)_damageType] = _damageAmount;
		m_procDamage = null;
		m_procDamageChance = 0f;
		m_procBuff = null;
		m_procBuffChance = 0f;
	}

	public void SetDamage(DamageType _damageType, int _amount)
	{
		m_amount[(int)_damageType] = _amount;
	}

	public void SetProcDamage(Damage _damage, float _chance)
	{
		m_procDamage = _damage;
		m_procDamageChance = _chance;
	}

	public void SetProcBuff(Buff _buff, float _chance)
	{
		m_procBuff = _buff;
		m_procBuffChance = _chance;
	}
}
