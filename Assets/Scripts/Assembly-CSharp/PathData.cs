using UnityEngine;

public class PathData
{
	public Vector2[] Points;

	public byte[] Types;

	public PathData()
	{
		Points = new Vector2[0];
	}

	public new byte[] GetType()
	{
		return Types;
	}
}
