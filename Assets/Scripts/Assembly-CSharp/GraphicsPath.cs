using UnityEngine;

public class GraphicsPath
{
	public FillMode FillMode;

	public PathData PathData;

	public Vector2[] PathPoints
	{
		get
		{
			return PathData.Points;
		}
	}

	public byte[] PathTypes
	{
		get
		{
			return PathData.GetType();
		}
	}

	public int PointCount
	{
		get
		{
			return PathData.Points.Length;
		}
	}

	public GraphicsPath()
	{
		FillMode = FillMode.Alternate;
		PathData = new PathData();
	}

	public void AddLines(Vector2[] points)
	{
		int pointCount = PointCount;
		if (pointCount == 0)
		{
			PathData.Points = points;
		}
		else
		{
			Vector2[] array = new Vector2[pointCount];
			PathData.Points.CopyTo(array, 0);
			PathData.Points = new Vector2[pointCount + points.Length];
			array.CopyTo(PathData.Points, 0);
			points.CopyTo(PathData.Points, pointCount);
		}
		PathData.Types = new byte[PointCount];
		for (int i = 0; i < PointCount; i++)
		{
			if (i == 0)
			{
				PathData.Types[0] = 0;
			}
			else
			{
				PathData.Types[i] = 1;
			}
		}
	}

	public void AddPolygon(Vector2[] points)
	{
		Vector2 vector = points[0] - points[points.Length - 1];
		if (vector == Vector2.zero)
		{
			PathData.Points = points;
		}
		else
		{
			PathData.Points = new Vector2[points.Length + 1];
			points.CopyTo(PathData.Points, 0);
			PathData.Points[points.Length] = points[0];
		}
		PathData.Types = new byte[PointCount];
		for (int i = 0; i < PointCount; i++)
		{
			if (i == 0)
			{
				PathData.Types[0] = 0;
			}
			else if (i == PointCount - 1)
			{
				PathData.Types[i] = 6;
			}
			else
			{
				PathData.Types[i] = 1;
			}
		}
	}
}
