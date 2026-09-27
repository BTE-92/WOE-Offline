public class IceGround : Ground
{
	public IceGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Ice";
		m_elasticity = 0.8f;
		m_friction = 0.3f;
		m_beltMaterialResourceName = "Ground/IceBeltMat";
		m_frontMaterialResourceName = "Ground/IceFrontMat";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Ice";
		m_drawSound = "/UI/DrawLoop_Ice";
		m_driveFX = "ParticleFx/DrivingIce";
	}
}
