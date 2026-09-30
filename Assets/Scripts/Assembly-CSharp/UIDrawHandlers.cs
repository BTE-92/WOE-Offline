using System.Collections.Generic;
using UnityEngine;

public class UIDrawHandlers
{
	public static void EditorPopupBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero, false);
		Color color = DebugDraw.GetColor(0f, 0f, 0f, 50f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * _c.m_TC.transform.position.z, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
	}

	public static void EditorPopupContentArea(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] array = new Vector2[8]
		{
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * -0.191f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.191f),
			new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * 0.191f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.191f)
		};
		DebugDraw.AddRandom(array, _c.m_actualHeight * 0.025f);
		Color color = DebugDraw.GetColor(0f, 60f, 109f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, array, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, array, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		Color color2 = DebugDraw.GetColor(0f, 0f, 0f, 100f);
		uint num2 = DebugDraw.ColorToUInt(color2);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, new Vector3(0.01f * (float)Screen.height, -0.01f * (float)Screen.height, 5f), array, num2, num2, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
	}

	public static void ScrollableList(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth * 0.95f, _c.m_actualHeight, Vector2.zero, false);
		Color color = DebugDraw.GetColor(0f, 24f, 71f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
	}

	public static void Textfield(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero, false);
		DebugDraw.AddRandom(rect, (float)Screen.height * 0.01f);
		Color color = DebugDraw.GetColor(255f, 255f, 255f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
	}

	public static void PositiveButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero, false);
		DebugDraw.AddRandom(rect, _c.m_actualHeight * 0.05f);
		Color color = DebugDraw.GetColor(98f, 216f, 15f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
        Color color2 = DebugDraw.GetColor(0f, 0f, 0f, 100f); // add shadow sorting logic to fix visual bug. Non existent in unity 4
        uint num2 = DebugDraw.ColorToUInt(color2);

        List<PrefabC> shadow = PrefabS.CreateFlatPrefabComponentsFromVectorArray(
            _c.m_TC,
            new Vector3(0.01f * (float)Screen.height, -0.01f * (float)Screen.height),
            rect, num2, num2,
            ResourceManager.GetMaterial("Framework/SolidMat"),
            camera, string.Empty);

        for (int i = 0; i < shadow.Count; i++)
        {
            shadow[i].p_gameObject.GetComponent<Renderer>().sortingOrder = -1;
        }
    }

	public static void NegativeButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero, false);
		DebugDraw.AddRandom(rect, _c.m_actualHeight * 0.05f);
		Color color = DebugDraw.GetColor(234f, 70f, 49f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
        Color color2 = DebugDraw.GetColor(0f, 0f, 0f, 100f);
        uint num2 = DebugDraw.ColorToUInt(color2);

        List<PrefabC> shadow = PrefabS.CreateFlatPrefabComponentsFromVectorArray(
            _c.m_TC,
            new Vector3(0.01f * (float)Screen.height, -0.01f * (float)Screen.height),
            rect, num2, num2,
            ResourceManager.GetMaterial("Framework/SolidMat"),
            camera, string.Empty);

        for (int i = 0; i < shadow.Count; i++)
        {
            shadow[i].p_gameObject.GetComponent<Renderer>().sortingOrder = -1;
        }
    }

	public static void NeutralButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero, false);
		DebugDraw.AddRandom(rect, _c.m_actualHeight * 0.05f);
		Color color = DebugDraw.GetColor(220f, 220f, 220f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, rect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
        Color color2 = DebugDraw.GetColor(0f, 0f, 0f, 100f);
        uint num2 = DebugDraw.ColorToUInt(color2);

        List<PrefabC> shadow = PrefabS.CreateFlatPrefabComponentsFromVectorArray(
            _c.m_TC,
            new Vector3(0.01f * (float)Screen.height, -0.01f * (float)Screen.height),
            rect, num2, num2,
            ResourceManager.GetMaterial("Framework/SolidMat"),
            camera, string.Empty);

        for (int i = 0; i < shadow.Count; i++)
        {
            shadow[i].p_gameObject.GetComponent<Renderer>().sortingOrder = -1;
        }
    }

	public static void SelectorCancelButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] points = new Vector2[6]
		{
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f - _c.m_actualHeight * 0.318f * 0.5f, 0f),
			new Vector2(_c.m_actualWidth * 0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f - _c.m_actualHeight * 0.318f * 0.5f, 0f)
		};
		Color color = DebugDraw.GetColor(234f, 70f, 49f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		Vector3 vector = Vector3.zero;
		if (_c.m_highlight)
		{
			vector = Vector3.up * -0.02f * Screen.height;
			(_c as UITextButton).m_tmc.m_go.transform.localPosition = vector;
		}
		else
		{
			(_c as UITextButton).m_tmc.m_go.transform.localPosition = Vector3.zero;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero + vector, points, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 1f + vector, points, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		if (!_c.m_highlight)
		{
			Color color2 = DebugDraw.GetColor(43f, 60f, 56f);
			uint num2 = DebugDraw.ColorToUInt(color2);
			PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, new Vector3(0f, -0.02f * (float)Screen.height, 5f), points, num2, num2, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
			PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(0f, -0.02f * (float)Screen.height, 6f), points, 4f, color2, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		}
	}

	public static void SelectorCategoryButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] points = new Vector2[6]
		{
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f - _c.m_actualHeight * 0.318f * 0.5f, 0f),
			new Vector2(_c.m_actualWidth * 0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f - _c.m_actualHeight * 0.318f * 0.5f, 0f)
		};
		Color color = DebugDraw.GetColor(0f, 112f, 178f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		Vector3 vector = Vector3.zero;
		if (_c.m_highlight)
		{
			vector = Vector3.up * -0.02f * Screen.height;
			(_c as UITextButton).m_tmc.m_go.transform.localPosition = vector;
		}
		else
		{
			(_c as UITextButton).m_tmc.m_go.transform.localPosition = Vector3.zero;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero + vector, points, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 1f + vector, points, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		if (!_c.m_highlight)
		{
			Color color2 = DebugDraw.GetColor(43f, 60f, 56f);
			uint num2 = DebugDraw.ColorToUInt(color2);
			PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, new Vector3(0f, -0.02f * (float)Screen.height, 5f), points, num2, num2, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
			PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(0f, -0.02f * (float)Screen.height, 6f), points, 4f, color2, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		}
	}

	public static void SelectedCategoryButton(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] points = new Vector2[6]
		{
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f - _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * 0.5f),
			new Vector2(_c.m_actualWidth * 0.5f + _c.m_actualHeight * 0.318f * 0.5f, 0f),
			new Vector2(_c.m_actualWidth * 0.5f - _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f + _c.m_actualHeight * 0.318f * 0.5f, _c.m_actualHeight * -0.5f),
			new Vector2(_c.m_actualWidth * -0.5f - _c.m_actualHeight * 0.318f * 0.5f, 0f)
		};
		Color color = DebugDraw.GetColor(0f, 112f, 178f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, new Vector3(0f, 0f, 5f), points, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(0f, 0f, 6f), points, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
	}

	public static void EditorFileMenuContentArea(UIComponent _c)
	{
		SpriteC spriteC = EntityManager.GetComponentByIdentifier(_c.m_TC.p_entity, 1) as SpriteC;
		if (spriteC == null)
		{
			spriteC = SpriteS.AddComponent(_c.m_TC, PsState.m_uiSheet.m_atlas.GetFrame("hud_dropdown_background"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC, _c.m_actualWidth, _c.m_actualHeight);
			spriteC.m_identifier = 1;
		}
		else
		{
			SpriteS.SetDimensions(spriteC, _c.m_actualWidth, _c.m_actualHeight);
		}
	}

	public static void DebugRect(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		DebugDraw.CreateBox(_c.m_camera, _c.m_TC, Vector2.zero, _c.m_actualWidth, _c.m_actualHeight);
		DebugDraw.CreateBox(_c.m_camera, _c.m_TC, new Vector2(_c.m_actualMargins.l - _c.m_actualMargins.r, _c.m_actualMargins.b - _c.m_actualMargins.t) * 0.5f, _c.m_actualWidth - _c.m_actualMargins.l - _c.m_actualMargins.r, _c.m_actualHeight - _c.m_actualMargins.b - _c.m_actualMargins.t);
		if (_c.m_highlight)
		{
			SpriteS.SetColorByTransformComponent(_c.m_TC, Color.green);
		}
		else if (_c.m_highlightSecondary)
		{
			SpriteS.SetColorByTransformComponent(_c.m_TC, Color.yellow);
		}
		else if (_c.m_hit)
		{
			SpriteS.SetColorByTransformComponent(_c.m_TC, Color.cyan);
		}
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		SpriteS.ConvertSpritesToPrefabComponent(_c.m_TC, camera, true);
	}
}
