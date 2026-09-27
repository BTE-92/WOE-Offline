using UnityEngine;

public class SandGround : Ground
{
	public SandGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Sand";
		m_elasticity = 0.5f;
		m_friction = 0.8f;
		m_beltMaterialResourceName = "Ground/SandBeltMat";
		m_frontMaterialResourceName = "Ground/SandFrontMat";
		m_angularDampEffect = 1f;
		m_linearDampEffect = Vector2.one;
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Sand";
		m_drawSound = "/UI/DrawLoop_Sand";
		m_driveFX = "ParticleFx/DrivingSand";
	}
}
