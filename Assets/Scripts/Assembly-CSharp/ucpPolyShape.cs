using UnityEngine;

public class ucpPolyShape : ucpShape
{
	public Vector2[] verts;

	public int numVerts;

	public float width;

	public float height;

	public ucpPolyShape(Vector2[] _verts, Vector2 _offset, float _mass = 10f, float _elasticity = 0.5f, float _friction = 0.5f, ucpCollisionType _collisionType = ucpCollisionType.None, bool _sensor = false)
		: base(ucpShapeType.Poly, _offset, _mass, _elasticity, _friction, _collisionType, _sensor)
	{
		verts = _verts;
		numVerts = _verts.Length;
		float num = 999999f;
		float num2 = -999999f;
		float num3 = -999999f;
		float num4 = 999999f;
		for (int i = 0; i < numVerts; i++)
		{
			Vector2 vector = verts[i];
			if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.x <= num)
			{
				num = vector.x;
			}
			if (vector.y > num3)
			{
				num3 = vector.y;
			}
			if (vector.y <= num4)
			{
				num4 = vector.y;
			}
		}
		width = num2 - num;
		height = num3 - num4;
		area = ChipmunkProWrapper.ucpAreaForPoly(numVerts, verts);
	}

	public ucpPolyShape(float _width, float _height, Vector2 _offset, float _mass = 10f, float _elasticity = 0.5f, float _friction = 0.5f, ucpCollisionType _collisionType = ucpCollisionType.None, bool _sensor = false)
		: base(ucpShapeType.Poly, _offset, _mass, _elasticity, _friction, _collisionType, _sensor)
	{
		verts = new Vector2[4];
		verts[0].x = _width * -0.5f;
		verts[0].y = _height * 0.5f;
		verts[1].x = _width * 0.5f;
		verts[1].y = _height * 0.5f;
		verts[2].x = _width * 0.5f;
		verts[2].y = _height * -0.5f;
		verts[3].x = _width * -0.5f;
		verts[3].y = _height * -0.5f;
		numVerts = 4;
		width = _width;
		height = _height;
		area = ChipmunkProWrapper.ucpAreaForPoly(numVerts, verts);
	}
}
