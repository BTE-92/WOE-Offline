using System;
using UnityEngine;

public class AgTile
{
	private static int m_instanceCount;

	public IntPtr tilePtr;

	public IntPtr[] shapes;

	public AgTileEdgeVert[] edgeVerts;

	public int shapeCount;

	public TransformC TC;

	public Vector2 pos;

	public cpBB bb;

	public bool dirty;

	public bool regenerateCollisionShapes;

	public AgTile(int _shapeSpace)
	{
		shapes = new IntPtr[_shapeSpace];
		shapeCount = 0;
		dirty = false;
		edgeVerts = null;
		regenerateCollisionShapes = true;
		pos = Vector3.zero;
		m_instanceCount++;
	}

	~AgTile()
	{
		m_instanceCount--;
	}
}
