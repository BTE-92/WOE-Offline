using System;
using UnityEngine;

[Serializable]
public class Vertex3
{
	public float x;

	public float y;

	public float z;

	public Vertex3()
	{
	}

	public Vertex3(Vector2 _v2)
	{
		x = _v2.x;
		y = _v2.y;
		z = 0f;
	}

	public Vertex3(Vector3 _v3)
	{
		x = _v3.x;
		y = _v3.y;
		z = _v3.z;
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);
	}
}
