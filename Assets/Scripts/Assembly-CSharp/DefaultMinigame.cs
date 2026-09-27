using UnityEngine;

public static class DefaultMinigame
{
	public static void Assemble()
	{
		Minigame minigame = LevelManager.NewLevel(new Minigame(16384, 2048)) as Minigame;
		minigame.m_fileName = "TestMinigame";
		minigame.m_name = "MyMinigame";
		minigame.m_description = "Minigame Description";
		minigame.m_groundNode = new LevelGroundNode();
		LevelManager.m_currentLevel.m_currentLayer.AddElement(minigame.m_groundNode);
		Vector2 vector = new Vector2(0f, (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.45f);
		LevelPlayerNode element = new LevelPlayerNode(typeof(OffroadCar), "Player", vector + Vector2.right * -500f, Vector3.zero, Vector3.one);
		SimpleLevelNode element2 = new SimpleLevelNode(typeof(Goal), "Goal", vector + Vector2.right * 500f, Vector3.zero, Vector3.one);
		LevelManager.m_currentLevel.m_currentLayer.AddElement(element);
		LevelManager.m_currentLevel.m_currentLayer.AddElement(element2);
		minigame.m_settings.Add("environment", "default");
		LevelManager.AssembleCurrentLevel();
		minigame.ApplySettings();
		PsState.m_editorCameraPos = vector + Vector2.up * 200f;
		PsState.m_editorCameraZoom = 500f;
	}
}
