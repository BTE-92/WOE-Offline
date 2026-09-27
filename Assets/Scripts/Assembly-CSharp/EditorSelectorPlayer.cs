using System;
using UnityEngine;

public class EditorSelectorPlayer : BasicState
{
	public UIModel m_model;

	public string m_levelName;

	private UICanvas m_selector;

	private UIScrollableCanvas m_itemCanvas;

	private UITextField m_textField;

	private UISelectorNavigator m_footer;

	public override void Enter(IStatedObject _parent)
	{
		m_levelName = LevelManager.m_currentLevel.m_name;
		m_model = new UIModel(this);
		m_selector = new UICanvas(null, "SelectorContainer", null, string.Empty);
		m_selector.SetWidth(1f, RelativeTo.ScreenWidth);
		m_selector.SetHeight(1f, RelativeTo.ScreenHeight);
		m_selector.SetDepthOffset(100f);
		m_selector.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_selector, "SelectorArea");
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.RemoveTouchAreas();
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, "HeaderArea");
		uIVerticalList2.SetMargins(0.2f, 0.2f, 0f, 0f, RelativeTo.ScreenShortest);
		uIVerticalList2.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList2, "Header", "player unit", "Choose a Player Unit");
		new UISelectorNavigator(uIVerticalList2, "Navigation", new string[0], "Player Units");
		UIScrollableCanvas uIScrollableCanvas = (m_itemCanvas = new UIScrollableCanvas(uIVerticalList, "ItemArea"));
		uIScrollableCanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		uIScrollableCanvas.SetHeight(0.5f, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetMargins(0.2f, 0.2f, 0.1f, 0f, RelativeTo.ScreenShortest);
		uIScrollableCanvas.m_maxScrollInertialY = 0f;
		uIScrollableCanvas.m_maxScrollInertialX = 200f;
		uIScrollableCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIScrollableCanvas, "ItemList");
		uIHorizontalList.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.RemoveDrawHandler();
		for (int i = 0; i < PsUnitDatabase.m_categories.Count; i++)
		{
			PsUnitCategory psUnitCategory = PsUnitDatabase.m_categories[i];
			if (!psUnitCategory.categoryType.Equals("Player"))
			{
				continue;
			}
			for (int j = 0; j < psUnitCategory.items.Count; j++)
			{
				PsUnitCategoryItem psUnitCategoryItem = psUnitCategory.items[j];
				UICanvas uICanvas = new UICanvas(uIHorizontalList, psUnitCategoryItem.className, null, string.Empty);
				uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
				uICanvas.SetWidth(0.7f, RelativeTo.OwnHeight);
				uICanvas.SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
				uICanvas.SetMargins(0.1f, RelativeTo.OwnHeight);
				uICanvas.SetTouchHandler(ItemTouchHandler);
				if (psUnitCategoryItem.iconImage != null)
				{
					UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas, false, "Unit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(psUnitCategoryItem.iconImage));
					uIFittedSprite.SetVerticalAlign(1f);
				}
				UITextbox uITextbox = new UITextbox(uICanvas, false, "Unit", psUnitCategoryItem.name, "Fonts/HurmeSemiBold", 0.03f, RelativeTo.ScreenShortest, true, Align.Center, Align.Bottom);
				uITextbox.SetVerticalAlign(0f);
			}
		}
		m_selector.Update();
	}

	private void ItemTouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (!_touchIsSecondary[0] && _touchPhases[0] == TouchAreaPhase.ReleaseIn && !m_itemCanvas.m_dragged)
		{
			Debug.Log(_touchArea.m_name);
			Vector3 pos = Vector3.zero;
			Vector3 rot = Vector3.zero;
			Vector3 sca = Vector3.one;
			LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
			if (levelPlayerNode != null)
			{
				pos = levelPlayerNode.m_position;
				rot = levelPlayerNode.m_rotation;
				sca = levelPlayerNode.m_scale;
				LevelManager.m_currentLevel.m_currentLayer.RemoveElement(levelPlayerNode);
				levelPlayerNode.Dispose();
			}
			GraphElement graphElement = null;
			string name = _touchArea.m_name;
			graphElement = new LevelPlayerNode(Type.GetType(name), "Player", pos, rot, sca);
			LevelManager.m_currentLevel.m_currentLayer.AddElement(graphElement);
			graphElement.Assemble();
			if (graphElement != null)
			{
				EditorBaseState.CreateTransformGizmo(graphElement);
			}
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		if (m_selector != null)
		{
			m_selector.Destroy();
			m_selector = null;
		}
	}
}
