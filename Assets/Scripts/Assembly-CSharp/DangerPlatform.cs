using UnityEngine;

public class DangerPlatform : Unit
{
	private ChipmunkBodyC m_cmb;

	public DangerPlatform(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/DangerPlatformPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("DangerPlatform").gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision1").gameObject, 1f, 0.25f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, shape, m_unitC);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, false, true);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin)
		{
			ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
			UnitC unitC = chipmunkBodyC.customComponent as UnitC;
			if (unitC.m_unit.m_currentArmor[2] != -1f)
			{
				Damage damage = new Damage(DamageType.Electric, float.MaxValue);
				unitC.m_unit.Damage(damage);
				EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/ElectricBurst"), new Vector3(_pair.point.x, _pair.point.y, 0f), Vector3.zero, 3f, "GTAG_INGAME_PARTICLES");
				SoundS.PlaySingleShot("/Ingame/ElectricZap", new Vector3(_pair.point.x, _pair.point.y, 0f));
			}
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
