using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw
{
	public static float m_lineWidth = 1f;

	public static Color defaultColor;

	public static SpriteSheet p_spriteSheet;

	public static SpriteSheet p_uiSpriteSheet;

	public static Frame m_debugFrame;

	public static TransformC m_debugTC;

	public static void Initialize()
	{
		if (CameraS.m_mainCamera == null || CameraS.m_uiCamera == null)
		{
			Debug.LogError("Initialize CameraS first");
			return;
		}
		p_spriteSheet = SpriteS.AddSpriteSheet(CameraS.m_mainCamera, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		p_uiSpriteSheet = SpriteS.AddSpriteSheet(CameraS.m_uiCamera, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		defaultColor = new Color(1f, 1f, 1f, 1f);
		m_debugFrame = new Frame(0f, 0f, 128f, 128f);
		Entity entity = EntityManager.AddEntity("DebugDraw");
		m_debugTC = TransformS.AddComponent(entity, "DebugDraw");
		entity.m_persistent = true;
	}

	public static void Clear(Camera _camera)
	{
		SpriteSheet spriteSheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			spriteSheet = p_uiSpriteSheet;
		}
		while (p_spriteSheet.m_components.m_aliveCount > 0)
		{
			SpriteS.RemoveComponent(spriteSheet.m_components.m_array[spriteSheet.m_components.m_aliveIndices[0]]);
		}
	}

	public static void Clear(Camera _camera, TransformC _tc)
	{
		List<SpriteC> spritesByTransform = SpriteS.GetSpritesByTransform(_tc);
		while (spritesByTransform.Count > 0)
		{
			SpriteS.RemoveComponent(spritesByTransform[0]);
			spritesByTransform.RemoveAt(0);
		}
	}

	public static SpriteC CreateLine(Camera _camera, TransformC _transformComponent, float _length, Vector3 _offset, float _offsetAngle)
	{
		SpriteSheet sheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			sheet = p_uiSpriteSheet;
		}
		SpriteC spriteC = SpriteS.AddComponent(_transformComponent, m_debugFrame, sheet);
		SpriteS.SetOffset(spriteC, _offset, _offsetAngle);
		SpriteS.SetDimensions(spriteC, _length, m_lineWidth);
		SpriteS.SetColor(spriteC, defaultColor);
		return spriteC;
	}

	public static SpriteC CreateLine(Camera _camera, TransformC _transformComponent, Vector3 _start, Vector3 _end)
	{
		SpriteSheet sheet = p_spriteSheet;
		if (p_uiSpriteSheet.m_camera == _camera)
		{
			sheet = p_uiSpriteSheet;
		}
		SpriteC spriteC = SpriteS.AddComponent(_transformComponent, m_debugFrame, sheet);
		Vector3 pos = (_start + _end) * 0.5f;
		Vector3 vector = _end - _start;
		float rot = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		SpriteS.SetOffset(spriteC, pos, rot);
		SpriteS.SetDimensions(spriteC, vector.magnitude, m_lineWidth);
		SpriteS.SetColor(spriteC, defaultColor);
		return spriteC;
	}

	public static void CreateBox(Camera _camera, TransformC _transformComponent, Vector3 _pos, float _width, float _height, bool _createAngleMarker = false)
	{
		Vector3 vector = new Vector3(_width * 0.5f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, _height * 0.5f, 0f);
		CreateLine(_camera, _transformComponent, _width, vector2 + _pos, 0f);
		CreateLine(_camera, _transformComponent, _width, -vector2 + _pos, 0f);
		CreateLine(_camera, _transformComponent, _height, vector + _pos, 90f);
		CreateLine(_camera, _transformComponent, _height, -vector + _pos, 90f);
		if (_createAngleMarker)
		{
			float num = _width * 0.5f * 0.5f;
			float num2 = _height * 0.5f * 0.5f;
			CreateLine(_camera, _transformComponent, num, _pos + new Vector3(num * 0.5f, 0f, 0f), 0f);
			CreateLine(_camera, _transformComponent, num2, _pos + new Vector3(0f, num2 * 0.5f, 0f), 90f);
		}
	}

	public static void CreateCircle(Camera _camera, TransformC _transformComponent, Vector3 pos, float radius, bool createAngleMarker = false, int segments = 16)
	{
		float num = 360f / (float)segments;
		float num2 = (float)Math.PI * radius;
		num2 /= (float)segments;
		float num3 = 90f;
		for (int i = 0; i < segments + 1; i++)
		{
			Vector3 offset = pos + new Vector3(Mathf.Cos(num3 * ((float)Math.PI / 180f)), Mathf.Sin(num3 * ((float)Math.PI / 180f)), 0f) * radius;
			if (i > 0)
			{
				CreateLine(_camera, _transformComponent, num2 * 2f, offset, num * (float)i);
			}
			num3 = ToolBox.getCappedAngle(num3 + num);
		}
		if (createAngleMarker)
		{
			CreateLine(_camera, _transformComponent, radius, pos + new Vector3(0f, radius * 0.5f, 0f), 90f);
		}
	}

	public static void DrawVectorArray(Camera _camera, TransformC _tc, Vector2[] _points)
	{
		for (int i = 0; i < _points.Length - 1; i++)
		{
			Vector2 vector = _points[i];
			Vector2 vector2 = _points[i + 1];
			Vector2 vector3 = vector2 - vector;
			Vector2 vector4 = vector + vector3 * 0.5f;
			float offsetAngle = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			CreateLine(_camera, _tc, vector3.magnitude, vector4, offsetAngle);
		}
	}

	public static Polygon TransformPolygon(Polygon _polygon, TransformC _tc)
	{
		Polygon polygon = new Polygon();
		for (int i = 0; i < _polygon.NofContours; i++)
		{
			GraphicsPath graphicsPath = _polygon.Contour[i].ToGraphicsPath();
			Vector2[] pathPoints = graphicsPath.PathPoints;
			Vector2[] array = new Vector2[pathPoints.Length];
			for (int j = 0; j < pathPoints.Length; j++)
			{
				Vector2 vector = pathPoints[j];
				float f = _tc.transform.rotation.eulerAngles.z * ((float)Math.PI / 180f);
				float x = vector.x * Mathf.Cos(f) - vector.y * Mathf.Sin(f);
				float y = vector.x * Mathf.Sin(f) + vector.y * Mathf.Cos(f);
				array[j] = new Vector2(x, y) + (Vector2)_tc.transform.position;
			}
			VertexList contour = new VertexList(array);
			polygon.AddContour(contour, false);
		}
		return polygon;
	}

	public static Polygon TransformPolygon(Polygon _polygon, Vector2 _pos, float _angle)
	{
		Polygon polygon = new Polygon();
		for (int i = 0; i < _polygon.NofContours; i++)
		{
			GraphicsPath graphicsPath = _polygon.Contour[i].ToGraphicsPath();
			Vector2[] pathPoints = graphicsPath.PathPoints;
			Vector2[] array = new Vector2[pathPoints.Length];
			for (int j = 0; j < pathPoints.Length; j++)
			{
				Vector2 vector = pathPoints[j];
				float f = _angle * ((float)Math.PI / 180f);
				float x = vector.x * Mathf.Cos(f) - vector.y * Mathf.Sin(f);
				float y = vector.x * Mathf.Sin(f) + vector.y * Mathf.Cos(f);
				array[j] = new Vector2(x, y) + _pos;
			}
			VertexList contour = new VertexList(array);
			polygon.AddContour(contour, false);
		}
		return polygon;
	}

	public static Polygon TransformPolygon(Polygon _polygon, Vector2 _pos, Vector2 _scale)
	{
		Polygon polygon = new Polygon();
		for (int i = 0; i < _polygon.NofContours; i++)
		{
			GraphicsPath graphicsPath = _polygon.Contour[i].ToGraphicsPath();
			Vector2[] pathPoints = graphicsPath.PathPoints;
			Vector2[] array = new Vector2[pathPoints.Length];
			for (int j = 0; j < pathPoints.Length; j++)
			{
				array[j] = new Vector2(pathPoints[j].x * _scale.x + _pos.x, pathPoints[j].y * _scale.y + _pos.y);
			}
			VertexList contour = new VertexList(array);
			polygon.AddContour(contour, false);
		}
		return polygon;
	}

	public static void DrawPolygon(Camera _camera, Polygon _polygon, TransformC _tc)
	{
		for (int i = 0; i < _polygon.NofContours; i++)
		{
			GraphicsPath graphicsPath = _polygon.Contour[i].ToGraphicsPath();
			Vector2[] pathPoints = graphicsPath.PathPoints;
			for (int j = 0; j < pathPoints.Length; j++)
			{
				Vector2 vector = pathPoints[j];
				Vector2 vector2 = ((j + 1 >= pathPoints.Length) ? pathPoints[0] : pathPoints[j + 1]);
				Vector2 vector3 = vector2 - vector;
				Vector2 vector4 = vector + vector3 * 0.5f;
				float offsetAngle = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
				CreateLine(_camera, _tc, vector3.magnitude, vector4, offsetAngle);
			}
		}
	}

	public static Polygon Vector2ArrayToPolygon(Vector2[] _vectorArray)
	{
		Polygon polygon = new Polygon();
		polygon.AddContour(new VertexList(_vectorArray), false);
		return polygon;
	}

	public static void AddRandom(Vector2[] _array, float _amount)
	{
		for (int i = 0; i < _array.Length; i++)
		{
			_array[i] += new Vector2(UnityEngine.Random.Range(_amount * -0.5f, _amount * 0.5f), UnityEngine.Random.Range(_amount * -0.5f, _amount * 0.5f));
		}
	}

	public static void AddRadialRandom(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			_array[j] = (_array[j] - zero).normalized * (UnityEngine.Random.Range(0f, _amount) - _amount) + _array[j];
		}
	}

	public static void AddRadialRandomToOutside(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			_array[j] = (_array[j] - zero).normalized * UnityEngine.Random.Range(0f, _amount) + _array[j];
		}
	}

	public static void AddRadialStarShift(Vector2[] _array, float _amount)
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < _array.Length; i++)
		{
			zero += _array[i];
		}
		zero /= (float)_array.Length;
		for (int j = 0; j < _array.Length; j++)
		{
			if (j % 2 == 0)
			{
				_array[j] = (_array[j] - zero).normalized * _amount + _array[j];
			}
		}
	}

	public static void ScaleVectorArray(Vector2[] _vectorArray, Vector2 _amount)
	{
		for (int i = 0; i < _vectorArray.Length; i++)
		{
			_vectorArray[i].x *= _amount.x;
			_vectorArray[i].y *= _amount.y;
		}
	}

	public static void ScalePolygon(Polygon _poly, Vector2 _amount)
	{
		for (int i = 0; i < _poly.NofContours; i++)
		{
			for (int j = 0; j < _poly.Contour[i].NofVertices; j++)
			{
				_poly.Contour[i].Vertex[j].x *= _amount.x;
				_poly.Contour[i].Vertex[j].y *= _amount.y;
			}
		}
	}

	public static void ExpandVectorArray(Vector2[] _vectorArray, float _amount)
	{
		if (_vectorArray.Length <= 2)
		{
			return;
		}
		for (int i = 0; i < _vectorArray.Length; i++)
		{
			Vector2 vector = _vectorArray[i];
			Vector2 vector2;
			Vector2 vector3;
			if (i == 0)
			{
				vector2 = _vectorArray[_vectorArray.Length - 1];
				vector3 = _vectorArray[i + 1];
			}
			else if (i == _vectorArray.Length - 1)
			{
				vector2 = _vectorArray[i - 1];
				vector3 = _vectorArray[0];
			}
			else
			{
				vector2 = _vectorArray[i - 1];
				vector3 = _vectorArray[i + 1];
			}
			Vector2 vector4 = vector3 - vector2;
			float f = Mathf.Atan2(0f - vector4.y, vector4.x);
			float num = Mathf.Sin(f) * _amount;
			float num2 = Mathf.Cos(f) * _amount;
			_vectorArray[i] = new Vector2(vector.x + num, vector.y + num2);
		}
	}

	public static Vector2[] ExtrudePath(Vector2[] _path, float _width)
	{
		Vector2[] array = new Vector2[_path.Length * 2];
		if (_path.Length > 1)
		{
			for (int i = 0; i < _path.Length; i++)
			{
				Vector2 vector = _path[i];
				Vector2 vector2 = ((i != 0) ? ((i != _path.Length - 1) ? _path[i + 1] : (_path[i] + (_path[i] - _path[i - 1]))) : _path[i + 1]);
				Vector2 vector3 = vector2 - vector;
				float f = Mathf.Atan2(0f - vector3.y, vector3.x);
				float num = Mathf.Sin(f) * _width * 0.5f;
				float num2 = Mathf.Cos(f) * _width * 0.5f;
				array[i] = new Vector2(vector.x - num, vector.y - num2);
				array[array.Length - i - 1] = new Vector2(vector.x + num, vector.y + num2);
			}
		}
		return array;
	}

	public static Vector2[] GetCircle(float _radius, int _segments, Vector2 _offset)
	{
		return GetCircle(_radius, _segments, _offset, false);
	}

	public static Vector2[] GetCircle(float _radius, int _segments, Vector2 _offset, bool _closed)
	{
		if (_closed)
		{
			return GetArc(_radius, _segments, 360f, 0f, _offset);
		}
		return GetArc(_radius, _segments, 360f - 360f / (float)_segments, 0f, _offset);
	}

	public static Vector2[] GetRect(float _width, float _height, Vector2 _offset)
	{
		return GetRect(_width, _height, _offset, false);
	}

	public static Vector2[] GetRect(float _width, float _height, Vector2 _offset, bool _closed)
	{
		return GetRoundedRect(_width, _height, 0f, 1, _offset, _closed);
	}

	public static Vector2[] GetLine(Vector2 _start, Vector2 _end, int _middlePointCount)
	{
		Vector2[] array = new Vector2[_middlePointCount + 2];
		Vector2 vector = _end - _start;
		int num = _middlePointCount + 1;
		Vector2 vector2 = vector / num;
		array[0] = _start;
		array[array.Length - 1] = _end;
		for (int i = 1; i < array.Length - 1; i++)
		{
			array[i] = _start + vector2 * i;
		}
		return array;
	}

	public static Vector2[] GetRoundedRect(float _width, float _height, float _radius, int _segments, Vector2 _offset)
	{
		return GetRoundedRect(_width, _height, _radius, _segments, _offset, true);
	}

	public static Vector2[] GetRoundedRect(float _width, float _height, float _radius, int _segments, Vector2 _offset, bool _closed)
	{
		Vector2[] array = null;
		int num = 0;
		if (_closed)
		{
			num = 1;
		}
		if (_height > _radius * 2f && _width > _radius * 2f)
		{
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc = GetArc(_radius, _segments, 90f, 270f, new Vector2(_width * 0.5f - _radius, _height * -0.5f + _radius) + _offset);
			Vector2[] arc2 = GetArc(_radius, _segments, 90f, 180f, new Vector2(_width * -0.5f + _radius, _height * -0.5f + _radius) + _offset);
			Vector2[] arc3 = GetArc(_radius, _segments, 90f, 90f, new Vector2(_width * -0.5f + _radius, _height * 0.5f - _radius) + _offset);
			Vector2[] arc4 = GetArc(_radius, _segments, 90f, 0f, new Vector2(_width * 0.5f - _radius, _height * 0.5f - _radius) + _offset);
			arc.CopyTo(array, 0);
			arc2.CopyTo(array, _segments * 1);
			arc3.CopyTo(array, _segments * 2);
			arc4.CopyTo(array, _segments * 3);
			if (_closed)
			{
				array[array.Length - 1] = arc[0];
			}
		}
		else if (_height > _radius * 2f)
		{
			_radius = _width * 0.5f;
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc5 = GetArc(_radius, _segments * 2, 180f, 180f, new Vector2(0f, _height * -0.5f + _radius) + _offset);
			Vector2[] arc6 = GetArc(_radius, _segments * 2, 180f, 0f, new Vector2(0f, _height * 0.5f - _radius) + _offset);
			arc5.CopyTo(array, 0);
			arc6.CopyTo(array, _segments * 2);
			if (_closed)
			{
				array[array.Length - 1] = arc5[0];
			}
		}
		else if (_width > _radius * 2f)
		{
			_radius = _height * 0.5f;
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc7 = GetArc(_radius, _segments * 2, 180f, 90f, new Vector2(_width * -0.5f + _radius, 0f) + _offset);
			Vector2[] arc8 = GetArc(_radius, _segments * 2, 180f, 270f, new Vector2(_width * 0.5f - _radius, 0f) + _offset);
			arc7.CopyTo(array, 0);
			arc8.CopyTo(array, _segments * 2);
			if (_closed)
			{
				array[array.Length - 1] = arc7[0];
			}
		}
		else
		{
			array = new Vector2[_segments * 4 + num];
			Vector2[] arc9 = GetArc(_radius, _segments * 4, 360f - 360f / (float)_segments * 4f, 0f, _offset);
			arc9.CopyTo(array, 0);
			if (_closed)
			{
				array[array.Length - 1] = arc9[0];
			}
		}
		return array;
	}

	public static Vector2[] GetArc(float radius, int _segments, float _arcAngle, float _startAngle, Vector2 _offset)
	{
		Vector2[] array = new Vector2[_segments];
		float num = _arcAngle / (float)(_segments - 1);
		float num2 = (float)Math.PI * radius;
		num2 /= (float)_segments;
		float num3 = _startAngle;
		for (int num4 = _segments - 1; num4 > -1; num4--)
		{
			array[num4] = _offset + new Vector2(Mathf.Cos(num3 * ((float)Math.PI / 180f)), Mathf.Sin(num3 * ((float)Math.PI / 180f))) * radius;
			num3 = ToolBox.getCappedAngle(num3 + num);
		}
		return array;
	}

	public static Color GetColor(float _r, float _g, float _b)
	{
		return new Color(_r / 255f, _g / 255f, _b / 255f);
	}

	public static Color GetColor(float _r, float _g, float _b, float _a)
	{
		return new Color(_r / 255f, _g / 255f, _b / 255f, _a / 255f);
	}

	public static uint ColorToUInt(Color _color)
	{
		return (uint)((Mathf.RoundToInt(_color.a * 255f) << 24) | (Mathf.RoundToInt(_color.r * 255f) << 16) | (Mathf.RoundToInt(_color.g * 255f) << 8) | Mathf.RoundToInt(_color.b * 255f));
	}

	public static Color UIntToColor(uint _uint)
	{
		return GetColor((_uint >> 16) & 0xFF, (_uint >> 8) & 0xFF, _uint & 0xFF, (_uint >> 24) & 0xFF);
	}
}
