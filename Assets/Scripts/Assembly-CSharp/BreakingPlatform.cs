using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : Unit
{
	private const float CRUMBLE_TIME_SECS = 0.8f;

	private ChipmunkBodyC m_cmb;

	private float m_crumbleTimer;

	private bool m_startedCrumbling;

	private TransformC m_mainTC;

	private Entity m_crumbleFxEntity;

	private GameObject m_platformMesh;

	private int m_moveDir = 1;

	public BreakingPlatform(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/BreakingPlatformMediumPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		m_mainTC = transformC;
		GameObject gameObject2 = gameObject.transform.Find("CrumblingPlatformMedium").gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		m_platformMesh = prefabC.p_gameObject;
		ucpPolyShape shape = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("Collision1").gameObject, 1f, 0.25f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, shape, m_unitC);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, CollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, false, true);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_crumbleTimer = 0.8f;
		m_startedCrumbling = false;
		m_graphElement.m_isRotateable = true;
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin && !m_startedCrumbling)
		{
			m_startedCrumbling = true;
			m_crumbleFxEntity = EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/CrumblingMaterialCountdown"), m_mainTC.transform.position, m_mainTC.transform.rotation.eulerAngles, 5.8f, "GTAG_INGAME_PARTICLES");
		}
	}

	public override void Update()
	{
		if (m_startedCrumbling)
		{
			if (Main.m_gameTicks % 3 == 0)
			{
				m_moveDir *= -1;
			}
			m_platformMesh.transform.localPosition = m_platformMesh.transform.localPosition + new Vector3(m_moveDir * 2, 0f, 0f);
			m_crumbleTimer -= Main.m_gameDeltaTime;
			if (m_crumbleTimer <= 0f)
			{
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Prefab, m_crumbleFxEntity);
				(componentsByEntity[0] as PrefabC).p_gameObject.particleSystem.Stop(true);
				EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/CrumblingMaterialBreakdown"), m_mainTC.transform.position, m_mainTC.transform.rotation.eulerAngles, 5f, "GTAG_INGAME_PARTICLES");
				Kill(DamageType.Impact, float.MaxValue);
			}
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
