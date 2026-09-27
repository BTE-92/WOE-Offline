using System;
using UnityEngine;

public struct ucpCollisionPair
{
	public int ucpComponentIndexA;

	public int ucpComponentIndexB;

	public IntPtr shapeA;

	public IntPtr shapeB;

	public uint typeA;

	public uint typeB;

	public Vector2 point;

	public Vector2 normal;

	public Vector2 impulse;

	public Vector2 friction;
}
