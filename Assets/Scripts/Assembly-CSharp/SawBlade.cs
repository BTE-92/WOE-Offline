using System;
using UnityEngine;

public class SawBlade : Unit
{
	// The visual mesh (pCylinder8) has a radius of about 262; the original collision circle was only 10, so the
	// saw could be driven straight through and never hurt anything.
	private const float RADIUS = 250f;

	private ChipmunkBodyC m_cmb;

	private float m_angle;

	public SawBlade(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/SawBladePrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f) + GetZBufferBias(), gameObject);
		ucpCircleShape shape = new ucpCircleShape(RADIUS, Vector2.zero, 1f, 0.25f, 0.8f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddRogueBody(transformC, shape, m_unitC);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, false);
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, false);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase != ucpCollisionPhase.Begin)
		{
			return;
		}
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC == null || unitC.m_unit == null || unitC.m_unit == this || unitC.m_unit.m_isDead)
		{
			return;
		}
		if (unitC.m_unit.m_currentArmor[1] != -1f)
		{
			unitC.m_unit.Damage(new Damage(DamageType.Weapon, float.MaxValue));
		}
	}

	public override void Update()
	{
		base.Update();
		if ((PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play) && Player.m_gameIsStarted)
		{
			m_angle = ToolBox.getCappedAngle(m_angle - 0.3f);
			ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, (m_graphElement.m_rotation.z + m_angle) * ((float)Math.PI / 180f));
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
