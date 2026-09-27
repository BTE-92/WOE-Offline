using UnityEngine;

public class UIEditorDrawButtonsWindow : UIVerticalList
{
	private UIRectSpriteButton m_drawAddButton;

	private UIRectSpriteButton m_drawSubButton;

	private UIRectSpriteButton m_moveArea;

	private UIText m_hintText;

	private Vector3 m_dragStartPosOffset;

	private bool m_dragging;

	public UIEditorDrawButtonsWindow(UIScrollableCanvas _parent, string _tag)
		: base(_parent, _tag)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetAlign(PsState.m_drawButtonWindowPosition, 0.5f);
		SetDepthOffset(-100f);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "DrawButtonArea");
		uIVerticalList.SetMargins(0.02f, RelativeTo.ScreenShortest);
		uIVerticalList.SetSpacing(0.02f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		m_drawAddButton = new UIRectSpriteButton(uIVerticalList, "DrawButtonAdd", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_draw_add"), false);
		m_drawAddButton.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		m_drawSubButton = new UIRectSpriteButton(uIVerticalList, "DrawButtonSub", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_draw_sub"), false);
		m_drawSubButton.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		Update();
	}

	protected void MoveHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchPhases[0] == TouchAreaPhase.Began)
		{
			m_dragStartPosOffset = m_TC.transform.position - (Vector3)_touches[0].m_currentPosition;
			m_dragging = true;
		}
		else if (m_dragging && (_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.MoveOut))
		{
			TransformS.SetPosition(m_TC, (Vector3)_touches[0].m_currentPosition + m_dragStartPosOffset);
		}
		else if (_touchPhases[0] == TouchAreaPhase.DragEnd)
		{
			m_dragging = true;
		}
	}

	public override void Step()
	{
		bool flag = false;
		bool flag2 = false;
		if (m_drawAddButton.m_began || Input.GetKeyDown(KeyCode.LeftShift))
		{
			EditorBaseState.RemoveTransformGizmo();
			if (PsState.m_currentTool == EditorTool.None)
			{
				PsState.m_currentTool = EditorTool.Paint;
			}
			PsState.m_addDown = true;
			flag = true;
			EntityManager.SetActivityOfEntitiesWithTag(new string[5] { "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonSub" }, false, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
			}
			m_hintText = new UIText(null, false, string.Empty, "Use another hand to <color=green>DRAW</color> ground", "Fonts/HurmeRegular", 0.04f, RelativeTo.ScreenShortest);
			m_hintText.SetAlign(0.5f, 1f);
			m_hintText.SetMargins(0.02f, RelativeTo.ScreenShortest);
			m_hintText.Update();
		}
		else if (m_drawAddButton.m_end || Input.GetKeyUp(KeyCode.LeftShift))
		{
			if (PsState.m_currentTool == EditorTool.Paint)
			{
				PsState.m_currentTool = EditorTool.None;
			}
			PsState.m_addDown = false;
			flag2 = true;
			EntityManager.SetActivityOfEntitiesWithTag(new string[5] { "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonSub" }, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
				m_hintText = null;
			}
		}
		if (m_drawSubButton.m_began || Input.GetKeyDown(KeyCode.LeftAlt))
		{
			EditorBaseState.RemoveTransformGizmo();
			if (PsState.m_currentTool == EditorTool.None)
			{
				PsState.m_currentTool = EditorTool.Paint;
			}
			PsState.m_subDown = true;
			flag = true;
			EntityManager.SetActivityOfEntitiesWithTag(new string[5] { "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonAdd" }, false, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
			}
			m_hintText = new UIText(null, false, string.Empty, "Use another hand to <color=red>ERASE</color> ground", "Fonts/HurmeRegular", 0.04f, RelativeTo.ScreenShortest);
			m_hintText.SetAlign(0.5f, 1f);
			m_hintText.SetMargins(0.02f, RelativeTo.ScreenShortest);
			m_hintText.Update();
		}
		else if (m_drawSubButton.m_end || Input.GetKeyUp(KeyCode.LeftAlt))
		{
			if (PsState.m_currentTool == EditorTool.Paint)
			{
				PsState.m_currentTool = EditorTool.None;
			}
			PsState.m_subDown = false;
			flag2 = true;
			EntityManager.SetActivityOfEntitiesWithTag(new string[5] { "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonAdd" }, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
				m_hintText = null;
			}
		}
		base.Step();
	}
}
