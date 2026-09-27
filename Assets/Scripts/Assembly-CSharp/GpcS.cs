using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GpcS
{
	public static GenericArray<GpcC> m_components;

	public static void Initialize(int _maxComponentCount)
	{
		m_components = new GenericArray<GpcC>(_maxComponentCount);
		for (int i = 0; i < _maxComponentCount; i++)
		{
			m_components.m_array[i] = new GpcC();
			m_components.m_array[i].m_index = i;
			m_components.m_array[i].m_componentType = ComponentType.Gpc;
			m_components.m_array[i].originalPolygon = new Polygon();
			m_components.m_array[i].modifiedPolygon = new Polygon();
			m_components.m_array[i].tiles = new Polygon[0];
		}
	}

	public static GpcC AddComponent(TransformC _transformComponent)
	{
		return AddComponent(_transformComponent, null);
	}

	public static GpcC AddComponent(TransformC _transformComponent, Polygon _poly)
	{
		int num = m_components.AddItem();
		GpcC gpcC = m_components.m_array[num];
		gpcC.p_TC = _transformComponent;
		gpcC.polyMinX = 999999f;
		gpcC.polyMaxX = -999999f;
		gpcC.polyMinY = 999999f;
		gpcC.polyMaxY = -999999f;
		gpcC.polyWidth = 0f;
		gpcC.polyHeight = 0f;
		gpcC.tileWidth = 0;
		gpcC.tileHeight = 0;
		gpcC.tileCountX = 0;
		gpcC.tileCountY = 0;
		if (_poly != null)
		{
			gpcC.originalPolygon = _poly;
			gpcC.modifiedPolygon = _poly;
		}
		EntityManager.AddComponentToEntity(_transformComponent.p_entity, gpcC);
		return gpcC;
	}

	public static void RemoveComponent(GpcC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		_c.p_TC = null;
		_c.originalPolygon = new Polygon();
		_c.modifiedPolygon = new Polygon();
		if (_c.tiles.Length > 0)
		{
			_c.tiles = new Polygon[0];
		}
		m_components.RemoveItem(_c.m_index);
		EntityManager.RemoveComponentFromEntity(_c);
	}

	public static void Update(float _dt)
	{
	}

	public static void SplitPolygonToTiles(GpcC _c, int _tileWidth, int _tileHeight)
	{
		_c.polyMinX = 999999f;
		_c.polyMaxX = -999999f;
		_c.polyMinY = 999999f;
		_c.polyMaxY = -999999f;
		_c.tileWidth = _tileWidth;
		_c.tileHeight = _tileHeight;
		for (int i = 0; i < _c.modifiedPolygon.Contour.Length; i++)
		{
			VertexList vertexList = _c.modifiedPolygon.Contour[i];
			for (int j = 0; j < vertexList.NofVertices; j++)
			{
				_c.polyMinX = Mathf.Min(vertexList.Vertex[j].x, _c.polyMinX);
				_c.polyMaxX = Mathf.Max(vertexList.Vertex[j].x, _c.polyMaxX);
				_c.polyMinY = Mathf.Min(vertexList.Vertex[j].y, _c.polyMinY);
				_c.polyMaxY = Mathf.Max(vertexList.Vertex[j].y, _c.polyMaxY);
			}
		}
		_c.polyWidth = _c.polyMaxX - _c.polyMinX;
		_c.polyHeight = _c.polyMaxY - _c.polyMinY;
		_c.tileCountX = Mathf.CeilToInt(_c.polyWidth / (float)_c.tileWidth);
		_c.tileCountY = Mathf.CeilToInt(_c.polyHeight / (float)_c.tileHeight);
		_c.tiles = new Polygon[_c.tileCountX * _c.tileCountY];
		VertexList contour = new VertexList(new Vector2[4]
		{
			new Vector2(_c.polyMinX, _c.polyMinY),
			new Vector2(_c.polyMinX + (float)_c.tileWidth, _c.polyMinY),
			new Vector2(_c.polyMinX + (float)_c.tileWidth, _c.polyMinY + (float)_c.tileHeight),
			new Vector2(_c.polyMinX, _c.polyMinY + (float)_c.tileHeight)
		});
		Polygon polygon = new Polygon();
		polygon.AddContour(contour, false);
		int num = 0;
		for (int k = 0; k < _c.tileCountY; k++)
		{
			for (int l = 0; l < _c.tileCountX; l++)
			{
				polygon.Contour[0].Vertex[0].x = _c.polyMinX + (float)(_c.tileWidth * l);
				polygon.Contour[0].Vertex[0].y = _c.polyMinY + (float)(_c.tileHeight * k);
				polygon.Contour[0].Vertex[1].x = _c.polyMinX + (float)(_c.tileWidth * l) + (float)_c.tileWidth;
				polygon.Contour[0].Vertex[1].y = _c.polyMinY + (float)(_c.tileHeight * k);
				polygon.Contour[0].Vertex[2].x = _c.polyMinX + (float)(_c.tileWidth * l) + (float)_c.tileWidth;
				polygon.Contour[0].Vertex[2].y = _c.polyMinY + (float)(_c.tileHeight * k) + (float)_c.tileHeight;
				polygon.Contour[0].Vertex[3].x = _c.polyMinX + (float)(_c.tileWidth * l);
				polygon.Contour[0].Vertex[3].y = _c.polyMinY + (float)(_c.tileHeight * k) + (float)_c.tileHeight;
				_c.tiles[num] = _c.modifiedPolygon.Clip(GpcOperation.Intersection, polygon);
				num++;
			}
		}
	}

	public static void SplitPolygonToTilesAtRange(GpcC _c, int _x, int _y, int _rangeX, int _rangeY)
	{
		float num = (float)(_x * _c.tileWidth) + _c.polyMinX;
		float num2 = (float)(_y * _c.tileHeight) + _c.polyMinY;
		float num3 = num;
		float num4 = num2;
		VertexList contour = new VertexList(new Vector2[4]
		{
			new Vector2(num3, num4),
			new Vector2(num3 + (float)_c.tileWidth, num4),
			new Vector2(num3 + (float)_c.tileWidth, num4 + (float)_c.tileHeight),
			new Vector2(num3, num4 + (float)_c.tileHeight)
		});
		Polygon polygon = new Polygon();
		polygon.AddContour(contour, false);
		int num5 = _y * _c.tileCountX + _x;
		for (int i = 0; i < _rangeY; i++)
		{
			for (int j = 0; j < _rangeX; j++)
			{
				int num6 = num5 + j + i * _c.tileCountX;
				if (_c.tiles[num6] != null)
				{
					polygon.Contour[0].Vertex[0].x = num3;
					polygon.Contour[0].Vertex[0].y = num4;
					polygon.Contour[0].Vertex[1].x = num3 + (float)_c.tileWidth;
					polygon.Contour[0].Vertex[1].y = num4;
					polygon.Contour[0].Vertex[2].x = num3 + (float)_c.tileWidth;
					polygon.Contour[0].Vertex[2].y = num4 + (float)_c.tileHeight;
					polygon.Contour[0].Vertex[3].x = num3;
					polygon.Contour[0].Vertex[3].y = num4 + (float)_c.tileHeight;
					_c.tiles[num6] = _c.modifiedPolygon.Clip(GpcOperation.Intersection, polygon);
				}
				num3 += (float)_c.tileWidth;
			}
			num3 = num;
			num4 += (float)_c.tileHeight;
		}
	}

	public static Polygon CleanPolygon(Polygon _polygon, float _minVertexDistance, float _minVertexAngle, float _maxVertexDistance, bool _forceConvex)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			Polygon polygon = new Polygon();
			List<VertexList> list = new List<VertexList>();
			List<bool> list2 = new List<bool>();
			for (int i = 0; i < _polygon.NofContours; i++)
			{
				VertexList vertexList = _polygon.Contour[i];
				bool item = _polygon.ContourIsHole[i];
				if (vertexList.NofVertices <= 2)
				{
					continue;
				}
				List<Vector2> list3 = new List<Vector2>();
				int num = 0;
				for (int j = 0; j < vertexList.NofVertices; j++)
				{
					Vector2 vector = vertexList.Vertex[j];
					Vector2 vector2;
					Vector2 vector3;
					if (j == 0)
					{
						vector2 = vertexList.Vertex[vertexList.NofVertices - 1];
						vector3 = vertexList.Vertex[j + 1];
					}
					else if (j == vertexList.NofVertices - 1)
					{
						vector2 = ((num <= 0) ? vertexList.Vertex[j - 1] : list3[num - 1]);
						vector3 = vertexList.Vertex[0];
					}
					else
					{
						vector2 = ((num <= 0) ? vertexList.Vertex[j - 1] : list3[num - 1]);
						vector3 = vertexList.Vertex[j + 1];
					}
					Vector2 vector4 = vector - vector2;
					Vector2 vector5 = vector3 - vector;
					float num2 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
					float num3 = Mathf.Atan2(vector5.y, vector5.x) * 57.29578f;
					float num4 = num2 - num3;
					if (num4 <= -180f)
					{
						num4 += 360f;
					}
					if (_forceConvex)
					{
						if (num4 < _minVertexAngle || num4 >= 180f)
						{
							flag = true;
							continue;
						}
						list3.Add(vector);
						num++;
					}
					else if ((Mathf.Abs(num2 - num3) > _minVertexAngle && Mathf.Abs(num2 - num3) < 360f - _minVertexAngle) || j == 0)
					{
						list3.Add(vector);
						num++;
					}
					else
					{
						flag = true;
					}
				}
				VertexList vertexList2 = new VertexList();
				vertexList2.Vertex = new Vector2[num];
				vertexList2.NofVertices = num;
				for (int k = 0; k < num; k++)
				{
					vertexList2.Vertex[k] = list3[k];
				}
				if (num > 2)
				{
					list.Add(vertexList2);
					list2.Add(item);
				}
			}
			polygon.Contour = new VertexList[list.Count];
			polygon.ContourIsHole = new bool[list2.Count];
			polygon.NofContours = list.Count;
			for (int l = 0; l < list.Count; l++)
			{
				polygon.Contour[l] = list[l];
				polygon.ContourIsHole[l] = list2[l];
			}
			_polygon = polygon;
		}
		bool flag2 = true;
		while (flag2)
		{
			flag2 = false;
			Polygon polygon2 = new Polygon();
			List<VertexList> list4 = new List<VertexList>();
			List<bool> list5 = new List<bool>();
			for (int m = 0; m < _polygon.NofContours; m++)
			{
				VertexList vertexList3 = _polygon.Contour[m];
				bool item2 = _polygon.ContourIsHole[m];
				if (vertexList3.NofVertices <= 2)
				{
					continue;
				}
				List<Vector2> list6 = new List<Vector2>();
				int num5 = 0;
				for (int n = 0; n < vertexList3.NofVertices; n++)
				{
					Vector2 vector6 = vertexList3.Vertex[n];
					int num6 = n + 1;
					if (n == vertexList3.NofVertices - 1)
					{
						num6 = 0;
					}
					Vector2 vector7 = vertexList3.Vertex[num6] - vector6;
					if (vector7.SqrMagnitude() < _minVertexDistance * _minVertexDistance)
					{
						vertexList3.Vertex[num6] = vector6 + vector7 * 0.5f;
						flag2 = true;
						continue;
					}
					list6.Add(vector6);
					num5++;
					if (vector7.SqrMagnitude() > _maxVertexDistance * _maxVertexDistance)
					{
						Vector2 vector8 = vector7 * 0.5f;
						Vector2 item3 = vector6;
						item3 += vector8;
						list6.Add(item3);
						num5++;
						flag2 = true;
					}
				}
				if (num5 > 2)
				{
					VertexList vertexList4 = new VertexList();
					vertexList4.Vertex = list6.ToArray();
					vertexList4.NofVertices = num5;
					list4.Add(vertexList4);
					list5.Add(item2);
				}
			}
			for (int num7 = 0; num7 < list4.Count; num7++)
			{
				polygon2.AddContour(list4[num7], list5[num7]);
			}
			_polygon = polygon2;
		}
		return _polygon;
	}

	public static Polygon ClonePolygon(Polygon _poly)
	{
		Polygon polygon = new Polygon(_poly.ToGraphicsPath());
		polygon.Contour = new VertexList[_poly.NofContours];
		polygon.ContourIsHole = new bool[_poly.NofContours];
		polygon.NofContours = _poly.NofContours;
		for (int i = 0; i < _poly.NofContours; i++)
		{
			VertexList vertexList = _poly.Contour[i];
			polygon.Contour[i] = new VertexList();
			polygon.Contour[i].Vertex = new Vector2[_poly.Contour[i].NofVertices];
			polygon.Contour[i].NofVertices = _poly.Contour[i].NofVertices;
			polygon.ContourIsHole[i] = _poly.ContourIsHole[i];
			vertexList.Vertex.CopyTo(polygon.Contour[i].Vertex, 0);
		}
		return polygon;
	}

	public static Polygon SmoothPolygon(Polygon _polygon, int _strength)
	{
		Polygon polygon = ClonePolygon(_polygon);
		for (int i = 0; i < _strength; i++)
		{
			for (int j = 0; j < _polygon.NofContours; j++)
			{
				VertexList vertexList = polygon.Contour[j];
				if (vertexList.NofVertices <= 2)
				{
					continue;
				}
				for (int k = 0; k < vertexList.NofVertices; k++)
				{
					Vector2 vector = vertexList.Vertex[k];
					Vector2 vector2;
					Vector2 vector3;
					if (k == 0)
					{
						vector2 = vertexList.Vertex[vertexList.NofVertices - 1];
						vector3 = vertexList.Vertex[k + 1];
					}
					else if (k == vertexList.NofVertices - 1)
					{
						vector2 = vertexList.Vertex[k - 1];
						vector3 = vertexList.Vertex[0];
					}
					else
					{
						vector2 = vertexList.Vertex[k - 1];
						vector3 = vertexList.Vertex[k + 1];
					}
					polygon.Contour[j].Vertex[k] = (vector2 + vector * 3f + vector3) / 5f;
				}
			}
		}
		bool flag = true;
		while (flag)
		{
			flag = false;
			Polygon polygon2 = new Polygon();
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			for (int l = 0; l < polygon.NofContours; l++)
			{
				VertexList vertexList2 = polygon.Contour[l];
				bool flag2 = polygon.ContourIsHole[l];
				if (vertexList2.NofVertices <= 2)
				{
					continue;
				}
				List<Vector2> list = new List<Vector2>();
				int num = 0;
				for (int m = 0; m < vertexList2.NofVertices; m++)
				{
					Vector2 vector4 = vertexList2.Vertex[m];
					Vector2 vector5;
					Vector2 vector6;
					if (m == 0)
					{
						vector5 = vertexList2.Vertex[vertexList2.NofVertices - 1];
						vector6 = vertexList2.Vertex[m + 1];
					}
					else if (m == vertexList2.NofVertices - 1)
					{
						vector5 = ((num <= 0) ? vertexList2.Vertex[m - 1] : list[num - 1]);
						vector6 = vertexList2.Vertex[0];
					}
					else
					{
						vector5 = ((num <= 0) ? vertexList2.Vertex[m - 1] : list[num - 1]);
						vector6 = vertexList2.Vertex[m + 1];
					}
					Vector2 vector7 = vector4 - vector5;
					Vector2 vector8 = vector6 - vector4;
					float num2 = Mathf.Atan2(vector7.y, vector7.x) * 57.29578f;
					float num3 = Mathf.Atan2(vector8.y, vector8.x) * 57.29578f;
					if (Mathf.Abs(num2 - num3) > 5f)
					{
						list.Add(vector4);
						num++;
					}
					else
					{
						flag = true;
					}
				}
				VertexList vertexList3 = new VertexList();
				vertexList3.Vertex = new Vector2[num];
				vertexList3.NofVertices = num;
				for (int n = 0; n < num; n++)
				{
					vertexList3.Vertex[n] = list[n];
				}
				if (num > 2)
				{
					arrayList.Add(vertexList3);
					arrayList2.Add(flag2);
				}
			}
			polygon2.Contour = new VertexList[arrayList.Count];
			polygon2.ContourIsHole = new bool[arrayList2.Count];
			polygon2.NofContours = arrayList.Count;
			for (int num4 = 0; num4 < arrayList.Count; num4++)
			{
				polygon2.Contour[num4] = (VertexList)arrayList[num4];
				polygon2.ContourIsHole[num4] = (bool)arrayList2[num4];
			}
			polygon = polygon2;
		}
		return polygon;
	}

	public static Polygon ScalePolygon(Polygon _polygon, float _amount, bool _resizeHoles = true)
	{
		Polygon polygon = ClonePolygon(_polygon);
		for (int i = 0; i < polygon.NofContours; i++)
		{
			if (!_resizeHoles && polygon.ContourIsHole[i])
			{
				continue;
			}
			VertexList vertexList = _polygon.Contour[i];
			if (vertexList.NofVertices <= 2)
			{
				continue;
			}
			for (int j = 0; j < vertexList.NofVertices; j++)
			{
				Vector2 vector = vertexList.Vertex[j];
				Vector2 vector2;
				Vector2 vector3;
				if (j == 0)
				{
					vector2 = vertexList.Vertex[vertexList.NofVertices - 1];
					vector3 = vertexList.Vertex[j + 1];
				}
				else if (j == vertexList.NofVertices - 1)
				{
					vector2 = vertexList.Vertex[j - 1];
					vector3 = vertexList.Vertex[0];
				}
				else
				{
					vector2 = vertexList.Vertex[j - 1];
					vector3 = vertexList.Vertex[j + 1];
				}
				Vector2 normalized = (vector2 - vector).normalized;
				Vector2 normalized2 = (vector - vector3).normalized;
				float f = Mathf.Atan2(0f - normalized.y, normalized.x);
				float x = Mathf.Sin(f) * _amount;
				float y = Mathf.Cos(f) * _amount;
				Vector2 vector4 = new Vector2(x, y);
				float f2 = Mathf.Atan2(0f - normalized2.y, normalized2.x);
				float x2 = Mathf.Sin(f2) * _amount;
				float y2 = Mathf.Cos(f2) * _amount;
				Vector2 vector5 = new Vector2(x2, y2);
				Vector2 vector6 = (vector4 + vector5) * 0.5f;
				polygon.Contour[i].Vertex[j] = vector + vector6.normalized * (0f - Mathf.Abs(_amount));
			}
		}
		return polygon;
	}
}
