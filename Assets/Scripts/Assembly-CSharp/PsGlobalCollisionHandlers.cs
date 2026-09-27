public class PsGlobalCollisionHandlers
{
	public static void Initialize()
	{
		ChipmunkProS.AddGlobalCollisionHandler(UnitToGroundCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)2, true, false, true);
		ChipmunkProS.AddGlobalCollisionHandler(UnitToUnitCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, true);
		ChipmunkProS.AddGlobalCollisionHandler(UnitToGroundCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
		ChipmunkProS.AddGlobalCollisionHandler(UnitToUnitCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
		ChipmunkProS.AddGlobalCollisionHandler(UnitToUnitCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)3, true, false, true);
	}

	public static void UnitToGroundCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexA];
		ChipmunkBodyC chipmunkBodyC2 = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		GroundC groundC = chipmunkBodyC2.customComponent as GroundC;
		if (unitC == null || unitC.m_unit == null || groundC == null || groundC.m_ground == null)
		{
			return;
		}
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
		{
			bool flag = unitC.m_unit.AddGroundContact(chipmunkBodyC, groundC.m_ground);
			Damage damage = new Damage();
			damage.m_amount[0] = _pair.impulse.magnitude * 0.1f / chipmunkBodyC.m_mass;
			bool isDead = unitC.m_unit.m_isDead;
			unitC.m_unit.Damage(damage);
			if (!isDead && unitC.m_unit.m_isDead)
			{
				unitC.m_unit.KillingImpact(chipmunkBodyC, _pair.point, _pair.normal, _pair.impulse);
			}
			if (flag)
			{
				groundC.m_ground.StartedContactWithUnit(unitC, _pair, _phase);
			}
			break;
		}
		case ucpCollisionPhase.Separate:
			unitC.m_unit.RemoveGroundContact(chipmunkBodyC, groundC.m_ground);
			break;
		}
	}

	public static void UnitToUnitCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexA];
		ChipmunkBodyC chipmunkBodyC2 = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		UnitC unitC2 = chipmunkBodyC2.customComponent as UnitC;
		if (unitC == null || unitC.m_unit == null || unitC2 == null || unitC2.m_unit == null)
		{
			return;
		}
		switch (_phase)
		{
		case ucpCollisionPhase.Begin:
		{
			unitC.m_unit.AddUnitContact(chipmunkBodyC, chipmunkBodyC2, unitC2.m_unit);
			unitC2.m_unit.AddUnitContact(chipmunkBodyC2, chipmunkBodyC, unitC.m_unit);
			bool isDead = unitC.m_unit.m_isDead;
			bool isDead2 = unitC2.m_unit.m_isDead;
			if (!ChipmunkProWrapper.ucpBodyIsStatic(chipmunkBodyC.body) && !ChipmunkProWrapper.ucpBodyIsStatic(chipmunkBodyC2.body))
			{
				float num = chipmunkBodyC.m_mass + chipmunkBodyC2.m_mass;
				float num2 = _pair.impulse.magnitude * (chipmunkBodyC2.m_mass / num);
				float num3 = _pair.impulse.magnitude * (chipmunkBodyC.m_mass / num);
				Damage damage = new Damage();
				damage.m_amount[0] = num2 * 0.05f;
				Damage damage2 = new Damage();
				damage2.m_amount[0] = num3 * 0.05f;
				unitC.m_unit.Damage(damage);
				unitC2.m_unit.Damage(damage2);
			}
			else
			{
				Damage damage3 = new Damage();
				damage3.m_amount[0] = _pair.impulse.magnitude * 0.1f / chipmunkBodyC.m_mass;
				Damage damage4 = new Damage();
				damage4.m_amount[0] = _pair.impulse.magnitude * 0.1f / chipmunkBodyC2.m_mass;
				unitC.m_unit.Damage(damage3);
				unitC2.m_unit.Damage(damage4);
			}
			if (!isDead && unitC.m_unit.m_isDead)
			{
				unitC.m_unit.KillingImpact(chipmunkBodyC2, _pair.point, _pair.normal, _pair.impulse);
			}
			if (!isDead2 && unitC2.m_unit.m_isDead)
			{
				unitC2.m_unit.KillingImpact(chipmunkBodyC, _pair.point, -_pair.normal, -_pair.impulse);
			}
			break;
		}
		case ucpCollisionPhase.Separate:
			unitC.m_unit.RemoveUnitContact(chipmunkBodyC, chipmunkBodyC2, unitC2.m_unit);
			unitC2.m_unit.RemoveUnitContact(chipmunkBodyC2, chipmunkBodyC, unitC.m_unit);
			break;
		}
	}
}
