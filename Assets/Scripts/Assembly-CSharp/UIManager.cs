using System.Collections.Generic;

public static class UIManager
{
	public static List<UIComponent> m_uiComponents = new List<UIComponent>();

	public static UICanvas m_canvas;

	public static void Update()
	{
		for (int i = 0; i < m_uiComponents.Count; i++)
		{
			m_uiComponents[i].Step();
		}
	}

	public static void DestroyUI()
	{
		while (m_uiComponents.Count > 0)
		{
			int index = m_uiComponents.Count - 1;
			m_uiComponents[index].Destroy(false);
			m_uiComponents.RemoveAt(index);
		}
	}

	public static void ScreenSizeChanged()
	{
		for (int i = 0; i < m_uiComponents.Count; i++)
		{
			m_uiComponents[i].Update();
		}
	}
}
