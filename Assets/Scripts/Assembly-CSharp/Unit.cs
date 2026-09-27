using System.Collections.Generic;
using UnityEngine;

public class Unit : BasicAssembledClass
{
	public UnitC m_unitC;

	public UnitType m_unitType;

	public string m_name;

	public Entity m_entity;

	public bool m_isDead;

	public HitPointType m_hitPointType;

	public float m_hitPoints;

	public float m_maxHitPoints;

	public float m_energy;

	public float m_maxEnergy;

	public float[] m_baseShield;

	public float[] m_currentShield;

	public List<StatModifier> m_shieldModifiers;

	public float[] m_baseArmor;

	public float[] m_currentArmor;

	public List<StatModifier> m_armorModifiers;

	public List<BuffInfo> m_buffs;

	public List<BuffInfo> m_debuffs;

	public List<ContactInfo> m_contacts;

	public float m_contactEndTreshold;

	public ContactState m_contactState;

	public bool m_isSpeeding;

	public Vector2 m_speedingForce;

	public int m_speedingTicks;

	public float m_speedingLimit;

	private ContactInfo ci;

	protected bool shieldModifiersChanged;

	protected bool armorModifiersChanged;

	protected BuffInfo buffInfo;

	protected Buff buff;

	public Unit(GraphElement _graphElement, UnitType _unitType)
		: base(_graphElement)
	{
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_UNIT", _graphElement.m_name });
		m_assembledEntities.Add(m_entity);
		m_unitC = PsS.AddUnit(m_entity, this);
		m_unitType = _unitType;
		m_hitPointType = HitPointType.Health;
		m_hitPoints = 100f;
		m_maxHitPoints = 100f;
		m_energy = 100f;
		m_maxEnergy = 100f;
		m_baseShield = new float[6];
		m_currentShield = new float[6];
		m_baseArmor = new float[6];
		m_currentArmor = new float[6];
		m_shieldModifiers = new List<StatModifier>();
		CalculateCurrentShield();
		m_armorModifiers = new List<StatModifier>();
		SetAllBaseArmours();
		CalculateCurrentArmor();
		m_buffs = new List<BuffInfo>();
		m_debuffs = new List<BuffInfo>();
		m_contacts = new List<ContactInfo>();
		m_contactEndTreshold = 0.1f;
		m_contactState = ContactState.OnAir;
		m_isSpeeding = false;
		m_speedingTicks = 0;
		m_speedingForce = Vector2.zero;
	}

	public virtual void CreateEditorTouchArea(float _width = 100f, float _height = 100f)
	{
		if (PsState.m_gameState == GameState.Edit)
		{
			CreateGraphElementTouchArea(_width, _height);
		}
	}

	public virtual void CreateEditorTouchArea(GameObject _collisionGO)
	{
		if (PsState.m_gameState == GameState.Edit)
		{
			Mesh mesh = (_collisionGO.GetComponent("MeshFilter") as MeshFilter).mesh;
			CreateGraphElementTouchArea(mesh);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public Vector3 GetZBufferBias()
	{
		Random.seed = (int)m_graphElement.m_id;
		return new Vector3(0f, 0f, Random.Range(-0.5f, 0.5f));
	}

	public virtual void SetAllBaseArmours()
	{
		m_baseArmor[1] = -1f;
		m_baseArmor[0] = -1f;
		m_baseArmor[2] = -1f;
	}

	public virtual void SetBaseArmor(DamageType _armorType, float _armorValue)
	{
		m_baseArmor[(int)_armorType] = _armorValue;
		CalculateCurrentArmor();
	}

	public virtual void SetBaseShield(DamageType _shieldType, float _shieldValue)
	{
		m_baseShield[(int)_shieldType] = _shieldValue;
		CalculateCurrentShield();
	}

	public void RemoveAllContacts()
	{
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_unit != null)
			{
				m_contacts[num].m_unit.RemoveUnitContacts(this);
				RemoveUnitContacts(m_contacts[num].m_unit);
			}
		}
	}

