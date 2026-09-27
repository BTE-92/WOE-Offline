public class UIEditorDrawMenu : UIHorizontalList
{
	public UIEditorDrawButtonsWindow m_drawWindow;

	public UIFittedSprite m_materialButton;

	public UIEditorDrawMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(0f, 0f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.02f, RelativeTo.ScreenShortest);
		RemoveTouchAreas();
		RemoveDrawHandler();
		Update();
	}

	public override void Step()
	{
		if (m_materialButton != null && m_materialButton.m_hit)
		{
			PsState.m_drawLayer = (PsState.m_drawLayer + 1) % LevelGroundNode.m_AGLayerCount;
			SwitchIconAndHighlight();
		}
		base.Step();
	}

	public void ApplyLeftySettings()
	{
		if (m_drawWindow != null)
		{
			m_drawWindow.SetHorizontalAlign(PsState.m_drawButtonWindowPosition);
			m_drawWindow.Update();
			if (PsState.m_editorIsLefty)
			{
				UIComponent uIComponent = m_childs[m_childs.Count - 1];
				if (uIComponent == m_materialButton)
				{
					m_childs.RemoveAt(m_childs.Count - 1);
					m_childs.Insert(0, m_materialButton);
				}
			}
			else
			{
				UIComponent uIComponent2 = m_childs[0];
				if (uIComponent2 == m_materialButton)
				{
					m_childs.RemoveAt(0);
					m_childs.Add(m_materialButton);
				}
			}
		}
		SetHorizontalAlign(PsState.m_drawMenuAlign);
		Update();
	}

	public void OpenDrawWindow()
	{
		EditorBaseState.RemoveTransformGizmo();
		if (m_materialButton == null)
		{
			Frame materialIconFrame = GetMaterialIconFrame();
			m_materialButton = new UIFittedSprite(this, true, "layerButton", PsState.m_uiSheet, materialIconFrame, false);
			m_materialButton.SetSize(0.2f, 0.15f, RelativeTo.ScreenHeight);
			if (PsState.m_editorIsLefty)
			{
				m_childs.RemoveAt(m_childs.Count - 1);
				m_childs.Insert(0, m_materialButton);
			}
			Update();
		}
		if (m_drawWindow == null)
		{
			m_drawWindow = new UIEditorDrawButtonsWindow(null, "DrawWindow");
		}
		SwitchIconAndHighlight();
	}

	public void CloseDrawWindow()
	{
		if (m_materialButton != null)
		{
			m_materialButton.Destroy();
			m_materialButton = null;
		}
		if (m_drawWindow != null)
		{
			m_drawWindow.Destroy();
			m_drawWindow = null;
		}
		if (AutoGeometryManager.m_layers.Count > PsState.m_drawLayer)
		{
			AutoGeometryLayer layer = AutoGeometryManager.m_layers[PsState.m_drawLayer];
			AutoGeometryManager.HighlightLayer(layer, 0f);
		}
	}

	public void SwitchIconAndHighlight()
	{
		AutoGeometryLayer layer = AutoGeometryManager.m_layers[PsState.m_drawLayer];
		AutoGeometryManager.UpdateMaxValueLookupTable(layer);
		AutoGeometryManager.HighlightLayer(layer, 0.3f);
		Frame materialIconFrame = GetMaterialIconFrame();
		SpriteS.SetFrame(PsState.m_uiSheet, m_materialButton.m_sprite, materialIconFrame);
	}

	public Frame GetMaterialIconFrame()
	{
		Frame result = null;
		if (PsState.m_drawLayer == 0)
		{
			result = PsState.m_uiSheet.m_atlas.GetFrame("hud_button_material_dirt");
		}
		else if (PsState.m_drawLayer == 1)
		{
			result = PsState.m_uiSheet.m_atlas.GetFrame("hud_button_material_ice");
		}
		else if (PsState.m_drawLayer == 2)
		{
			result = PsState.m_uiSheet.m_atlas.GetFrame("hud_button_material_mud");
		}
		else if (PsState.m_drawLayer == 3)
		{
			result = PsState.m_uiSheet.m_atlas.GetFrame("hud_button_material_metal");
		}
		else if (PsState.m_drawLayer == 4)
		{
			result = PsState.m_uiSheet.m_atlas.GetFrame("hud_button_material_danger");
		}
		return result;
	}

	public override void Destroy()
	{
		if (m_drawWindow != null)
		{
			CloseDrawWindow();
		}
		base.Destroy();
	}
}
