using UnityEngine;

public class ucpSegmentShape : ucpShape
{
	public Vector2 a;

	public Vector2 b;

	public float radius;

	public ucpSegmentShape(Vector2 _a, Vector2 _b, float _radius, Vector2 _offset, float _mass = 10f, float _elasticity = 0.5f, float _friction = 0.5f, ucpCollisionType _collisionType = ucpCollisionType.None, bool _sensor = false)
		: base(ucpShapeType.Segment, _offset, _mass, _elasticity, _friction, _collisionType, _sensor)
	{
		a = _a;
		b = _b;
		radius = _radius;
	}
}
