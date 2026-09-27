public class MetalGround : Ground
{
	public MetalGround(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_name = "Metal";
		m_elasticity = 0.8f;
		m_friction = 0.7f;
		m_rectBrush = true;
		m_marchHard = false;
		m_smoothingAngle = 0f;
		m_beltMaterialResourceName = "Ground/MetalBeltMat";
		m_frontMaterialResourceName = "Ground/MetalFrontMat";
		m_tireRollSound = "/InGame/Vehicles/TireRollLoop_Metal";
		m_drawSound = "/UI/DrawLoop_Metal";
	}
}
