public class GpcC : BasicComponent
{
	public TransformC p_TC;

	public Polygon originalPolygon;

	public Polygon modifiedPolygon;

	public Polygon[] tiles;

	public float polyMinX;

	public float polyMaxX;

	public float polyMinY;

	public float polyMaxY;

	public float polyWidth;

	public float polyHeight;

	public int tileWidth;

	public int tileHeight;

	public int tileCountX;

	public int tileCountY;

	public GpcC()
		: base(ComponentType.Gpc)
	{
	}
}
