using System.Collections.Generic;
using UnityEngine;

public class AgPolygon
{
	public bool isHole;

	public List<Vector2> vertices;

	public List<Vector3> extraData;

	public AgPolygon()
	{
		vertices = new List<Vector2>();
		extraData = new List<Vector3>();
	}

	public void Clear()
	{
		vertices.Clear();
		extraData.Clear();
	}
}
