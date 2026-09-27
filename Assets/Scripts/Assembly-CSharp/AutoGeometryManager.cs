using System.Collections.Generic;
using UnityEngine;

public static class AutoGeometryManager
{
	public const int MAX_LAYERS = 10;

	public const int TEXEL_SCALE = 16;

	public static int m_width;

	public static int m_height;

	public static Vector2 m_tileCacheOffset;

	public static List<AutoGeometryLayer> m_layers = new List<AutoGeometryLayer>();

	public static List<AutoGeometryBrush> m_brushes = new List<AutoGeometryBrush>();

	public static int m_layerWidth;

	public static int m_layerHeight;

	public static TransformC m_debugDrawTC;

	public static void Initialize(int _width, int _height)
	{
		m_width = _width;
		m_height = _height;
		m_layerWidth = m_width / 16;
		m_layerHeight = m_height / 16;
		Debug.Log("Autogeometry manager initialize for world: " + m_width + "/" + m_height);
		m_tileCacheOffset = -new Vector2(m_width, m_height) * 0.5f;
		m_debugDrawTC = EntityManager.AddEntityWithTC("AutoGeometryDebugDrawTC");
		m_debugDrawTC.p_entity.m_persistent = true;
	}

	public static void Update()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].Update();
		}
	}

	public static AutoGeometryLayer AddLayer(Ground _groundClass, int _tileSize, int _samplesPerTile, float _simplifyTreshold = 1f)
	{
		_tileSize *= 16;
		if (m_layers.Count < 10)
		{
			AutoGeometryLayer autoGeometryLayer = new AutoGeometryLayer(_groundClass, _tileSize, _samplesPerTile, 16, _simplifyTreshold);
			m_layers.Add(autoGeometryLayer);
			return autoGeometryLayer;
		}
		Debug.LogError("No more layers allowed!");
		return null;
	}

	public static AutoGeometryBrush AddBrush(string _resourceName, bool _useSubPixelAccuracy = false, bool _storeBytes = true)
	{
		AutoGeometryBrush autoGeometryBrush = new AutoGeometryBrush(_resourceName, _useSubPixelAccuracy, _storeBytes);
		m_brushes.Add(autoGeometryBrush);
		return autoGeometryBrush;
	}

	public static void RemoveLayer(AutoGeometryLayer layer)
	{
		m_layers.Remove(layer);
	}

	public static void UpdateSegments()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].UpdateSegments();
		}
	}

	public static void DestroyAllLayers()
	{
		Debug.Log("lol");
		while (m_layers.Count > 0)
		{
			int index = m_layers.Count - 1;
			m_layers[index].Destroy();
			m_layers.RemoveAt(index);
		}
	}

	public static void DestroyAllBrushes()
	{
		while (m_brushes.Count > 0)
		{
			int index = m_brushes.Count - 1;
			m_brushes[index].Destroy();
			m_brushes.RemoveAt(index);
		}
	}

	public static void ClearTileDirtyFlags()
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			m_layers[i].ClearAgTileDirtyFlags();
		}
	}

	public static void UpdateMaxValueLookupTable(AutoGeometryLayer _layer)
	{
		bool flag = true;
		for (int i = 0; i < m_layers.Count; i++)
		{
			if (m_layers[i] == _layer)
			{
				continue;
			}
			for (int j = 0; j < m_layerWidth * m_layerHeight; j++)
			{
				byte b = m_layers[i].m_bytes[j];
				if (flag)
				{
					_layer.m_maxValueLookupBytes[j] = b;
					continue;
				}
				byte b2 = _layer.m_maxValueLookupBytes[j];
				if (b > b2)
				{
					_layer.m_maxValueLookupBytes[j] = b;
				}
			}
			flag = false;
		}
	}

	public static int GetLayerAtWorldPos(Vector2 _worldPosition)
	{
		byte b = 127;
		int result = -1;
		for (int i = 0; i < m_layers.Count; i++)
		{
			byte b2 = m_layers[i].ReadDataFromWorldPos(_worldPosition);
			if (b2 > b)
			{
				result = i;
				b = b2;
			}
		}
		return result;
	}

	public static void HighlightLayer(AutoGeometryLayer _layer, float _amount, float _speed = 0.1f)
	{
		for (int i = 0; i < m_layers.Count; i++)
		{
			float emissionAmount = 0f;
			if (_layer == m_layers[i])
			{
				emissionAmount = _amount;
			}
			m_layers[i].SetHighlight(emissionAmount, _speed);
		}
	}

	public static void PaintWithBrush(AutoGeometryLayer _layer, AutoGeometryBrush _brush, Vector2 _pos, bool _additive, bool _soften)
	{
		int index = m_layers.IndexOf(_layer);
		m_layers[index].PaintWithBrush(_brush, _pos, (!_additive) ? AGDrawMode.SUB : AGDrawMode.ADD, _soften, ref m_layers[index].m_bytes, true, true);
	}
}
