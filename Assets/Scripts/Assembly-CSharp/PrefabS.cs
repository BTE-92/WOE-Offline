using System.Collections.Generic;
using LibTessDotNet;
using UnityEngine;

public static class PrefabS
{
	public static DynamicArray<PrefabC> m_components;

	public static GameObject m_emptyGameObject;

	public static void Initialize()
	{
		m_components = new DynamicArray<PrefabC>();
		m_emptyGameObject = new GameObject("PrefabSystem: InstantiateHelper");
		MeshFilter meshFilter = m_emptyGameObject.AddComponent("MeshFilter") as MeshFilter;
		meshFilter.mesh = new UnityEngine.Mesh();
		MeshRenderer meshRenderer = m_emptyGameObject.AddComponent("MeshRenderer") as MeshRenderer;
		meshRenderer.enabled = false;
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset)
	{
		return AddComponent(_parentTC, _offset, string.Empty);
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, string _name)
	{
		PrefabC prefabC = m_components.AddItem();
		prefabC.p_gameObject = Object.Instantiate(m_emptyGameObject) as GameObject;
		prefabC.p_gameObject.renderer.enabled = true;
		prefabC.p_mesh = (prefabC.p_gameObject.GetComponent("MeshFilter") as MeshFilter).mesh;
		prefabC.p_gameObject.transform.parent = _parentTC.transform;
		prefabC.p_gameObject.transform.localPosition = _offset;
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_parentTC = _parentTC;
		prefabC.m_name = _name;
		prefabC.m_wasVisible = true;
		prefabC.p_gameObject.renderer.castShadows = false;
		prefabC.p_gameObject.renderer.receiveShadows = false;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, prefabC);
		return prefabC;
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, GameObject _gameObject)
	{
		return AddComponent(_parentTC, _offset, _gameObject, string.Empty);
	}

	public static PrefabC AddComponent(TransformC _parentTC, Vector3 _offset, GameObject _gameObject, string _identifier, bool _resetLocalRotation = true)
	{
		PrefabC prefabC = m_components.AddItem();
		prefabC.p_gameObject = Object.Instantiate(_gameObject) as GameObject;
		if (prefabC.p_gameObject.renderer == null)
		{
			MeshRenderer meshRenderer = prefabC.p_gameObject.AddComponent<MeshRenderer>();
		}
		prefabC.p_gameObject.transform.parent = _parentTC.transform;
		prefabC.p_gameObject.transform.localPosition = _offset;
		if (_resetLocalRotation)
		{
			prefabC.p_gameObject.transform.localRotation = Quaternion.identity;
		}
		prefabC.p_parentTC = _parentTC;
		prefabC.m_name = _identifier;
		prefabC.m_wasVisible = true;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, prefabC);
		return prefabC;
	}

	public static void RemoveComponent(PrefabC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		if (_c.p_gameObject.renderer != null)
		{
			Object.Destroy(_c.p_gameObject.renderer.material);
		}
		if (_c.p_mesh != null)
		{
			Object.Destroy(_c.p_mesh);
		}
		if (_c.p_gameObject != null)
		{
			Object.Destroy(_c.p_gameObject);
		}
		_c.p_mesh = null;
		_c.p_gameObject = null;
		_c.p_parentTC = null;
		_c.m_name = string.Empty;
		_c.m_wasVisible = false;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static void RemoveComponentsByEntity(Entity _e)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Prefab, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponent(componentsByEntity[index] as PrefabC);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void Update()
	{
		m_components.Update();
	}

	public static void SetCamera(PrefabC _c, Camera _camera)
	{
		SetCameraLayer(_c, _camera.gameObject.layer);
	}

	public static void SetCameraLayer(PrefabC _c, int _cameraLayer)
	{
		_c.p_gameObject.layer = _cameraLayer;
		for (int i = 0; i < _c.p_gameObject.transform.childCount; i++)
		{
			SetCameraLayer(_c.p_gameObject.transform.GetChild(i).gameObject, _cameraLayer);
		}
	}

	public static void SetCamera(GameObject _go, Camera _camera)
	{
		SetCameraLayer(_go, _camera.gameObject.layer);
	}

	public static void SetCameraLayer(GameObject _go, int _cameraLayer)
	{
		_go.layer = _cameraLayer;
		for (int i = 0; i < _go.transform.childCount; i++)
		{
			SetCameraLayer(_go.transform.GetChild(i).gameObject, _cameraLayer);
		}
	}

	public static void PauseParticleSystems(PrefabC _c, bool _pause)
	{
		Component[] componentsInChildren = _c.p_gameObject.GetComponentsInChildren(typeof(ParticleSystem));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem particleSystem = componentsInChildren[i] as ParticleSystem;
			if (_pause)
			{
				particleSystem.Pause(true);
			}
			else
			{
				particleSystem.Play(true);
			}
		}
	}

	public static void SetVertexColors(PrefabC _c, Color _color)
	{
		MeshFilter meshFilter = _c.p_gameObject.GetComponent("MeshFilter") as MeshFilter;
		UnityEngine.Mesh mesh = meshFilter.mesh;
		SetVertexColors(mesh, _color);
	}

	public static void SetVertexColors(GameObject _gameObject, Color _color)
	{
		MeshFilter meshFilter = _gameObject.GetComponent("MeshFilter") as MeshFilter;
		UnityEngine.Mesh mesh = meshFilter.mesh;
		SetVertexColors(mesh, _color);
	}

	public static void SetVertexColors(UnityEngine.Mesh _mesh, Color _color)
	{
		Color[] array = new Color[_mesh.colors.Length];
		for (int i = 0; i < _mesh.colors.Length; i++)
		{
			array[i] = _color;
		}
		_mesh.colors = array;
	}

	public static void SetVisibilityByTransformComponent(TransformC _tc, bool _visible, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetVisibilityByTransformComponent(_tc.childs[i], _visible, true);
			}
		}
		int aliveCount = m_components.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			PrefabC prefabC = m_components.m_array[m_components.m_aliveIndices[j]];
			if (prefabC.p_parentTC == _tc)
			{
				SetVisibility(prefabC, _visible);
			}
		}
	}

	public static void SetVisibility(PrefabC _c, bool _visible, bool _markVisibility = true)
	{
		_c.p_gameObject.SetActive(_visible);
		if (_markVisibility)
		{
			_c.m_wasVisible = _visible;
		}
	}

	public static void ColorizeByTransformComponent(TransformC _tc, Color _color, bool _affectChildren, bool _affectWholeHierarchy)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				ColorizeByTransformComponent(_tc.childs[i], _color, true, false);
			}
		}
		int aliveCount = m_components.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			PrefabC prefabC = m_components.m_array[m_components.m_aliveIndices[j]];
			if (prefabC.p_parentTC == _tc && prefabC.p_mesh != null)
			{
				SetVertexColors(prefabC.p_mesh, _color);
			}
		}
	}

	public static Color GetShaderColor(PrefabC _c)
	{
		return _c.p_gameObject.renderer.material.GetColor("_Color");
	}

	public static void SetShaderColor(PrefabC _c, Color _color)
	{
		_c.p_gameObject.renderer.material.SetColor("_Color", _color);
	}

	public static List<PrefabC> CreatePathPrefabComponentFromPolygon(TransformC _tc, Vector3 _offset, Polygon _polygon, float _width, Color _color, Material _material, Camera _camera, Position _align, bool _closed)
	{
		List<PrefabC> list = new List<PrefabC>();
		for (int i = 0; i < _polygon.NofContours; i++)
		{
			GraphicsPath graphicsPath = _polygon.Contour[i].ToGraphicsPath();
			Vector2[] pathPoints = graphicsPath.PathPoints;
			list.Add(CreatePathPrefabComponentFromVectorArray(_tc, _offset, pathPoints, _width, _color, _material, _camera, _align, _closed));
		}
		return list;
	}

	public static PrefabC CreateRect(TransformC _tc, Vector3 _offset, float _width, float _height, Color _color, Material _material, Camera _camera)
	{
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		Object.Destroy(prefabC.p_gameObject.renderer.material);
		prefabC.p_gameObject.renderer.material = _material;
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(_width * -0.5f, _height * 0.5f, 0f) + _offset,
			new Vector3(_width * 0.5f, _height * 0.5f, 0f) + _offset,
			new Vector3(_width * 0.5f, _height * -0.5f, 0f) + _offset,
			new Vector3(_width * -0.5f, _height * -0.5f, 0f) + _offset
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f)
		};
		Color[] colors = new Color[4] { _color, _color, _color, _color };
		int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = vertices;
		prefabC.p_mesh.triangles = triangles;
		prefabC.p_mesh.uv = uv;
		prefabC.p_mesh.colors = colors;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreatePathPrefabComponentFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, float _width, Color _color, Material _material, Camera _camera, Position _align, bool _closed)
	{
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		Object.Destroy(prefabC.p_gameObject.renderer.material);
		prefabC.p_gameObject.renderer.material = _material;
		Vector2[] array;
		if (_points[0] - _points[_points.Length - 1] == Vector2.zero || !_closed)
		{
			array = _points;
		}
		else
		{
			array = new Vector2[_points.Length + 1];
			_points.CopyTo(array, 0);
			array[array.Length - 1] = array[0];
		}
		Vector3[] array2 = new Vector3[array.Length * 2];
		Vector3[] array3 = new Vector3[array.Length * 2];
		Vector2[] array4 = new Vector2[array.Length * 2];
		Color[] array5 = new Color[array.Length * 2];
		int[] array6 = new int[array.Length * 6];
		float num = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			Vector2 vector = array[i];
			Vector2 vector2;
			Vector2 vector3;
			if (!_closed)
			{
				if (i == 0)
				{
					vector2 = array[array.Length - 1];
					vector3 = array[i + 1];
				}
				else if (i == array.Length - 1)
				{
					vector2 = array[i - 1];
					vector3 = array[0];
				}
				else
				{
					vector2 = array[i - 1];
					vector3 = array[i + 1];
				}
			}
			else if (i == 0)
			{
				vector2 = array[array.Length - 2];
				vector3 = array[i + 1];
			}
			else if (i == array.Length - 1)
			{
				vector2 = array[i - 1];
				vector3 = array[1];
			}
			else
			{
				vector2 = array[i - 1];
				vector3 = array[i + 1];
			}
			Vector2 normalized = (vector2 - vector).normalized;
			Vector2 normalized2 = (vector - vector3).normalized;
			float f = Mathf.Atan2(0f - normalized.y, normalized.x);
			float x = Mathf.Sin(f);
			float y = Mathf.Cos(f);
			Vector2 vector4 = new Vector2(x, y);
			float f2 = Mathf.Atan2(0f - normalized2.y, normalized2.x);
			float x2 = Mathf.Sin(f2);
			float y2 = Mathf.Cos(f2);
			Vector2 vector5 = new Vector2(x2, y2);
			Vector2 normalized3 = ((vector4 + vector5) * 0.5f).normalized;
			Vector3 vector6 = new Vector3(normalized3.x, normalized3.y, 0f);
			Vector3 vector7 = new Vector3(vector.x, vector.y, 0f);
			Vector3 vector8 = vector7;
			Vector3 vector9 = vector7;
			switch (_align)
			{
			case Position.Center:
				vector8 = vector7 + vector6 * _width * 0.5f;
				vector9 = vector7 - vector6 * _width * 0.5f;
				break;
			case Position.Inside:
				vector8 = vector7 + vector6 * _width;
				vector9 = vector7;
				break;
			case Position.Outside:
				vector8 = vector7;
				vector9 = vector7 - vector6 * _width;
				break;
			}
			array2[i * 2] = vector8 + _offset;
			array2[i * 2 + 1] = vector9 + _offset;
			num += (vector - vector2).magnitude;
			array4[i * 2] = Vector2.up * num;
			array4[i * 2 + 1] = Vector2.up * num + Vector2.right;
			array3[i * 2] = Vector3.forward;
			array3[i * 2 + 1] = Vector3.forward;
			array5[i * 2] = _color;
			array5[i * 2 + 1] = _color;
			if (i < array.Length - 1)
			{
				array6[i * 6] = i * 2;
				array6[i * 6 + 1] = i * 2 + 1;
				array6[i * 6 + 2] = i * 2 + 2;
				array6[i * 6 + 3] = i * 2 + 2;
				array6[i * 6 + 4] = i * 2 + 1;
				array6[i * 6 + 5] = i * 2 + 3;
			}
		}
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = array2;
		prefabC.p_mesh.triangles = array6;
		prefabC.p_mesh.uv = array4;
		prefabC.p_mesh.colors = array5;
		prefabC.p_mesh.normals = array3;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static PrefabC CreateLinePrefabComponentFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, float _width, Color _color, Material _material, Camera _camera, Position _align)
	{
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		prefabC.p_gameObject.layer = _camera.gameObject.layer;
		Object.Destroy(prefabC.p_gameObject.renderer.material);
		prefabC.p_gameObject.renderer.material = _material;
		Vector3[] array = new Vector3[_points.Length * 2];
		Vector3[] array2 = new Vector3[_points.Length * 2];
		Vector2[] array3 = new Vector2[_points.Length * 2];
		Color[] array4 = new Color[_points.Length * 2];
		int[] array5 = new int[_points.Length * 6];
		float num = 0f;
		for (int i = 0; i < _points.Length; i++)
		{
			Vector2 vector = _points[i];
			Vector2 vector2;
			Vector2 vector3;
			if (i == 0)
			{
				vector2 = _points[i + 1];
				vector3 = vector + vector - vector2;
			}
			else if (i == _points.Length - 1)
			{
				vector3 = _points[i - 1];
				vector2 = vector + vector - vector3;
			}
			else
			{
				vector3 = _points[i - 1];
				vector2 = _points[i + 1];
			}
			Vector2 normalized = (vector3 - vector).normalized;
			Vector2 normalized2 = (vector - vector2).normalized;
			float f = Mathf.Atan2(0f - normalized.y, normalized.x);
			float x = Mathf.Sin(f);
			float y = Mathf.Cos(f);
			Vector2 vector4 = new Vector2(x, y);
			float f2 = Mathf.Atan2(0f - normalized2.y, normalized2.x);
			float x2 = Mathf.Sin(f2);
			float y2 = Mathf.Cos(f2);
			Vector2 vector5 = new Vector2(x2, y2);
			Vector2 normalized3 = ((vector4 + vector5) * 0.5f).normalized;
			Vector3 vector6 = new Vector3(normalized3.x, normalized3.y, 0f);
			Vector3 vector7 = new Vector3(vector.x, vector.y, 0f);
			Vector3 vector8 = vector7;
			Vector3 vector9 = vector7;
			switch (_align)
			{
			case Position.Center:
				vector8 = vector7 + vector6 * _width * 0.5f;
				vector9 = vector7 - vector6 * _width * 0.5f;
				break;
			case Position.Inside:
				vector8 = vector7 + vector6 * _width;
				vector9 = vector7;
				break;
			case Position.Outside:
				vector8 = vector7;
				vector9 = vector7 - vector6 * _width;
				break;
			}
			array[i * 2] = vector8 + _offset;
			array[i * 2 + 1] = vector9 + _offset;
			num += (vector - vector3).magnitude;
			array3[i * 2] = Vector2.up * num;
			array3[i * 2 + 1] = Vector2.up * num + Vector2.right;
			array2[i * 2] = Vector3.forward;
			array2[i * 2 + 1] = Vector3.forward;
			array4[i * 2] = _color;
			array4[i * 2 + 1] = _color;
			if (i < _points.Length - 1)
			{
				array5[i * 6] = i * 2;
				array5[i * 6 + 1] = i * 2 + 1;
				array5[i * 6 + 2] = i * 2 + 2;
				array5[i * 6 + 3] = i * 2 + 2;
				array5[i * 6 + 4] = i * 2 + 1;
				array5[i * 6 + 5] = i * 2 + 3;
			}
		}
		prefabC.p_mesh.triangles = null;
		prefabC.p_mesh.vertices = null;
		prefabC.p_mesh.vertices = array;
		prefabC.p_mesh.triangles = array5;
		prefabC.p_mesh.uv = array3;
		prefabC.p_mesh.colors = array4;
		prefabC.p_mesh.normals = array2;
		prefabC.p_mesh.RecalculateBounds();
		prefabC.p_mesh.RecalculateNormals();
		return prefabC;
	}

	public static UnityEngine.Mesh CreateMeshFromVector2Array(Vector2[] _vertices)
	{
		return CreateMeshFromVector2Array(_vertices, Vector3.zero);
	}

	public static UnityEngine.Mesh CreateMeshFromVector2Array(Vector2[] _vertices, Vector3 _offset)
	{
		Tess tess = new Tess();
		ContourVertex[] array = new ContourVertex[_vertices.Length];
		for (int i = 0; i < _vertices.Length; i++)
		{
			array[i].Position.X = _vertices[i].x;
			array[i].Position.Y = _vertices[i].y;
		}
		tess.AddContour(array, ContourOrientation.Clockwise);
		tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
		UnityEngine.Mesh mesh = new UnityEngine.Mesh();
		if (tess.VertexCount > 0)
		{
			int[] array2 = new int[tess.ElementCount * 3];
			for (int j = 0; j < tess.ElementCount; j++)
			{
				int num = j * 3;
				for (int k = 0; k < 3; k++)
				{
					int num2 = tess.Elements[j * 3 + k];
					if (num2 != -1)
					{
						array2[num + 2 - k] = num2;
					}
				}
			}
			int vertexCount = tess.VertexCount;
			Vector3[] array3 = new Vector3[vertexCount];
			Vector2[] array4 = new Vector2[vertexCount];
			Color[] colors = new Color[vertexCount];
			array = tess.Vertices;
			for (int l = 0; l < vertexCount; l++)
			{
				int num3 = l;
				Vector2 vector = new Vector2(array[num3].Position.X, array[num3].Position.Y);
				array3[num3] = new Vector3(vector.x, vector.y, 0f) + _offset;
				array4[num3] = Vector2.zero;
			}
			mesh.vertices = array3;
			mesh.uv = array4;
			mesh.colors = colors;
			mesh.triangles = array2;
		}
		return mesh;
	}

	public static List<PrefabC> CreateFlatPrefabComponentsFromPolygon(TransformC _tc, Vector3 _offset, Polygon _polygon, Color _color, Material _material, Camera _camera)
	{
		uint num = DebugDraw.ColorToUInt(_color);
		return CreateFlatPrefabComponentsFromPolygon(_tc, _offset, _polygon, num, num, _material, _camera, string.Empty, null);
	}

	public static List<PrefabC> CreateFlatPrefabComponentsFromVectorArray(TransformC _tc, Vector3 _offset, Vector2[] _points, uint _bottomColor, uint _topColor, Material _material, Camera _camera, string _identifier, UVRect _normalizeRect = null)
	{
		Polygon polygon = new Polygon();
		polygon.AddContour(new VertexList(_points), false);
		return CreateFlatPrefabComponentsFromPolygon(_tc, _offset, polygon, _bottomColor, _topColor, _material, _camera, _identifier, _normalizeRect);
	}

	public static List<PrefabC> CreateFlatPrefabComponentsFromPolygon(TransformC _tc, Vector3 _offset, Polygon _polygon, uint _bottomColor, uint _topColor, Material _material, Camera _camera, string _identifier, UVRect _normalizeRect)
	{
		List<PrefabC> list = new List<PrefabC>();
		if (_polygon.NofContours > 0)
		{
			Tristrip tristrip = _polygon.ToTristrip();
			float num = 99999f;
			float num2 = -99999f;
			float num3 = 99999f;
			float num4 = -99999f;
			for (int i = 0; i < tristrip.NofStrips; i++)
			{
				VertexList vertexList = tristrip.Strip[i];
				for (int j = 0; j < vertexList.NofVertices; j++)
				{
					Vector2 vector = vertexList.Vertex[j];
					if (vector.y < num)
					{
						num = vector.y;
					}
					if (vector.y > num2)
					{
						num2 = vector.y;
					}
					if (vector.x < num3)
					{
						num3 = vector.x;
					}
					if (vector.x > num4)
					{
						num4 = vector.x;
					}
				}
			}
			float num5 = num2 - num;
			float num6 = num4 - num3;
			for (int k = 0; k < tristrip.NofStrips; k++)
			{
				PrefabC prefabC = AddComponent(_tc, Vector3.zero);
				prefabC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
				prefabC.p_gameObject.layer = _camera.gameObject.layer;
				Object.Destroy(prefabC.p_gameObject.renderer.material);
				prefabC.p_gameObject.renderer.material = _material;
				prefabC.m_name = _identifier;
				VertexList vertexList2 = tristrip.Strip[k];
				Vector3[] array = new Vector3[vertexList2.NofVertices];
				Vector2[] array2 = new Vector2[vertexList2.NofVertices];
				Color[] array3 = new Color[vertexList2.NofVertices];
				int[] array4 = new int[(vertexList2.NofVertices - 2) * 3];
				Color color = DebugDraw.UIntToColor(_bottomColor);
				Color color2 = DebugDraw.UIntToColor(_topColor);
				int num7 = -1;
				for (int l = 0; l < vertexList2.NofVertices; l++)
				{
					Vector2 vector2 = vertexList2.Vertex[l];
					array[l] = new Vector3(vector2.x, vector2.y, 0f) + _offset;
					if (_normalizeRect != null)
					{
						float x = (vector2.x - num3) / num6 * _normalizeRect.width + _normalizeRect.left;
						float y = (vector2.y - num) / num5 * _normalizeRect.height + _normalizeRect.bottom;
						array2[l] = new Vector2(x, y);
					}
					else
					{
						array2[l] = vector2;
					}
					float num8 = (vector2.y - num) / num5;
					array3[l] = color2 * num8 + color * (1f - num8);
					if (l < vertexList2.NofVertices - 2)
					{
						if (num7 == -1)
						{
							array4[l * 3] = l;
							array4[l * 3 + 1] = l + 2;
							array4[l * 3 + 2] = l + 1;
							num7 *= -1;
						}
						else
						{
							array4[l * 3] = l;
							array4[l * 3 + 1] = l + 1;
							array4[l * 3 + 2] = l + 2;
							num7 *= -1;
						}
					}
				}
				prefabC.p_mesh.triangles = null;
				prefabC.p_mesh.vertices = null;
				prefabC.p_mesh.vertices = array;
				prefabC.p_mesh.triangles = array4;
				prefabC.p_mesh.uv = array2;
				prefabC.p_mesh.colors = array3;
				prefabC.p_mesh.RecalculateBounds();
				prefabC.p_mesh.RecalculateNormals();
				list.Add(prefabC);
			}
		}
		return list;
	}

	public static PrefabC CreatePrefabFromMesh(TransformC _tc, UnityEngine.Mesh _mesh, Camera _camera, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		return CreatePrefabFromMesh(_tc, _mesh, _camera.gameObject.layer, _material, _destroyMeshes, _recalcNormals, _optimize);
	}

	public static PrefabC CreatePrefabFromMesh(TransformC _tc, UnityEngine.Mesh _mesh, int _cameraLayer, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		if (_mesh != null)
		{
			PrefabC prefabC = AddComponent(_tc, Vector3.zero);
			CombineInstance[] array = new CombineInstance[1];
			array[0].mesh = _mesh;
			prefabC.p_mesh.CombineMeshes(array, true, false);
			prefabC.p_mesh.RecalculateBounds();
			if (_recalcNormals)
			{
				prefabC.p_mesh.RecalculateNormals();
			}
			if (_optimize)
			{
				prefabC.p_mesh.Optimize();
			}
			prefabC.p_gameObject.renderer.material = _material;
			prefabC.p_gameObject.layer = _cameraLayer;
			return prefabC;
		}
		return null;
	}

	public static PrefabC CreatePrefabFromMeshArray(TransformC _tc, UnityEngine.Mesh[] _meshes, Camera _camera, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		return CreatePrefabFromMeshArray(_tc, _meshes, _camera.gameObject.layer, _material, _destroyMeshes, _recalcNormals, _optimize);
	}

	public static PrefabC CreatePrefabFromMeshArray(TransformC _tc, UnityEngine.Mesh[] _meshes, int _cameraLayer, Material _material, bool _destroyMeshes, bool _recalcNormals = false, bool _optimize = false)
	{
		CombineInstance[] array = new CombineInstance[_meshes.Length];
		for (int i = 0; i < _meshes.Length; i++)
		{
			CombineInstance combineInstance = new CombineInstance
			{
				mesh = _meshes[i]
			};
			array[i] = combineInstance;
		}
		PrefabC prefabC = AddComponent(_tc, Vector3.zero);
		prefabC.p_mesh.CombineMeshes(array, true, false);
		prefabC.p_mesh.RecalculateBounds();
		if (_recalcNormals)
		{
			prefabC.p_mesh.RecalculateNormals();
		}
		if (_optimize)
		{
			prefabC.p_mesh.Optimize();
		}
		prefabC.p_gameObject.renderer.material = _material;
		prefabC.p_gameObject.layer = _cameraLayer;
		if (_destroyMeshes)
		{
			for (int j = 0; j < _meshes.Length; j++)
			{
				if (_meshes[j] != null)
				{
					Object.DestroyImmediate(_meshes[j]);
				}
			}
		}
		return prefabC;
	}

	public static UVRect GetLargestUVRect(float _texWidth, float _texHeight, float _targetWidth, float _targetHeight)
	{
		if (_texWidth == _targetWidth && _texHeight == _targetHeight)
		{
			return UVRect.Normal();
		}
		if (_texWidth <= _targetWidth && _texHeight <= _targetHeight)
		{
			float num = _texWidth / _targetWidth;
			float num2 = _texHeight / _targetHeight;
			if (num > num2)
			{
				float num3 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
				return new UVRect(0f, (1f - num3) * 0.5f, 1f, num3);
			}
			float num4 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
			return new UVRect((1f - num4) * 0.5f, 0f, num4, 1f);
		}
		if (_texWidth > _targetWidth && _texHeight > _targetHeight)
		{
			float num5 = _texWidth / _targetWidth;
			float num6 = _texHeight / _targetHeight;
			if (num5 > num6)
			{
				float num7 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
				return new UVRect((1f - num7) * 0.5f, 0f, num7, 1f);
			}
			float num8 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
			return new UVRect(0f, (1f - num8) * 0.5f, 1f, num8);
		}
		if (_texWidth <= _targetWidth && _texHeight > _targetHeight)
		{
			float num9 = _targetHeight / _targetWidth / (_texHeight / _texWidth);
			return new UVRect(0f, (1f - num9) * 0.5f, 1f, num9);
		}
		if (_texWidth > _targetWidth && _texHeight <= _targetHeight)
		{
			float num10 = _texHeight / _texWidth / (_targetHeight / _targetWidth);
			return new UVRect((1f - num10) * 0.5f, 0f, num10, 1f);
		}
		return UVRect.Normal();
	}
}
