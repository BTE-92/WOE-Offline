using System;
using UnityEngine;

public struct AgTileEdgeVert
{
	public Vector2 pos;

	public Vector2 normal;

	public Vector2 fixedNormal;

	public IntPtr polyline;

	public Vector2 cTangent;

	public bool wasTravelled;

	public bool isCorner;

	public AgTileEdgeVert(Vector2 _pos, Vector2 _normal, bool _isCorner = false)
	{
		pos = _pos;
		normal = _normal;
		fixedNormal = _normal;
		polyline = (IntPtr)0;
		cTangent = Vector2.zero;
		wasTravelled = false;
		isCorner = _isCorner;
	}
}
