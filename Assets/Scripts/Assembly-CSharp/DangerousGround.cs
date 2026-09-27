using UnityEngine;

public class DangerousGround : Ground
{
	public DangerousGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Dangerous";
		m_elasticity = 0.1f;
		m_friction = 0.8f;
		m_rectBrush = true;
		m_beltMaterialResourceName = "Ground/DangerBeltMat";
		m_frontMaterialResourceName = "Ground/DangerFrontMat";
		m_buff = new Buff();
		m_buff.m_beganEffect = new Damage(DamageType.Electric, float.MaxValue);
		m_buff.m_interval = 1f;
		m_drawSound = "/UI/DrawLoop_Danger";
	}

	public override void StartedContactWithUnit(UnitC _unitC, ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_unitC.m_unit.m_currentArmor[2] != -1f)
		{
			EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/ElectricBurst"), new Vector3(_pair.point.x, _pair.point.y, 0f), Vector3.zero, 3f, "GTAG_INGAME_PARTICLES");
			SoundS.PlaySingleShot("/Ingame/ElectricZap", new Vector3(_pair.point.x, _pair.point.y, 0f));
		}
	}
}
