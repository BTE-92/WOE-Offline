using UnityEngine;

public struct CVertex
{
	public Vector3 vert;

	public Vector2 uv;

	public Color color;

	public Vector3 normal;

	public bool isDuplicate;

	public CVertex(Vector3 _vert, Vector2 _uv, Color _color, Vector3 _normal, bool _isDuplicate = false)
	{
		vert = _vert;
		uv = _uv;
		color = _color;
		isDuplicate = _isDuplicate;
		normal = _normal;
	}
}
