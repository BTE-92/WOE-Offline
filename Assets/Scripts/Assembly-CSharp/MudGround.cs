using UnityEngine;

public class MudGround : Ground
{
	public MudGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Mud";
		m_elasticity = 0.1f;
		m_friction = 0.4f;
		m_beltMaterialResourceName = "Ground/MudBeltMat";
		m_frontMaterialResourceName = "Ground/MudFrontMat";
		m_angularDampEffect = 1f;
		m_linearDampEffect = Vector2.one * 0.9f;
		m_destructible = true;
		m_depth = 150f;
		m_zOffset = 10f;
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Mud";
		m_drawSound = "/UI/DrawLoop_Mud";
		m_driveFX = "ParticleFx/MudSplatter";
	}
}
