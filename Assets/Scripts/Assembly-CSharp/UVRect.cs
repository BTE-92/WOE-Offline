public class UVRect
{
	public float left;

	public float bottom;

	public float width;

	public float height;

	public UVRect(float _left, float _bottom, float _width, float _height)
	{
		left = _left;
		bottom = _bottom;
		width = _width;
		height = _height;
	}

	public static UVRect Normal()
	{
		return new UVRect(0f, 0f, 1f, 1f);
	}
}
