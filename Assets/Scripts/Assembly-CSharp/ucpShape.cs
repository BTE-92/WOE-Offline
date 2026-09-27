using System;
using UnityEngine;

public class ucpShape
{
	public ucpShapeType shapeType;

	public IntPtr shapePtr;

	public Vector2 offset;

	public float elasticity;

	public float friction;

	public ucpCollisionType collisionType;

	public bool sensor;

	public uint group;

	public uint layers;

	public Vector2 surfaceVelocity;

	public float mass;

	public float area;

	public ucpShape(ucpShapeType _shapeType, Vector2 _offset, float _mass, float _elasticity, float _friction, ucpCollisionType _collisionType, bool _sensor)
	{
		shapeType = _shapeType;
		shapePtr = IntPtr.Zero;
		offset = _offset;
		elasticity = _elasticity;
		friction = _friction;
		collisionType = _collisionType;
		sensor = _sensor;
		group = 0u;
		layers = 1u;
		surfaceVelocity = Vector2.zero;
		mass = _mass;
		area = 1f;
	}
}
