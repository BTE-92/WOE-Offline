public class MetalCrate : Crate
{
	private const float SIZE = 30f;

	private const float FRICTION = 0.6f;

	private const float ELASTICITY = 0.2f;

	private const float WEIGHTMULT = 0.5f;

	private const string RESOURCE = "Units/MetalCratePrefab";

	public MetalCrate(GraphElement _graphElement)
		: base(_graphElement, 30f, 0.6f, 0.2f, 0.5f, "Units/MetalCratePrefab")
	{
	}
}
