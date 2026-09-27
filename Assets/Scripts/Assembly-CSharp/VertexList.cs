using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class VertexList : ISerializable, IDeserializationCallback
{
	public int NofVertices;

	public Vector2[] Vertex;

	private Vertex3[] VerticesFromDeserialization;

	public VertexList()
	{
	}

	public VertexList(SerializationInfo info, StreamingContext ctxt)
	{
		NofVertices = (int)info.GetValue("NofVertices", typeof(int));
		VerticesFromDeserialization = (Vertex3[])info.GetValue("Vertex", typeof(Vertex3[]));
		Vertex = new Vector2[NofVertices];
	}

	public VertexList(Vector2[] p)
	{
		NofVertices = p.Length;
		Vertex = p;
	}

	public void OnDeserialization(object sender)
	{
		for (int i = 0; i < NofVertices; i++)
		{
			Vertex[i] = VerticesFromDeserialization[i].ToVector2();
		}
		VerticesFromDeserialization = null;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("NofVertices", NofVertices);
		Vertex3[] array = new Vertex3[NofVertices];
		for (int i = 0; i < NofVertices; i++)
		{
			array[i] = new Vertex3(Vertex[i]);
		}
		info.AddValue("Vertex", array);
	}

	public GraphicsPath ToGraphicsPath()
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddLines(Vertex);
		return graphicsPath;
	}

	public Vector2[] ToPoints()
	{
		return Vertex;
	}

	public GraphicsPath TristripToGraphicsPath()
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		for (int i = 0; i < NofVertices - 2; i++)
		{
			graphicsPath.AddPolygon(new Vector2[3]
			{
				Vertex[i],
				Vertex[i + 1],
				Vertex[i + 2]
			});
		}
		return graphicsPath;
	}

	public override string ToString()
	{
		string text = "Polygon with " + NofVertices + " vertices: ";
		for (int i = 0; i < NofVertices; i++)
		{
			text += Vertex[i].ToString();
			if (i != NofVertices - 1)
			{
				text += ",";
			}
		}
		return text;
	}
}
