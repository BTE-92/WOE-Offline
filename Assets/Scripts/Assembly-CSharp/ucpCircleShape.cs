using UnityEngine;

public class ucpCircleShape : ucpShape
{
	public float innerRadius;

	public float outerRadius;

	public ucpCircleShape(float _radius, Vector2 _offset, float _mass = 10f, float _elasticity = 0.5f, float _friction = 0.5f, ucpCollisionType _collisionType = ucpCollisionType.None, bool _sensor = false)
		: base(ucpShapeType.Circle, _offset, _mass, _elasticity, _friction, _collisionType, _sensor)
	{
		innerRadius = 0f;
		outerRadius = _radius;
		area = ChipmunkProWrapper.ucpAreaForCircle(innerRadius, outerRadius);
	}

	public ucpCircleShape(float _innerRadius, float _outerRadius, Vector2 _offset, float _mass = 10f, float _elasticity = 0.5f, float _friction = 0.5f, ucpCollisionType _collisionType = ucpCollisionType.None, bool _sensor = false)
		: base(ucpShapeType.Circle, _offset, _mass, _elasticity, _friction, _collisionType, _sensor)
	{
		innerRadius = _innerRadius;
		outerRadius = _outerRadius;
		area = ChipmunkProWrapper.ucpAreaForCircle(innerRadius, outerRadius);
	}
}
