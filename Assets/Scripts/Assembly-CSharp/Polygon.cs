using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Polygon : ISerializable
{
	public int NofContours;

	public bool[] ContourIsHole;

	public VertexList[] Contour;

	public Polygon()
	{
	}

	public Polygon(SerializationInfo info, StreamingContext ctxt)
	{
		NofContours = (int)info.GetValue("NofContours", typeof(int));
		ContourIsHole = (bool[])info.GetValue("ContourIsHole", typeof(bool[]));
		Contour = (VertexList[])info.GetValue("Contour", typeof(VertexList[]));
	}

	public Polygon(GraphicsPath path)
	{
		NofContours = 0;
		byte[] pathTypes = path.PathTypes;
		Vector2[] pathPoints = path.PathPoints;
		byte[] array = pathTypes;
		foreach (byte b in array)
		{
			if ((b & 6) != 0)
			{
				NofContours++;
			}
		}
		ContourIsHole = new bool[NofContours];
		Contour = new VertexList[NofContours];
		for (int j = 0; j < NofContours; j++)
		{
			ContourIsHole[j] = j == 0;
		}
		int num = 0;
		List<Vector2> list = new List<Vector2>();
		for (int k = 0; k < pathPoints.Length; k++)
		{
			list.Add(pathPoints[k]);
			if ((path.PathTypes[k] & 6) != 0)
			{
				Vector2[] p = list.ToArray();
				VertexList vertexList = new VertexList(p);
				Contour[num++] = vertexList;
				list.Clear();
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("NofContours", NofContours);
		info.AddValue("ContourIsHole", ContourIsHole);
		info.AddValue("Contour", Contour);
	}

	public void AddContour(VertexList contour, bool contourIsHole)
	{
		bool[] array = new bool[NofContours + 1];
		VertexList[] array2 = new VertexList[NofContours + 1];
		for (int i = 0; i < NofContours; i++)
		{
			array[i] = ContourIsHole[i];
			array2[i] = Contour[i];
		}
		array[NofContours] = contourIsHole;
		array2[NofContours++] = contour;
		ContourIsHole = array;
		Contour = array2;
	}

	public GraphicsPath ToGraphicsPath()
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		for (int i = 0; i < NofContours; i++)
		{
			Vector2[] array = Contour[i].ToPoints();
			if (ContourIsHole[i])
			{
				Array.Reverse(array);
			}
			graphicsPath.AddPolygon(array);
		}
		return graphicsPath;
	}

	public override string ToString()
	{
		string text = "Polygon with " + NofContours + " contours.\r\n";
		for (int i = 0; i < NofContours; i++)
		{
			text = ((!ContourIsHole[i]) ? (text + "Contour: ") : (text + "Hole: "));
			text += Contour[i].ToString();
		}
		return text;
	}

	public Tristrip ClipToTristrip(GpcOperation operation, Polygon polygon)
	{
		return GpcWrapper.ClipToTristrip(operation, this, polygon);
	}

	public Polygon Clip(GpcOperation operation, Polygon polygon)
	{
		return GpcWrapper.Clip(operation, this, polygon);
	}

	public Tristrip ToTristrip()
	{
		return GpcWrapper.PolygonToTristrip(this);
	}
}
