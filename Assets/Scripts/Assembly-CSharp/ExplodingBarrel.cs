using UnityEngine;

public class ExplodingBarrel : Unit
{
	private ChipmunkBodyC m_cmb;

	public ExplodingBarrel(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/ExplodingBarrelPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f), gameObject.transform.Find("ExplosiveBarrel").gameObject);
		ucpPolyShape[] shapes = ChipmunkProS.GeneratePolyShapesFromChildren(gameObject.transform.Find("CollisionBarrel").gameObject, 5f, 0.25f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(transformC, shapes);
		m_cmb.customComponent = m_unitC;
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		CreateEditorTouchArea(50f, 50f);
		m_graphElement.m_isRotateable = true;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Impact, 35f);
		SetBaseArmor(DamageType.Electric, 10f);
	}

	public override void EmergencyKill()
	{
		Kill(DamageType.Impact, float.MaxValue);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		Vector2 blastCenter = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		PsS.ApplyBlastWave(blastCenter, 2000f, 1000f, 150f, 25f);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/ExplodingBarrelExplosion"), new Vector3(blastCenter.x, blastCenter.y, 0f), Vector3.zero, 2f, "GTAG_INGAME_PARTICLES");
		SoundS.PlaySingleShot("/Ingame/Units/BarrelExplosion", new Vector3(blastCenter.x, blastCenter.y, 0f));
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
