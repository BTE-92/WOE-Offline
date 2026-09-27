using UnityEngine;

public class DualButtonController : Controller
{
	private UIHorizontalList m_leftArea;

	private UIHorizontalList m_rightArea;

	public override void Open()
	{
		m_leftArea = new UIHorizontalList(null, "buttonArea");
		m_leftArea.SetAlign(0f, 0f);
		m_leftArea.SetMargins(0.04f, 0f, 0f, 0.04f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.02f, RelativeTo.ScreenShortest);
		m_leftArea.RemoveDrawHandler();
		UIRectSpriteSensor uIRectSpriteSensor = new UIRectSpriteSensor(m_leftArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_left"), false);
		uIRectSpriteSensor.SetHeight(0.175f, RelativeTo.ScreenShortest);
		uIRectSpriteSensor.SetTouchAreaSizeMultipler(1.5f);
		m_leftArea.Update();
		m_rightArea = new UIHorizontalList(null, "buttonArea");
		m_rightArea.SetAlign(1f, 0f);
		m_rightArea.SetMargins(0f, 0.04f, 0f, 0.04f, RelativeTo.ScreenShortest);
		m_rightArea.SetSpacing(0.02f, RelativeTo.ScreenShortest);
		m_rightArea.RemoveDrawHandler();
		UIRectSpriteSensor uIRectSpriteSensor2 = new UIRectSpriteSensor(m_rightArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("controller_arrow_right"), false);
		uIRectSpriteSensor2.SetHeight(0.175f, RelativeTo.ScreenShortest);
		uIRectSpriteSensor2.SetTouchAreaSizeMultipler(1.5f);
		m_rightArea.Update();
		AddButton("LeftButton", uIRectSpriteSensor, ControllerButtonType.BUTTON, KeyCode.LeftArrow);
		AddButton("RightButton", uIRectSpriteSensor2, ControllerButtonType.BUTTON, KeyCode.RightArrow);
		m_open = true;
	}

	public override void Close()
	{
		m_leftArea.Destroy();
		m_rightArea.Destroy();
		RemoveAllButtons();
		m_open = false;
	}
}
