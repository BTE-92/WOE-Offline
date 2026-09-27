using System;
using System.Collections.Generic;
using LibTessDotNet;
using UnityEngine;

public static class AutogeometryVisuals
{
	public static UnityEngine.Mesh CreateBeltMeshFromVertexArray(AgPolygon _poly, float _width, Vector3 _offset, bool _loop, Vector2 _tileCenter, float _smoothingAngle)
	{
		bool flag = false;
		UnityEngine.Mesh mesh = new UnityEngine.Mesh();
		int num = _poly.vertices.Count - ((!_loop) ? 1 : 0);
		int num2 = num * 6;
		int[] array = new int[num2 * ((!flag) ? 1 : 2)];
		Vector3[] array2 = new Vector3[_poly.vertices.Count - (_loop ? 1 : 0)];
		Vector3[] array3 = new Vector3[_poly.vertices.Count - (_loop ? 1 : 0)];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new Vector3(_poly.vertices[i].x + _offset.x, _poly.vertices[i].y + _offset.y, _offset.z) - (Vector3)_tileCenter;
			array3[i] = new Vector3(_poly.extraData[i].x, _poly.extraData[i].y, 0f);
		}
		Color black = Color.black;
		CVertex[] array4;
		if (_smoothingAngle < 180f)
		{
			List<CVertex> list = new List<CVertex>();
			for (int j = 0; j < array2.Length; j++)
			{
				Vector3 vector;
				Vector3 vector2;
				if (_loop)
				{
					int rolledValue = ToolBox.getRolledValue(j - 1, 0, array2.Length - 1);
					int rolledValue2 = ToolBox.getRolledValue(j + 1, 0, array2.Length - 1);
					vector = array2[rolledValue] - array2[j];
					vector2 = array2[j] - array2[rolledValue2];
				}
				else
				{
					int rolledValue = ToolBox.limitBetween(j - 1, 0, array2.Length - 1);
					int rolledValue2 = ToolBox.limitBetween(j + 1, 0, array2.Length - 1);
					vector = ((j <= 0) ? (array2[rolledValue] - array2[j + 1]) : (array2[rolledValue] - array2[j]));
					vector2 = ((j >= array2.Length - 1) ? (array2[j - 1] - array2[rolledValue2]) : (array2[j] - array2[rolledValue2]));
				}
				float num3 = Vector2.Angle(vector, vector2);
				if (num3 >= _smoothingAngle)
				{
					list.Add(new CVertex(array2[j], Vector2.zero, black, new Vector2(vector.y, 0f - vector.x).normalized));
					list.Add(new CVertex(array2[j], Vector2.zero, black, new Vector2(vector2.y, 0f - vector2.x).normalized, true));
				}
				else
				{
					list.Add(new CVertex(array2[j], Vector2.zero, black, array3[j]));
				}
			}
			array4 = list.ToArray();
		}
		else
		{
			array4 = new CVertex[array2.Length];
			for (int k = 0; k < array2.Length; k++)
			{
				array4[k] = new CVertex(array2[k], Vector2.zero, black, array3[k]);
			}
		}
		CVertex[] array5 = array4.Clone() as CVertex[];
		for (int l = 0; l < array5.Length; l++)
		{
			array5[l].vert.z += _width;
		}
		int num4 = array4.Length;
		int num5 = 0;
		int num6 = 0;
		for (int m = 0; m < num4; m++)
		{
			int num7 = (num6 + 1) % num4;
			if (!array4[num7].isDuplicate && num5 < num2)
			{
				array[num5] = num6;
				array[num5 + 1] = num6 + num4;
				array[num5 + 2] = num7;
				array[num5 + 3] = num7;
				array[num5 + 4] = num6 + num4;
				array[num5 + 5] = num7 + num4;
				num5 += 6;
			}
			num6++;
		}
		Vector3[] array6 = new Vector3[array4.Length * 2 * ((!flag) ? 1 : 2)];
		Vector2[] array7 = new Vector2[array4.Length * 2 * ((!flag) ? 1 : 2)];
		Color[] array8 = new Color[array4.Length * 2 * ((!flag) ? 1 : 2)];
		Vector3[] array9 = new Vector3[array4.Length * 2 * ((!flag) ? 1 : 2)];
		for (int n = 0; n < array4.Length; n++)
		{
			array6[n] = array4[n].vert;
			array7[n] = array4[n].uv;
			array8[n] = array4[n].color;
			array9[n] = array4[n].normal;
			array6[n + array4.Length] = array5[n].vert;
			array7[n + array4.Length] = array5[n].uv;
			array8[n + array4.Length] = array5[n].color;
			array9[n + array4.Length] = array5[n].normal;
		}
		if (flag)
		{
			Array.Copy(array6, 0, array6, array6.Length / 2, array6.Length / 2);
			Array.Copy(array7, 0, array7, array6.Length / 2, array6.Length / 2);
			Array.Copy(array8, 0, array8, array6.Length / 2, array6.Length / 2);
			Array.Copy(array9, 0, array9, array6.Length / 2, array6.Length / 2);
			Array.Copy(array, 0, array, array.Length / 2, array.Length / 2);
			int num8 = array6.Length / 2;
			for (int num9 = array.Length / 2; num9 < array.Length; num9 += 3)
			{
				int num10 = array[num9] + num8;
				int num11 = array[num9 + 1] + num8;
				int num12 = array[num9 + 2] + num8;
				array[num9] = num12;
				array[num9 + 1] = num11;
				array[num9 + 2] = num10;
			}
		}
		mesh.vertices = array6;
		mesh.triangles = array;
		mesh.colors = array8;
		mesh.uv = array7;
		mesh.normals = array9;
		return mesh;
	}

	public static Tess TesselateAgPolygons(AgPolygon[] _polyList, Vector2 _offset)
	{
		Tess tess = new Tess();
		for (int i = 0; i < _polyList.Length; i++)
		{
			ContourVertex[] array = new ContourVertex[_polyList[i].vertices.Count];
			for (int j = 0; j < _polyList[i].vertices.Count; j++)
			{
				array[j].Position.X = _polyList[i].vertices[j].x + _offset.x;
				array[j].Position.Y = _polyList[i].vertices[j].y + _offset.y;
				if (_polyList[i].extraData.Count > 0)
				{
					array[j].Data = _polyList[i].extraData[j];
				}
				else
				{
					array[j].Data = new Vector3(0f, 0f, 0f);
				}
			}
			tess.AddContour(array, ContourOrientation.Clockwise);
		}
		tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
		return tess;
	}

	public static UnityEngine.Mesh CreateFrontFaceMeshFromPolygon(AgPolygon[] _polygons, Vector3 _offset, Vector3 _tileCenter)
	{
		Tess tess = TesselateAgPolygons(_polygons, -(Vector2)_tileCenter);
		UnityEngine.Mesh mesh = new UnityEngine.Mesh();
		if (tess.VertexCount > 0)
		{
			int[] array = new int[tess.ElementCount * 3];
			for (int i = 0; i < tess.ElementCount; i++)
			{
				int num = i * 3;
				for (int j = 0; j < 3; j++)
				{
					int num2 = tess.Elements[i * 3 + j];
					if (num2 != -1)
					{
						array[num + 2 - j] = num2;
					}
				}
			}
			int vertexCount = tess.VertexCount;
			Vector3[] array2 = new Vector3[vertexCount];
			Vector2[] array3 = new Vector2[vertexCount];
			Vector2[] array4 = new Vector2[vertexCount];
			Color[] colors = new Color[vertexCount];
			ContourVertex[] vertices = tess.Vertices;
			Vector2 vector = _tileCenter / 200f;
			vector.x = ToolBox.getRolledValue(vector.x, -1f, 1f);
			vector.y = ToolBox.getRolledValue(vector.y, -1f, 1f);
			for (int k = 0; k < vertexCount; k++)
			{
				int num3 = k;
				Vector2 vector2 = new Vector2(vertices[num3].Position.X, vertices[num3].Position.Y);
				array2[num3] = new Vector3(vector2.x, vector2.y, 0f) + _offset;
				Vector2 vector3 = new Vector2(vector2.x, vector2.y) / 200f;
				array4[num3] = vector + vector3;
				float positionBetween = ToolBox.getPositionBetween(vector2.x + _tileCenter.x + 8f, (float)(-AutoGeometryManager.m_width) * 0.5f, (float)AutoGeometryManager.m_width * 0.5f);
				float positionBetween2 = ToolBox.getPositionBetween(vector2.y + _tileCenter.y + 8f, (float)(-AutoGeometryManager.m_height) * 0.5f, (float)AutoGeometryManager.m_height * 0.5f);
				array3[num3] = new Vector2(positionBetween, positionBetween2);
			}
			mesh.vertices = array2;
			mesh.uv = array3;
			mesh.uv2 = array4;
			mesh.colors = colors;
			mesh.triangles = array;
		}
		return mesh;
	}
}
