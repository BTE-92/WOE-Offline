using UnityEngine;

public class Ground : BasicAssembledClass
{
	public string m_name;

	public float m_elasticity;

	public float m_friction;

	public Vector2 m_surfaceVelocity;

	public float m_smoothingAngle;

	public float m_depth;

	public float m_zOffset;

	public string m_beltMaterialResourceName;

	public string m_frontMaterialResourceName;

	public float m_density;

	public float m_angularDamp;

	public Vector2 m_linearDamp;

	public float m_angularDampEffect;

	public Vector2 m_linearDampEffect;

	public Buff m_buff;

	public float m_buffInterval;

	public string m_tireRollSound;

	public string m_drawSound;

	public string m_driveFX;

	public bool m_rectBrush;

	public bool m_marchHard;

	public bool m_destructible;

	public Ground(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Name me!";
		m_elasticity = 0.5f;
		m_friction = 0.5f;
		m_surfaceVelocity = Vector2.zero;
		m_smoothingAngle = 90f;
		m_depth = 300f;
		m_zOffset = -10f;
		m_beltMaterialResourceName = string.Empty;
		m_frontMaterialResourceName = string.Empty;
		m_density = 1f;
		m_angularDamp = 1f;
		m_linearDamp = Vector2.one;
		m_angularDampEffect = 1f;
		m_linearDampEffect = Vector2.one;
		m_buff = null;
		m_buffInterval = 1f;
		m_tireRollSound = null;
		m_drawSound = null;
		m_driveFX = null;
		m_rectBrush = false;
		m_marchHard = false;
		m_destructible = false;
	}

	public virtual void Update()
	{
	}

	public virtual void StartedContactWithUnit(UnitC _unitC, ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
	}
}
