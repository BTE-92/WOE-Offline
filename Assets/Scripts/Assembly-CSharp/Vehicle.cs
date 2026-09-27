using UnityEngine;

public class Vehicle : Unit
{
	public Shield m_shield;

	public Armor m_armor;

	public Weapon m_weapon;

	private ChipmunkBodyC m_chassisBody;

	public ChipmunkConstraintC m_rearMotor;

	public ChipmunkConstraintC m_frontMotor;

	private Vector2 m_frontMotorForces;

	private Vector2 m_rearMotorForces;

	private float m_frontMaxRate;

	private float m_rearMaxRate;

	public Vehicle(GraphElement _graphElement)
		: base(_graphElement, UnitType.Vehicle)
	{
	}

	public virtual void InitMotors(ChipmunkBodyC _chassis, ChipmunkBodyC _rearWheel, ChipmunkBodyC _frontWheel)
	{
		m_chassisBody = _chassis;
		TransformC anchor = TransformS.AddComponent(m_entity, _rearWheel.TC.transform.position);
		m_rearMotor = ChipmunkProS.AddSimpleMotor(_chassis, _rearWheel, anchor, 0f, 0f);
		ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, 0f);
		anchor = TransformS.AddComponent(m_entity, _frontWheel.TC.transform.position);
		m_frontMotor = ChipmunkProS.AddSimpleMotor(_chassis, _frontWheel, anchor, 0f, 0f);
		ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, 0f);
		m_frontMotorForces = new Vector2(750000f, 1250000f);
		m_rearMotorForces = new Vector2(750000f, 1250000f);
		m_frontMaxRate = 50f;
		m_rearMaxRate = 50f;
	}

	public virtual void SetMotorParameters(Vector2 _frontForces, float _frontRate, Vector2 _rearForces, float _rearRate)
	{
		m_frontMotorForces = _frontForces;
		m_rearMotorForces = _rearForces;
		m_frontMaxRate = _frontRate;
		m_rearMaxRate = _rearRate;
	}

	public virtual void UpdateMotors(float _gasAmount)
	{
		float f = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
		Vector2 lhs = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
		float num = Vector2.Dot(lhs, vector.normalized);
		if (_gasAmount != 0f)
		{
			bool flag = false;
			bool flag2 = _gasAmount > 0f;
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, (!flag2) ? m_rearMotorForces.y : m_rearMotorForces.x);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, (!flag2) ? m_frontMotorForces.y : m_frontMotorForces.x);
			if (flag && vector.magnitude > 50f)
			{
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
			}
			else
			{
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, m_rearMaxRate * _gasAmount);
				ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, m_frontMaxRate * _gasAmount);
			}
		}
		else
		{
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, 0f);
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, 0f);
		}
	}

	public virtual void SetHandBrake(bool _brake, float _force = 1000000f)
	{
		if (!m_isDead)
		{
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_rearMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_rearMotor.constraint, (!_brake) ? 0f : _force);
			ChipmunkProWrapper.ucpSimpleMotorSetRate(m_frontMotor.constraint, 0f);
			ChipmunkProWrapper.ucpConstraintSetMaxForce(m_frontMotor.constraint, (!_brake) ? 0f : _force);
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		UpdateMotors(0f);
	}
}
