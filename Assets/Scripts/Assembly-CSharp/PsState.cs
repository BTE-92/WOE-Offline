using System.Collections.Generic;
using UnityEngine;

public static class PsState
{
	public const string GTAG_INGAME_PARTICLES = "GTAG_INGAME_PARTICLES";

	public const string GTAG_UNIT = "GTAG_UNIT";

	public const float ZBUFFER_FIGHT_BIAS = 0.5f;

	public const int UI_SPRITE_REFERENCE_SCALE = 1536;

	public const float EDITOR_MAX_ZOOM_LEVEL = 1000f;

	public const float EDITOR_MIN_ZOOM_LEVEL = 200f;

	public const uint CP_LAYER_GROUND = 286331153u;

	public const uint CP_LAYER_UNIT = 1u;

	public const uint CP_LAYER_EFFECT = 16u;

	public static bool m_mute = false;

	public static SpriteSheet m_uiSheet;

	public static byte[] m_lastDownloadedLevelBytesZipped;

	public static string m_lastDownloadedLevelId;

	// Thumbnail captured when the creator wins a level that has no id yet (not saved/published). It is uploaded
	// as soon as that same level gets an id.
	public static byte[] m_pendingScreenshotPng;

	public static object m_pendingScreenshotLevel;

	public static void FlushPendingScreenshot(string _levelId)
	{
		byte[] png = m_pendingScreenshotPng;
		object level = m_pendingScreenshotLevel;
		m_pendingScreenshotPng = null;
		m_pendingScreenshotLevel = null;
		if (png != null && _levelId != null && object.ReferenceEquals(level, LevelManager.m_currentLevel))
		{
			Server.SaveScreenshot(_levelId, png, null);
		}
	}

	public static int m_lastSentScore;

	public static bool m_editorIsLefty;

	public static float m_drawButtonWindowPosition;

	public static float m_drawMenuAlign;

	public static float m_objectMenuButtonAlign;

	public static Vector3 m_editorCameraPos;

	public static float m_editorCameraZoom;

	public static bool m_editorPaused;

	public static bool m_gamePaused;

	public static GameState m_gameState;

	public static List<GraphElement> m_selection = new List<GraphElement>();

	public static Vector3[] m_selectionOffsets;

	public static TransformGizmo m_transformGizmo;

	public static EditorTool m_currentTool;

	public static int m_currentBrush = 0;

	public static int m_drawLayer;

	public static bool m_addDown;

	public static bool m_subDown;

	public static bool m_specialDown;

	public static bool m_playerReachedGoal;

	public static bool m_gameStarted;

	public static bool m_gameEnded;

	public static int m_gameTicks;

	public static int m_sessionBestTime;

	public static int m_sessionLongestRunTicks;
}