	public ContactState GetContactState(ChipmunkBodyC _body)
	{
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody.body == _body.body)
			{
				return ContactState.OnContact;
			}
		}
		return ContactState.OnAir;
	}

	public bool AddGroundContact(ChipmunkBodyC _contactBody, Ground _ground)
	{
		bool result = false;
		ci = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_ground == _ground)
			{
				ci = m_contacts[num];
				break;
			}
		}
		if (ci == null)
		{
			result = true;
			m_contacts.Add(new ContactInfo(_contactBody, _ground));
		}
		else
		{
			ci.m_contactCount++;
			ci.m_endTime = -1f;
			ci.m_end = false;
		}
		return result;
	}

	public void RemoveGroundContact(ChipmunkBodyC _contactBody, Ground _ground)
	{
		ci = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_ground == _ground)
			{
				ci = m_contacts[num];
				break;
			}
		}
		if (ci != null)
		{
			ci.m_contactCount--;
			if (ci.m_contactCount == 0)
			{
				ci.m_end = true;
				ci.m_endTime = Main.m_gameTime;
			}
		}
	}

	public void AddUnitContact(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Unit _unit)
	{
		ci = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_cmb == _cmb && m_contacts[num].m_unit == _unit)
			{
				ci = m_contacts[num];
				break;
			}
		}
		if (ci == null)
		{
			m_contacts.Add(new ContactInfo(_contactBody, _cmb, _unit));
			return;
		}
		ci.m_contactCount++;
		ci.m_endTime = -1f;
		ci.m_end = false;
	}

	public void RemoveUnitContact(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Unit _unit)
	{
		ci = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_cmb == _cmb && m_contacts[num].m_unit == _unit)
			{
				ci = m_contacts[num];
				break;
			}
		}
		if (ci != null)
		{
			ci.m_contactCount--;
			if (ci.m_contactCount == 0)
			{
				ci.m_end = true;
				ci.m_endTime = Main.m_gameTime;
			}
		}
	}

	public void RemoveUnitContacts(Unit _unit)
	{
		ci = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_unit == _unit)
			{
				ci = m_contacts[num];
				break;
			}
		}
		if (ci != null)
		{
			ci.m_contactCount = 0;
			ci.m_end = true;
			ci.m_endTime = Main.m_gameTime;
		}
	}

	public virtual void KillingImpact(ChipmunkBodyC _collidingBody, Vector2 _point, Vector2 _normal, Vector2 _impulse)
	{
	}

	public void SetAsSpeeding(int _ticks, Vector2 _force, float _speedLimit)
	{
		m_isSpeeding = true;
		m_speedingTicks = _ticks;
		m_speedingForce = _force;
		m_speedingLimit = _speedLimit;
	}

	public virtual void Update()
	{
		if (PsState.m_gameState != GameState.Play && PsState.m_gameState != GameState.Test)
		{
			return;
		}
		if (m_isSpeeding)
		{
			Entity entity = m_entity;
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, entity);
			float num = 0f;
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				ChipmunkBodyC chipmunkBodyC = componentsByEntity[i] as ChipmunkBodyC;
				num += ChipmunkProWrapper.ucpBodyGetVel(chipmunkBodyC.body).magnitude;
			}
			num /= (float)componentsByEntity.Count;
			if (num < m_speedingLimit)
			{
				Vector2 speedingForce = m_speedingForce;
				for (int j = 0; j < componentsByEntity.Count; j++)
				{
					ChipmunkBodyC chipmunkBodyC2 = componentsByEntity[j] as ChipmunkBodyC;
					ChipmunkProWrapper.ucpBodyApplyImpulse(chipmunkBodyC2.body, speedingForce * chipmunkBodyC2.m_mass, Vector2.zero);
				}
			}
			m_speedingTicks--;
			if (m_speedingTicks <= 0)
			{
				m_isSpeeding = false;
			}
		}
		shieldModifiersChanged = false;
		for (int num2 = m_shieldModifiers.Count - 1; num2 > -1; num2--)
		{
			if (m_shieldModifiers[num2].m_endTime < Main.m_gameTime)
			{
				m_shieldModifiers.RemoveAt(num2);
				shieldModifiersChanged = true;
			}
		}
		if (shieldModifiersChanged)
		{
			CalculateCurrentShield();
		}
		armorModifiersChanged = false;
		for (int num3 = m_armorModifiers.Count - 1; num3 > -1; num3--)
		{
			if (m_armorModifiers[num3].m_endTime < Main.m_gameTime)
			{
				m_armorModifiers.RemoveAt(num3);
				armorModifiersChanged = true;
			}
		}
		if (armorModifiersChanged)
		{
			CalculateCurrentArmor();
		}
		for (int num4 = m_buffs.Count - 1; num4 > -1; num4--)
		{
			buffInfo = m_buffs[num4];
			buff = m_buffs[num4].m_buff;
			if (buffInfo.m_lastTick < Main.m_gameTime)
			{
				RemoveBuff(buffInfo, true);
			}
			else if (buffInfo.m_nextTick < Main.m_gameTime)
			{
				Heal(buff.m_tickEffect);
				buffInfo.m_nextTick += buff.m_interval;
			}
		}
		for (int num5 = m_debuffs.Count - 1; num5 > -1; num5--)
		{
			buffInfo = m_debuffs[num5];
			buff = m_debuffs[num5].m_buff;
			if (buffInfo.m_lastTick < Main.m_gameTime)
			{
				RemoveBuff(buffInfo, true);
			}
			else if (buffInfo.m_nextTick < Main.m_gameTime)
			{
				Damage(buff.m_tickEffect);
				buffInfo.m_nextTick += buff.m_interval;
			}
		}
		for (int num6 = m_contacts.Count - 1; num6 > -1; num6--)
		{
			ci = m_contacts[num6];
			if (ci.m_began)
			{
				ci.m_began = false;
				if (ci.m_ground != null && ci.m_ground.m_buff != null)
				{
					AddBuff(ci.m_ground.m_buff, ci.m_ground);
				}
				if (ci.m_ground != null)
				{
					GroundContactStart(ci);
				}
				else if (ci.m_unit != null)
				{
					UnitContactStart(ci);
				}
				m_contactState = ContactState.OnContact;
			}
			else if (ci.m_end && ci.m_endTime < Main.m_gameTime - m_contactEndTreshold)
			{
				for (int num7 = m_buffs.Count - 1; num7 > -1; num7--)
				{
					if (m_buffs[num7].m_source == ci.m_ground)
					{
						RemoveBuff(m_buffs[num7], false);
					}
				}
				for (int num8 = m_debuffs.Count - 1; num8 > -1; num8--)
				{
					if (m_debuffs[num8].m_source == ci.m_ground)
					{
						RemoveBuff(m_debuffs[num8], false);
					}
				}
				if (ci.m_ground != null)
				{
					GroundContactEnd(ci);
				}
				else if (ci.m_unit != null)
				{
					UnitContactEnd(ci);
				}
				m_contacts.Remove(ci);
				if (m_contacts.Count == 0)
				{
					m_contactState = ContactState.OnAir;
				}
			}
		}
	}

	public virtual void AddBuff(Buff _buff, IAssembledClass _source)
	{
		if (_buff == null || m_isDead)
		{
			return;
		}
		List<BuffInfo> list = ((!_buff.m_isDebuff) ? m_buffs : m_debuffs);
		BuffInfo buffInfo = null;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].m_buff == _buff)
			{
				buffInfo = list[i];
				break;
			}
		}
		if (buffInfo == null)
		{
			buffInfo = new BuffInfo(_buff, _source, this);
			list.Add(buffInfo);
		}
		else
		{
			buffInfo.m_tries++;
			if (buffInfo.m_stack < buffInfo.m_maxStack)
			{
				buffInfo.m_stack++;
			}
		}
		buffInfo.m_lastTick = Main.m_gameTime + _buff.m_duration;
		if (_buff.m_tickEffect != null)
		{
			buffInfo.m_nextTick = Main.m_gameTime + _buff.m_interval;
		}
		else
		{
			buffInfo.m_nextTick = -1f;
		}
		if (_buff.m_isDebuff)
		{
			Damage(_buff.m_beganEffect, buffInfo.m_stack);
		}
		else
		{
			Heal(_buff.m_beganEffect, buffInfo.m_stack);
		}
		if (_buff.m_shieldModifier != null)
		{
			ModifyShield(_buff.m_shieldModifier);
		}
		if (_buff.m_armorModifier != null)
		{
			ModifyArmor(_buff.m_armorModifier);
		}
	}

	public virtual void RemoveBuff(BuffInfo _buffInfo, bool _applyEndEffect)
	{
		List<BuffInfo> list = ((!_buffInfo.m_buff.m_isDebuff) ? m_buffs : m_debuffs);
		if (_buffInfo.m_buff.m_endEffect != null && _applyEndEffect)
		{
			if (_buffInfo.m_buff.m_isDebuff)
			{
				Damage(_buffInfo.m_buff.m_endEffect, _buffInfo.m_stack);
			}
			else
			{
				Heal(_buffInfo.m_buff.m_endEffect, _buffInfo.m_stack);
			}
		}
		if (_buffInfo.m_buff.m_shieldModifier != null)
		{
			m_shieldModifiers.Remove(_buffInfo.m_buff.m_shieldModifier);
			CalculateCurrentShield();
		}
		if (_buffInfo.m_buff.m_armorModifier != null)
		{
			m_armorModifiers.Remove(_buffInfo.m_buff.m_armorModifier);
			CalculateCurrentArmor();
		}
		if (_buffInfo.m_tries > 1)
		{
			_buffInfo.m_tries--;
		}
		else
		{
			list.Remove(_buffInfo);
		}
	}

	public virtual void ModifyShield(StatModifier _modifier)
	{
		if (_modifier == null || m_isDead)
		{
			return;
		}
		StatModifier statModifier = null;
		for (int i = 0; i < m_shieldModifiers.Count; i++)
		{
			if (m_shieldModifiers[i] == _modifier)
			{
				statModifier = m_shieldModifiers[i];
				break;
			}
		}
		if (statModifier == null)
		{
			statModifier = _modifier;
			statModifier.m_stack = 1;
			m_shieldModifiers.Add(statModifier);
		}
		else if (statModifier.m_stack < statModifier.m_maxStack)
		{
			statModifier.m_stack++;
		}
		statModifier.m_endTime = Main.m_gameTime + statModifier.m_duration;
	}

	public virtual void ModifyArmor(StatModifier _modifier)
	{
		if (_modifier == null)
		{
			return;
		}
		StatModifier statModifier = null;
		for (int i = 0; i < m_armorModifiers.Count; i++)
		{
			if (m_armorModifiers[i] == _modifier)
			{
				statModifier = m_armorModifiers[i];
				break;
			}
		}
		if (statModifier == null)
		{
			statModifier = _modifier;
			statModifier.m_stack = 1;
			m_armorModifiers.Add(statModifier);
		}
		else if (statModifier.m_stack < statModifier.m_maxStack)
		{
			statModifier.m_stack++;
		}
		statModifier.m_endTime = Main.m_gameTime + statModifier.m_duration;
	}

	public virtual void CalculateCurrentShield()
	{
		for (int i = 0; i < m_currentShield.Length; i++)
		{
			m_currentShield[i] = m_baseShield[i];
			if (m_currentShield[i] > -1f)
			{
				for (int j = 0; j < m_shieldModifiers.Count; j++)
				{
					m_currentShield[i] *= m_shieldModifiers[j].m_multipler;
				}
			}
		}
	}

	public virtual void CalculateCurrentArmor()
	{
		for (int i = 0; i < m_currentArmor.Length; i++)
		{
			m_currentArmor[i] = m_baseArmor[i];
			if (m_currentArmor[i] > -1f)
			{
				for (int j = 0; j < m_armorModifiers.Count; j++)
				{
					m_currentArmor[i] *= m_armorModifiers[j].m_multipler;
				}
			}
		}
	}

	public virtual void Heal(Damage _health, int _multipler = 1)
	{
		if (_health != null && !m_isDead)
		{
			float[] amount = _health.m_amount;
			float num = 0f;
			for (int i = 0; i < amount.Length; i++)
			{
				num += amount[i];
			}
			m_hitPoints += num * (float)_multipler;
			if (m_hitPoints > m_maxHitPoints)
			{
				m_hitPoints = m_maxHitPoints;
			}
			Nurture(num * (float)_multipler);
		}
	}

	public virtual void Damage(Damage _damage, int _multipler = 1)
	{
		if (_damage == null || m_isDead)
		{
			return;
		}
		float[] amount = _damage.m_amount;
		int damageType = 0;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < amount.Length; i++)
		{
			if (m_currentArmor[i] == -1f || m_currentShield[i] == -1f || amount[i] == 0f)
			{
				continue;
			}
			float num3 = amount[i] * (float)_multipler;
			num3 -= m_currentShield[i];
			num3 -= m_currentArmor[i];
			if (num3 > 0f)
			{
				num2 += num3;
				if (num3 > num)
				{
					num = num3;
					damageType = i;
				}
			}
		}
		if (num2 > 0f)
		{
			if (m_hitPointType == HitPointType.Lives)
			{
				m_hitPoints -= 1f;
			}
			else
			{
				m_hitPoints -= num2;
			}
			if (m_hitPoints <= 0f)
			{
				Kill((DamageType)damageType, num2);
			}
			else
			{
				Wound((DamageType)damageType, num2);
			}
		}
	}

	public virtual void Wound(DamageType _damageType, float _totalDamage)
	{
		Debug.Log(m_name + " - Damage: " + _totalDamage + ", Hit Points: " + m_hitPoints + "/" + m_maxHitPoints);
	}

	public virtual void Nurture(float _totalHeal)
	{
		Debug.Log(m_name + " - Heal: " + _totalHeal + ", Hit Points: " + m_hitPoints + "/" + m_maxHitPoints);
	}

	public virtual void Kill(DamageType _damageType, float _totalDamage)
	{
		m_isDead = true;
	}

	public virtual void EmergencyKill()
	{
		m_isDead = true;
		Destroy();
	}

	public virtual void GroundContactStart(ContactInfo _contactInfo)
	{
	}

	public virtual void GroundContactEnd(ContactInfo _contactInfo)
	{
	}

	public virtual void UnitContactStart(ContactInfo _contactInfo)
	{
	}

	public virtual void UnitContactEnd(ContactInfo _contactInfo)
	{
	}
}
