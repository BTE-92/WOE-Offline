using System;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteS
{
	public static int m_resizeTolerance = 64;

	public static GenericArray<SpriteSheet> m_sheets;

	private static SpriteC[] p_sprites;

	private static Vector3[] p_vertices;

	private static int[] p_aliveIndices;

	public static Vector3 m_outOfScreen = new Vector3(99999f, 99999f, -99999f);

	public static Color m_defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private static int debug_rotationUpdates;

	private static int debug_scaleUpdates;

	private static int debug_positionUpdates;

	public static void Initialize()
	{
		m_sheets = new GenericArray<SpriteSheet>(50);
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Material _material, float _globalSpriteScale)
	{
		return AddSpriteSheet(_camera, _material, null, _globalSpriteScale);
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Material _material, TextAsset _atlas, float _globalSpriteScale)
	{
		SpriteSheet spriteSheet = new SpriteSheet(_camera, _material, _atlas, _globalSpriteScale);
		int num = m_sheets.AddItem(spriteSheet);
		m_sheets.m_array[num].m_index = num;
		return spriteSheet;
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Texture _texture, Shader _shader, float _globalSpriteScale)
	{
		SpriteSheet spriteSheet = new SpriteSheet(_camera, _texture, _shader, _globalSpriteScale);
		int num = m_sheets.AddItem(spriteSheet);
		m_sheets.m_array[num].m_index = num;
		return spriteSheet;
	}

	public static void RemoveSpriteSheet(SpriteSheet _sheet)
	{
		RemoveAllComponentsFromSheet(_sheet);
		UnityEngine.Object.Destroy(_sheet.m_gameObject);
		_sheet.m_gameObject = null;
		_sheet.m_atlas = null;
		m_sheets.RemoveItem(_sheet.m_index);
		m_sheets.m_array[_sheet.m_index] = null;
	}

	public static TiledSpriteSheet AddTiledSpriteSheet(float _startX, float _startY, float _tileSize, int _width, int _height, int _maxComponentCount, Camera _camera, Texture _texture, Shader _shader, float _globalSpriteScale)
	{
		TiledSpriteSheet tiledSpriteSheet = new TiledSpriteSheet();
		tiledSpriteSheet.m_startX = _startX;
		tiledSpriteSheet.m_startY = _startY;
		tiledSpriteSheet.m_tileSize = _tileSize;
		tiledSpriteSheet.m_width = _width;
		tiledSpriteSheet.m_height = _height;
		tiledSpriteSheet.m_textureWidth = _texture.width;
		tiledSpriteSheet.m_textureHeight = _texture.height;
		tiledSpriteSheet.m_sheets = new SpriteSheet[_width * _height];
		Material material = new Material(_shader);
		material.mainTexture = _texture;
		for (int i = 0; i < _width * _height; i++)
		{
			SpriteSheet item = new SpriteSheet(_camera, material, _globalSpriteScale);
			int num = m_sheets.AddItem(item);
			m_sheets.m_array[num].m_index = num;
			m_sheets.m_array[num].m_gameObject.name = "SpriteSheet (tile:" + i + ")";
			int num2 = Mathf.FloorToInt((float)i / (float)_width);
			int num3 = i - num2 * _width;
			m_sheets.m_array[num].m_mesh.bounds = new Bounds(new Vector3(_startX + _tileSize * (float)num3 + _tileSize * 0.5f, _startY + _tileSize * (float)num2 + _tileSize * 0.5f, 0f), new Vector3(_tileSize, _tileSize, _tileSize));
			tiledSpriteSheet.m_sheets[i] = m_sheets.m_array[num];
		}
		return tiledSpriteSheet;
	}

	public static SpriteC AddComponent(TransformC _transformComponent, string _frameName, SpriteSheet _sheet)
	{
		Frame frame = _sheet.m_atlas.GetFrame(_frameName);
		Debug.Log(frame.x + "/" + frame.y);
		Debug.Log(frame.width + "/" + frame.height);
		return AddComponent(_transformComponent, frame, _sheet);
	}

	public static SpriteC AddComponent(TransformC _transformComponent, Frame _frame, SpriteSheet _sheet)
	{
		if (_sheet.m_components.m_aliveCount == 0)
		{
			_sheet.m_gameObject.SetActive(true);
		}
		SpriteC spriteC = _sheet.m_components.AddItem();
		if (_sheet.m_components.m_aliveCount < _sheet.m_currentMeshCapacity)
		{
			spriteC.vertDataIndex = _sheet.m_freeVertexIndices[_sheet.m_freeVertexIndicesCount - 1];
			_sheet.m_freeVertexIndicesCount--;
		}
		else
		{
			spriteC.vertDataIndex = -1;
		}
		spriteC.p_TC = _transformComponent;
		spriteC.p_spriteSheet = _sheet;
		SetFrame(_sheet, spriteC, _frame);
		EntityManager.AddComponentToEntity(_transformComponent.p_entity, spriteC);
		return spriteC;
	}

	public static SpriteC AddComponent(TransformC _transformComponent, Frame _frame, TiledSpriteSheet _tiledSheet)
	{
		Vector3 position = _transformComponent.transform.position;
		int num = Mathf.FloorToInt((position.x - _tiledSheet.m_startX) / _tiledSheet.m_tileSize);
		int num2 = Mathf.FloorToInt((position.y - _tiledSheet.m_startY) / _tiledSheet.m_tileSize);
		int num3 = num2 * _tiledSheet.m_width + num;
		SpriteSheet sheet = _tiledSheet.m_sheets[num3];
		return AddComponent(_transformComponent, _frame, sheet);
	}

	public static void RemoveComponent(SpriteC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		SpriteSheet p_spriteSheet = _c.p_spriteSheet;
		if (_c.vertDataIndex >= 0)
		{
			int num = _c.vertDataIndex * 4;
			for (int i = 0; i < 4; i++)
			{
				p_spriteSheet.m_vertices[num + i] = m_outOfScreen;
			}
			p_spriteSheet.m_vertsChanged = true;
			p_spriteSheet.m_freeVertexIndices[p_spriteSheet.m_freeVertexIndicesCount] = _c.vertDataIndex;
			p_spriteSheet.m_freeVertexIndicesCount++;
		}
		_c.p_TC = null;
		_c.p_spriteSheet = null;
		_c.sortValue = 0f;
		EntityManager.RemoveComponentFromEntity(_c);
		p_spriteSheet.m_components.RemoveItem(_c);
		if (p_spriteSheet.m_components.m_aliveCount == 0)
		{
			p_spriteSheet.m_gameObject.SetActive(false);
		}
	}

	public static void RemoveComponentsByEntity(Entity _e)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponent(componentsByEntity[index] as SpriteC);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void RemoveAllComponentsFromSheet(SpriteSheet _sheet)
	{
		while (_sheet.m_components.m_aliveCount > 0)
		{
			SpriteC c = _sheet.m_components.m_array[_sheet.m_components.m_aliveIndices[0]];
			RemoveComponent(c);
		}
	}

	public static void RemoveSpritesFromTransformComponent(TransformC _tc)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _tc.p_entity);
		while (componentsByEntity.Count > 0)
		{
			RemoveComponent(componentsByEntity[0] as SpriteC);
			componentsByEntity.RemoveAt(0);
		}
	}

	public static void SetSpriteSheetCamera(SpriteSheet _sheet, Camera _camera)
	{
		_sheet.m_camera = _camera;
		_sheet.m_gameObject.layer = _sheet.m_camera.gameObject.layer;
	}

	public static void SetDimensions(SpriteC _c, float _width, float _height)
	{
		_c.wDimension = _width;
		_c.hDimension = _height;
		_c.width = _width * _c.p_spriteSheet.m_globalSpriteScale;
		_c.height = _height * _c.p_spriteSheet.m_globalSpriteScale;
		_c.updateScale = true;
	}

	public static void SetDimensionScale(SpriteC _c, float _scale)
	{
		_c.dimensionScale = _scale;
		_c.updateScale = true;
	}

	public static void SetVisibility(SpriteC _s, bool _visible)
	{
		SetVisibility(_s.p_spriteSheet.m_index, _s.m_index, _visible, true);
	}

	public static void SetVisibility(SpriteC _s, bool _visible, bool _markVisibility)
	{
		SetVisibility(_s.p_spriteSheet.m_index, _s.m_index, _visible, _markVisibility);
	}

	public static void SetVisibility(int _sheetIndex, int _spriteIndex, bool _visible, bool _markVisibility)
	{
		SpriteC spriteC = m_sheets.m_array[_sheetIndex].m_components.m_array[_spriteIndex];
		spriteC.visible = _visible;
		if (_markVisibility)
		{
			spriteC.wasVisible = _visible;
		}
		spriteC.updatePosition = true;
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
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				if (spriteC.p_TC == _tc)
				{
					SetVisibility(spriteC, _visible);
				}
			}
		}
	}

	public static void SetAlignment(int _sheetIndex, int _spriteIndex, Align _horizontal, Align _vertical)
	{
		float horizontal = 0f;
		switch (_horizontal)
		{
		case Align.Left:
			horizontal = -0.5f;
			break;
		case Align.Right:
			horizontal = 0.5f;
			break;
		}
		float vertical = 0f;
		switch (_horizontal)
		{
		case Align.Top:
			vertical = 0.5f;
			break;
		case Align.Bottom:
			vertical = -0.5f;
			break;
		}
		SetAlignment(_sheetIndex, _spriteIndex, horizontal, vertical);
	}

	public static void SetAlignment(int _sheetIndex, int _spriteIndex, float _horizontal, float _vertical)
	{
		SpriteC spriteC = m_sheets.m_array[_sheetIndex].m_components.m_array[_spriteIndex];
		spriteC.align.x = (0f - _horizontal) * spriteC.width;
		spriteC.align.y = _vertical * spriteC.height;
		spriteC.updatePosition = true;
	}

	public static void SetOffset(SpriteC _s, Vector3 _pos, float _rot)
	{
		_s.offset = _pos;
		float num = Mathf.Cos(_rot * ((float)Math.PI / 180f));
		float num2 = Mathf.Sin(_rot * ((float)Math.PI / 180f));
		_s.offsetRight = new Vector3(num, num2, 0f);
		_s.offsetUp = new Vector3(0f - num2, num, 0f);
		_s.updateRotation = true;
	}

	public static void SetColor(SpriteC _sprite, Color _color)
	{
		_sprite.color = _color;
		_sprite.updateColors = true;
	}

	public static void SetColorByTransformComponent(TransformC _tc, Color _color, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetColorByTransformComponent(_tc.childs[i], _color, true);
			}
		}
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				if (spriteC.p_TC == _tc)
				{
					SetColor(spriteC, _color);
				}
			}
		}
	}

	public static void SetAlphaByTransformComponent(TransformC _tc, float _alpha, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetAlphaByTransformComponent(_tc.childs[i], _alpha, true);
			}
		}
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				Color color = spriteC.color;
				color.a = _alpha;
				if (spriteC.p_TC == _tc)
				{
					SetColor(spriteC, color);
				}
			}
		}
	}

	public static void SetFlip(SpriteC _c, bool _x, bool _y)
	{
		_c.frame.flipX = _x;
		_c.frame.flipY = _y;
		SetFrame(_c.p_spriteSheet, _c, _c.frame);
	}

	public static void SetSortValue(TransformC _tc, float _sortValue)
	{
		for (int i = 0; i < m_sheets.m_aliveCount; i++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[i]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int j = 0; j < aliveCount; j++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[j]];
				if (spriteC.p_TC == _tc)
				{
					SetSortValue(spriteC, _sortValue);
				}
			}
		}
	}

	public static void SetSortValue(SpriteC _s, float _sortValue)
	{
		_s.sortValue = _sortValue;
		_s.p_spriteSheet.m_sortMesh = true;
	}

	public static void SortMesh(SpriteSheet _sheet)
	{
		_sheet.resetVertexIndices();
		int[] array = new int[_sheet.m_components.m_aliveCount];
		float[] array2 = new float[_sheet.m_components.m_aliveCount];
		for (int i = 0; i < _sheet.m_components.m_aliveCount; i++)
		{
			int num = _sheet.m_components.m_aliveIndices[i];
			SpriteC spriteC = _sheet.m_components.m_array[num];
			if (spriteC.vertDataIndex >= 0)
			{
				int num2 = spriteC.vertDataIndex * 4;
				for (int j = 0; j < 4; j++)
				{
					_sheet.m_vertices[num2 + j] = m_outOfScreen;
				}
			}
			array[i] = num;
			array2[i] = spriteC.sortValue;
		}
		array = ToolBox.sortTable(array, array2);
		for (int k = 0; k < _sheet.m_components.m_aliveCount; k++)
		{
			SpriteC spriteC2 = _sheet.m_components.m_array[array[k]];
			spriteC2.vertDataIndex = _sheet.m_freeVertexIndices[_sheet.m_freeVertexIndicesCount - 1];
			_sheet.m_freeVertexIndicesCount--;
		}
	}

	public static void SetFrame(SpriteSheet _sheet, SpriteC _sprite, Frame _frame)
	{
		_sprite.frame = _frame;
		if (_sprite.wDimension > 0f)
		{
			_sprite.width = _sprite.wDimension * _sheet.m_globalSpriteScale;
		}
		else
		{
			_sprite.wDimension = _frame.width;
			_sprite.width = _frame.width * _sheet.m_globalSpriteScale;
		}
		if (_sprite.hDimension > 0f)
		{
			_sprite.height = _sprite.hDimension * _sheet.m_globalSpriteScale;
		}
		else
		{
			_sprite.hDimension = _frame.height;
			_sprite.height = _frame.height * _sheet.m_globalSpriteScale;
		}
		_sprite.updateUVs = true;
	}

	public static void Update()
	{
		for (int i = 0; i < m_sheets.m_aliveCount; i++)
		{
			int num = m_sheets.m_aliveIndices[i];
			SpriteSheet spriteSheet = m_sheets.m_array[num];
			if (spriteSheet.m_currentMeshCapacity <= spriteSheet.m_components.m_aliveCount || Mathf.Abs(spriteSheet.m_components.m_aliveCount - spriteSheet.m_currentMeshCapacity) >= m_resizeTolerance * 2)
			{
				spriteSheet.setMeshData(spriteSheet.m_components.m_aliveCount + m_resizeTolerance);
				spriteSheet.assignVertexDataIndices();
				if (spriteSheet.m_sortingEnabled)
				{
					spriteSheet.m_sortMesh = true;
				}
			}
			if (spriteSheet.m_sortingEnabled && spriteSheet.m_sortMesh)
			{
				SortMesh(spriteSheet);
				spriteSheet.m_sortMesh = false;
				spriteSheet.m_meshCapacityChanged = true;
			}
			if (spriteSheet.m_components.m_aliveCount > 0)
			{
				p_sprites = spriteSheet.m_components.m_array;
				p_vertices = spriteSheet.m_vertices;
				for (int j = 0; j < spriteSheet.m_components.m_aliveCount; j++)
				{
					int num2 = spriteSheet.m_components.m_aliveIndices[j];
					SpriteC spriteC = spriteSheet.m_components.m_array[num2];
					TransformC p_TC = spriteC.p_TC;
					Transform transform = spriteC.p_TC.transform;
					if (p_TC.updatedPosition)
					{
						spriteC.updatePosition = true;
					}
					if (p_TC.updatedRotation)
					{
						spriteC.updateRotation = true;
					}
					if (p_TC.updatedScale)
					{
						spriteC.updateScale = true;
					}
					if (spriteC.vertDataIndex < 0)
					{
						spriteC.vertDataIndex = spriteSheet.m_freeVertexIndices[spriteSheet.m_freeVertexIndicesCount - 1];
						spriteSheet.m_freeVertexIndicesCount--;
					}
					bool flag = false;
					if (spriteC.updateScale || spriteSheet.m_meshCapacityChanged)
					{
						if (p_TC.forceScale)
						{
							spriteC.wScale = spriteC.width * spriteC.dimensionScale * 0.5f * p_TC.forcedScale.x;
							spriteC.hScale = spriteC.height * spriteC.dimensionScale * 0.5f * p_TC.forcedScale.y;
						}
						else
						{
							Vector3 lossyScale = transform.lossyScale;
							spriteC.wScale = spriteC.width * spriteC.dimensionScale * 0.5f * lossyScale.x;
							spriteC.hScale = spriteC.height * spriteC.dimensionScale * 0.5f * lossyScale.y;
						}
						spriteC.updatePosition = true;
						spriteC.updateScale = false;
						flag = true;
						debug_scaleUpdates++;
					}
					if (spriteC.updateRotation || spriteSheet.m_meshCapacityChanged)
					{
						if (p_TC.forceRotation)
						{
							spriteC.relRight = p_TC.forcedRotation * spriteC.offsetRight;
							spriteC.relUp = p_TC.forcedRotation * spriteC.offsetUp;
							spriteC.relOffset = p_TC.forcedRotation * spriteC.offset;
							if (!p_TC.forceScale)
							{
								spriteC.relOffset.Scale(transform.lossyScale);
							}
						}
						else
						{
							Quaternion rotation = transform.rotation;
							spriteC.relRight = rotation * spriteC.offsetRight;
							spriteC.relUp = rotation * spriteC.offsetUp;
							spriteC.relOffset = rotation * spriteC.offset;
							if (!p_TC.forceScale)
							{
								spriteC.relOffset.Scale(transform.lossyScale);
							}
						}
						spriteC.updatePosition = true;
						spriteC.updateRotation = false;
						flag = true;
						debug_rotationUpdates++;
					}
					if (flag)
					{
						spriteC.scaledRelRight = spriteC.relRight * spriteC.wScale;
						spriteC.scaledRelUp = spriteC.relUp * spriteC.hScale;
					}
					if (spriteC.updatePosition)
					{
						int num3 = spriteC.vertDataIndex * 4;
						if (spriteC.visible)
						{
							Vector3 vector = transform.position + spriteC.relOffset;
							p_vertices[num3] = vector + spriteC.scaledRelUp - spriteC.scaledRelRight;
							p_vertices[num3 + 1] = vector - spriteC.scaledRelUp - spriteC.scaledRelRight;
							p_vertices[num3 + 2] = vector - spriteC.scaledRelUp + spriteC.scaledRelRight;
							p_vertices[num3 + 3] = vector + spriteC.scaledRelUp + spriteC.scaledRelRight;
						}
						else
						{
							for (int k = 0; k < 4; k++)
							{
								p_vertices[num3 + k] = m_outOfScreen;
							}
						}
						spriteC.updatePosition = false;
						spriteSheet.m_vertsChanged = true;
						debug_positionUpdates++;
					}
					if (spriteC.updateUVs || spriteSheet.m_meshCapacityChanged)
					{
						float num4 = spriteC.frame.x / (float)spriteSheet.m_textureWidth;
						float num5 = (spriteC.frame.x + spriteC.frame.width) / (float)spriteSheet.m_textureWidth;
						float num6 = 1f - spriteC.frame.y / (float)spriteSheet.m_textureHeight;
						float num7 = 1f - (spriteC.frame.y + spriteC.frame.height) / (float)spriteSheet.m_textureHeight;
						if (spriteC.frame.flipX)
						{
							float num8 = num4;
							num4 = num5;
							num5 = num8;
						}
						if (spriteC.frame.flipY)
						{
							float num9 = num6;
							num6 = num7;
							num7 = num9;
						}
						int num10 = spriteC.vertDataIndex * 4;
						Vector2[] uVs = spriteSheet.m_UVs;
						uVs[num10 + 3].x = num5;
						uVs[num10 + 3].y = num6;
						uVs[num10 + 2].x = num5;
						uVs[num10 + 2].y = num7;
						uVs[num10 + 1].x = num4;
						uVs[num10 + 1].y = num7;
						uVs[num10].x = num4;
						uVs[num10].y = num6;
						spriteC.updateUVs = false;
						spriteSheet.m_uvsChanged = true;
					}
					if (spriteC.updateColors || spriteSheet.m_meshCapacityChanged)
					{
						int num11 = spriteC.vertDataIndex * 4;
						Color[] colors = spriteSheet.m_colors;
						colors[num11] = spriteC.color;
						colors[num11 + 1] = spriteC.color;
						colors[num11 + 2] = spriteC.color;
						colors[num11 + 3] = spriteC.color;
						spriteC.updateColors = false;
						spriteSheet.m_colorsChanged = true;
					}
				}
			}
			if (spriteSheet.m_vertsChanged)
			{
				spriteSheet.m_mesh.vertices = spriteSheet.m_vertices;
				spriteSheet.m_vertsChanged = false;
			}
			if (spriteSheet.m_colorsChanged)
			{
				spriteSheet.m_mesh.colors = spriteSheet.m_colors;
				spriteSheet.m_colorsChanged = false;
			}
			if (spriteSheet.m_uvsChanged)
			{
				spriteSheet.m_mesh.uv = spriteSheet.m_UVs;
				spriteSheet.m_uvsChanged = false;
			}
			spriteSheet.m_meshCapacityChanged = false;
			spriteSheet.m_components.Update();
		}
		debug_rotationUpdates = 0;
		debug_scaleUpdates = 0;
		debug_positionUpdates = 0;
	}

	public static List<SpriteC> GetSpritesByTransform(TransformC _tc)
	{
		List<SpriteC> list = new List<SpriteC>();
		Entity p_entity = _tc.p_entity;
		if (p_entity == null)
		{
			return list;
		}
		for (int i = 0; i < p_entity.m_components.Count; i++)
		{
			if (p_entity.m_components[i].m_componentType == ComponentType.Sprite && (p_entity.m_components[i] as SpriteC).p_TC == _tc)
			{
				list.Add(p_entity.m_components[i] as SpriteC);
			}
		}
		return list;
	}

	public static List<PrefabC> ConvertSpritesToPrefabComponent(TransformC _tc, bool _removeSprites)
	{
		return ConvertSpritesToPrefabComponent(_tc, null, _removeSprites);
	}

	public static List<PrefabC> ConvertSpritesToPrefabComponent(TransformC _tc, Camera _camera, bool _removeSprites)
	{
		Update();
		List<PrefabC> list = new List<PrefabC>();
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _tc.p_entity);
		List<GameObject> list2 = new List<GameObject>();
		List<SpriteSheet> list3 = new List<SpriteSheet>();
		List<List<int>> list4 = new List<List<int>>();
		if (componentsByEntity.Count > 0)
		{
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				SpriteC spriteC = componentsByEntity[i] as SpriteC;
				SpriteSheet p_spriteSheet = spriteC.p_spriteSheet;
				bool flag = false;
				int num = 0;
				if (list3.Count > 0)
				{
					for (int j = 0; j < list3.Count; j++)
					{
						if (p_spriteSheet == list3[j])
						{
							flag = true;
							num = j;
							list4[num].Add(i);
							break;
						}
					}
				}
				if (!flag || list3.Count == 0)
				{
					num = list3.Count;
					list3.Add(p_spriteSheet);
					list4.Add(new List<int>());
					list4[num].Add(i);
					list2.Add(new GameObject("ConvertedSprites"));
					if (_camera == null)
					{
						list2[num].layer = p_spriteSheet.m_camera.gameObject.layer;
					}
					else
					{
						list2[num].layer = _camera.gameObject.layer;
					}
					list2[num].AddComponent("MeshFilter");
					MeshRenderer meshRenderer = list2[num].AddComponent("MeshRenderer") as MeshRenderer;
					meshRenderer.renderer.material = p_spriteSheet.m_material;
				}
			}
			for (int k = 0; k < list3.Count; k++)
			{
				Vector3[] array = new Vector3[list4[k].Count * 4];
				Vector3[] normals = new Vector3[list4[k].Count * 4];
				Vector2[] array2 = new Vector2[list4[k].Count * 4];
				Color[] array3 = new Color[list4[k].Count * 4];
				int[] array4 = new int[list4[k].Count * 6];
				for (int l = 0; l < list4[k].Count; l++)
				{
					array4[l * 6 + 5] = l * 4;
					array4[l * 6 + 4] = l * 4 + 1;
					array4[l * 6 + 3] = l * 4 + 3;
					array4[l * 6 + 2] = l * 4 + 3;
					array4[l * 6 + 1] = l * 4 + 1;
					array4[l * 6] = l * 4 + 2;
				}
				int[] array5 = new int[list4[k].Count];
				float[] array6 = new float[list4[k].Count];
				for (int m = 0; m < list4[k].Count; m++)
				{
					array5[m] = list4[k][m];
					array6[m] = (componentsByEntity[list4[k][m]] as SpriteC).sortValue;
				}
				int[] array7 = ToolBox.sortTable(array5, array6);
				for (int n = 0; n < list4[k].Count; n++)
				{
					SpriteC spriteC2 = componentsByEntity[array7[n]] as SpriteC;
					int num2 = spriteC2.vertDataIndex * 4;
					int num3 = n * 4;
					Vector3[] vertices = list3[k].m_vertices;
					array[num3] = vertices[num2];
					array[num3 + 1] = vertices[num2 + 1];
					array[num3 + 2] = vertices[num2 + 2];
					array[num3 + 3] = vertices[num2 + 3];
					Vector2[] uVs = list3[k].m_UVs;
					array2[num3 + 3].x = uVs[num2 + 3].x;
					array2[num3 + 3].y = uVs[num2 + 3].y;
					array2[num3 + 2].x = uVs[num2 + 2].x;
					array2[num3 + 2].y = uVs[num2 + 2].y;
					array2[num3 + 1].x = uVs[num2 + 1].x;
					array2[num3 + 1].y = uVs[num2 + 1].y;
					array2[num3].x = uVs[num2].x;
					array2[num3].y = uVs[num2].y;
					Color[] colors = list3[k].m_colors;
					array3[num3] = colors[num2];
					array3[num3 + 1] = colors[num2 + 1];
					array3[num3 + 2] = colors[num2 + 2];
					array3[num3 + 3] = colors[num2 + 3];
					if (_removeSprites)
					{
						RemoveComponent(spriteC2);
					}
				}
				MeshFilter meshFilter = list2[k].gameObject.GetComponent("MeshFilter") as MeshFilter;
				meshFilter.mesh.vertices = array;
				meshFilter.mesh.triangles = array4;
				meshFilter.mesh.uv = array2;
				meshFilter.mesh.colors = array3;
				meshFilter.mesh.normals = normals;
				meshFilter.mesh.RecalculateBounds();
				list.Add(PrefabS.AddComponent(_tc, -_tc.transform.position, list2[k], "ConvertedSprites"));
				UnityEngine.Object.Destroy(list2[k]);
			}
		}
		return list;
	}

	public static GameObject CloneAsGameObject(SpriteSheet _sheet)
	{
		return UnityEngine.Object.Instantiate(_sheet.m_gameObject) as GameObject;
	}
}
