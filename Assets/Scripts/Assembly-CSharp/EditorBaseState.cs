using System.Collections.Generic;
using UnityEngine;

public class EditorBaseState : BasicState
{
	private const int BRUSH_Z_OFFSET = 90;

	private Vector2 brushXYOffset = new Vector2(16f, 16f);

	public UIEditorMinigameMenu m_minigameMenu;

	public UIEditorMenu m_editorMenu;

	public UIEditorObjectMenu m_objectMenu;

	public UIEditorDrawMenu m_drawMenu;

	public UIEditorDrawButtonsWindow m_drawWindow;

	public static CameraTargetC m_camTarget;

	private TouchAreaC m_fullscreenTAC;

	private float m_startDistance;

	private float m_startDistanceMultipler;

	private Vector2 m_startZoom;

	private bool m_pinching;

	private bool m_drawing;

	private Vector2 m_brushPos;

	private AutoGeometryBrush m_dynamicBrush;

	private AutoGeometryBrush m_dynamicMaskBrush;

	private float m_lastBrushCameraDistance;

	private int m_lastBrushGroundIndex;

	private TransformC m_brushVisualTC;

	private SoundC m_brushSound;

	private float m_brushSoundVelocity;

	private float m_currentZoomPos;

	public EditorBaseState()
	{
		PsState.m_gameState = GameState.Edit;
		PsState.m_editorPaused = true;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_startZoom = Vector2.zero;
		m_startDistance = 0f;
		m_pinching = false;
		m_lastBrushGroundIndex = -1;
		Entity entity = EntityManager.AddEntity();
		TransformC tc = TransformS.AddComponent(entity, "Editor Camera Target");
		m_camTarget = CameraS.AddTargetComponent(tc, 1200f, 1200f);
		m_camTarget.maxAngleChange = new Vector2(0f, 0f);
		m_camTarget.maxScaleChange = 0f;
		m_camTarget.maxOffsetChange = 0f;
		CameraS.m_mainCameraMaxVelocity = 9999f;
		m_camTarget.TC.transform.position = PsState.m_editorCameraPos;
		CameraS.ResetMainCamera(PsState.m_editorCameraPos, 500f);
		SetZoomAmount(PsState.m_editorCameraZoom);
		entity = EntityManager.AddEntity();
		tc = TransformS.AddComponent(entity, Vector3.forward * 200f);
		tc.transform.name = "EditorCanvasTransform";
		float num = Mathf.Max(Screen.width, Screen.height);
		m_fullscreenTAC = TouchAreaS.AddRectArea(tc, "first", num, num, CameraS.m_uiCamera);
		TouchAreaS.AddTouchEventListener(m_fullscreenTAC, TouchHandler);
		m_fullscreenTAC.m_allowSecondary = false;
		m_fullscreenTAC.m_maxTouches = 2;
		DebugDraw.CreateBox(CameraS.m_mainCamera, tc, Vector2.zero, AutoGeometryManager.m_width, AutoGeometryManager.m_height);
		m_minigameMenu = new UIEditorMinigameMenu(null, "MinigameMenu");
		m_editorMenu = new UIEditorMenu(null, "EditorMenu");
		m_objectMenu = new UIEditorObjectMenu(null, "ObjectMenu");
		m_drawMenu = new UIEditorDrawMenu(null, "DrawMenu");
		ApplyLeftySettings();
		m_drawMenu.OpenDrawWindow();
	}

	public override void Execute()
	{
		if (Input.GetKey(KeyCode.A))
		{
			SetZoomAmount(PsState.m_editorCameraZoom - 25f);
		}
		else if (Input.GetKey(KeyCode.Z))
		{
			SetZoomAmount(PsState.m_editorCameraZoom + 25f);
		}
		if (PsState.m_currentTool != EditorTool.None)
		{
			return;
		}
		if (m_editorMenu.m_playButton.m_hit)
		{
			EntityManager.RemoveEntity(m_camTarget.p_entity);
			m_camTarget = null;
			GraphElement element = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player");
			if (element != null)
			{
				CameraS.ResetMainCamera(element.m_position, 500f);
			}
			RemoveTransformGizmo();
			UndoManager.Purge();
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorTestState());
			(LevelManager.m_currentLevel as Minigame).m_groundNode.SaveGroundBeforePlay();
			LevelManager.SaveCurrentLevel();
			LevelManager.ResetCurrentLevel();
			EditorScene.InitGame();
			Player.OpenController();
			Player.SetAsAudioListener();
		}
		else if (m_editorMenu.m_fileMenuButton != null)
		{
			if (m_editorMenu.m_fileMenuButton.m_hit)
			{
				m_editorMenu.OpenFileMenu();
			}
		}
		else if (m_editorMenu.m_fileMenu != null)
		{
			if (m_editorMenu.m_fileMenu.m_menuButton.m_hit)
			{
				m_editorMenu.CloseFileMenu();
			}
			else if (m_editorMenu.m_fileMenu.m_newButton.m_hit)
			{
				RemoveTransformGizmo();
				UndoManager.Purge();
				PsState.m_lastDownloadedLevelBytesZipped = null;
				PsState.m_lastDownloadedLevelId = null;
				DefaultMinigame.Assemble();
				m_editorMenu.CloseFileMenu();
			}
			else if (m_editorMenu.m_fileMenu.m_saveButton.m_hit)
			{
				RemoveTransformGizmo();
				UndoManager.Purge();
				Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSavePopupState());
			}
			else if (m_editorMenu.m_fileMenu.m_exitButton.m_hit)
			{
				RemoveTransformGizmo();
				UndoManager.Purge();
				Main.m_currentGame.m_sceneManager.ChangeScene(new MenuScene("MenuScene"), new FadeLoadingScene(Color.black));
			}
		}
	}

	public static void CreateTransformGizmo(GraphElement _graphElement)
	{
		if (PsState.m_transformGizmo != null)
		{
			RemoveTransformGizmo();
		}
		PsState.m_selection.Add(_graphElement);
		_graphElement.m_selected = true;
		if (PsState.m_transformGizmo == null)
		{
			PsState.m_transformGizmo = new TransformGizmo(true);
		}
	}

	public static void RemoveTransformGizmo()
	{
		if (PsState.m_transformGizmo != null)
		{
			PsState.m_transformGizmo.Destroy();
		}
	}

	private void SetZoomAmount(float _amount)
	{
		float min = 200f;
		float max = 1000f;
		PsState.m_editorCameraZoom = ToolBox.limitBetween(_amount, min, max);
		m_currentZoomPos = ToolBox.getPositionBetween(_amount, min, max);
		cpBB bb = new cpBB
		{
			r = PsState.m_editorCameraZoom,
			l = 0f - PsState.m_editorCameraZoom,
			t = PsState.m_editorCameraZoom,
			b = 0f - PsState.m_editorCameraZoom
		};
		m_camTarget.bb = bb;
	}

	public void ApplyLeftySettings()
	{
		if (PsState.m_editorIsLefty)
		{
			PsState.m_drawButtonWindowPosition = 0.975f;
			PsState.m_drawMenuAlign = 1f;
			PsState.m_objectMenuButtonAlign = 0f;
		}
		else
		{
			PsState.m_drawButtonWindowPosition = 0.025f;
			PsState.m_drawMenuAlign = 0f;
			PsState.m_objectMenuButtonAlign = 1f;
		}
		m_drawMenu.ApplyLeftySettings();
		m_objectMenu.SetHorizontalAlign(PsState.m_objectMenuButtonAlign);
		m_objectMenu.Update();
	}

	private void TouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchIsSecondary[0])
		{
			return;
		}
		m_editorMenu.CloseFileMenu();
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		AutoGeometryLayer autoGeometryLayer = minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer];
		if (PsState.m_currentTool == EditorTool.Camera)
		{
			switch (_touchCount)
			{
			case 1:
				if (_touchArea.m_wasDragged)
				{
					Vector3 position2 = m_camTarget.TC.transform.position;
					position2 -= (Vector3)_touches[0].m_deltaPosition / CameraS.m_mainCameraDistanceMultipler;
					position2.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f, position2.x));
					position2.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, position2.y));
					m_camTarget.TC.transform.position = position2;
					PsState.m_editorCameraPos = position2;
				}
				if (_touchPhases[0] == TouchAreaPhase.ReleaseIn)
				{
					PsState.m_currentTool = EditorTool.None;
					m_pinching = false;
				}
				break;
			case 2:
				if (!_touchIsSecondary[1])
				{
					Vector3 vector = (_touches[0].m_deltaPosition + _touches[1].m_deltaPosition) * 0.5f;
					Vector2 vector2 = _touches[0].m_currentPosition - _touches[1].m_currentPosition;
					if (_touchPhases[0] == TouchAreaPhase.Began || _touchPhases[1] == TouchAreaPhase.Began)
					{
						cpBB bb = m_camTarget.bb;
						m_startZoom = new Vector2(bb.r - bb.l, bb.t - bb.b);
						m_startDistance = (_touches[0].m_currentPosition - _touches[1].m_currentPosition).magnitude;
						m_startDistanceMultipler = CameraS.m_mainCameraDistanceMultipler;
						m_pinching = true;
					}
					if (m_pinching)
					{
						float num = (vector2.magnitude - m_startDistance) * 2f / m_startDistanceMultipler;
						float zoomAmount = (m_startZoom.x - num) * 0.5f;
						SetZoomAmount(zoomAmount);
					}
					if (_touchArea.m_wasDragged)
					{
						Vector3 position = m_camTarget.TC.transform.position;
						position -= vector / CameraS.m_mainCameraDistanceMultipler;
						position.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f, position.x));
						position.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, position.y));
						m_camTarget.TC.transform.position = position;
						PsState.m_editorCameraPos = position;
					}
					if (_touchPhases[0] == TouchAreaPhase.ReleaseIn && _touchPhases[1] == TouchAreaPhase.ReleaseIn)
					{
						PsState.m_currentTool = EditorTool.None;
						m_pinching = false;
					}
				}
				break;
			}
		}
		else if (PsState.m_currentTool == EditorTool.Paint)
		{
			Vector2 currentPosition = _touches[0].m_currentPosition;
			if (_touchPhases[0] == TouchAreaPhase.Began || !m_drawing)
			{
				m_drawing = true;
				m_brushPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, currentPosition, 90f) + (Vector3)brushXYOffset;
				autoGeometryLayer.TakeSnapshot();
				autoGeometryLayer.ResetUndoRect();
				SoundS.PlaySingleShot("/UI/DrawBegin", Vector3.zero);
				m_brushSoundVelocity = 0f;
				Debug.Log("Draw start");
			}
			PaintWithBrush(currentPosition, 0.5f, true);
			if (_touchPhases[0] == TouchAreaPhase.ReleaseIn || _touchPhases[0] == TouchAreaPhase.ReleaseOut)
			{
				if (autoGeometryLayer.HasUndoRect())
				{
					new DrawUndoAction(autoGeometryLayer);
				}
				m_drawing = false;
				EntityManager.RemoveEntity(m_brushVisualTC.p_entity);
				m_brushVisualTC = null;
				SoundS.PlaySingleShot("/UI/DrawEnd", Vector3.zero);
				Debug.Log("Draw stop");
			}
		}
		else
		{
			if (PsState.m_currentTool != EditorTool.None)
			{
				return;
			}
			if (m_drawing)
			{
				m_drawing = false;
				DebugDraw.Clear(CameraS.m_mainCamera, AutoGeometryManager.m_debugDrawTC);
				if (autoGeometryLayer.HasUndoRect())
				{
					new DrawUndoAction(autoGeometryLayer);
				}
				EntityManager.RemoveEntity(m_brushVisualTC.p_entity);
				m_brushVisualTC = null;
				SoundS.PlaySingleShot("/UI/DrawEnd", Vector3.zero);
				Debug.Log("Draw stop");
			}
			switch (_touchCount)
			{
			case 2:
				if (!_touchIsSecondary[1] && (_touchPhases[0] == TouchAreaPhase.Began || _touchPhases[1] == TouchAreaPhase.Began))
				{
					PsState.m_currentTool = EditorTool.Camera;
					cpBB bb2 = m_camTarget.bb;
					m_startZoom = new Vector2(bb2.r - bb2.l, bb2.t - bb2.b);
					m_startDistance = (_touches[0].m_currentPosition - _touches[1].m_currentPosition).magnitude;
					m_startDistanceMultipler = CameraS.m_mainCameraDistanceMultipler;
					m_pinching = true;
				}
				break;
			case 1:
				if (_touchPhases[0] == TouchAreaPhase.DragStart)
				{
					PsState.m_currentTool = EditorTool.Camera;
				}
				else
				{
					if (_touchPhases[0] != TouchAreaPhase.ReleaseIn || _touchArea.m_wasDragged)
					{
						break;
					}
					if (PsState.m_transformGizmo != null)
					{
						List<GraphElement> oldElements = new List<GraphElement>(PsState.m_selection.ToArray());
						PsState.m_transformGizmo.Destroy();
						new SelectUndoAction(oldElements, PsState.m_selection);
					}
					float radius = 25f * (1f / CameraS.m_mainCameraDistanceMultipler);
					Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
					Vector3 position3 = CameraS.m_mainCamera.transform.position;
					Vector3 vector3 = touchWorldPos - position3;
					RaycastHit[] array = null;
					array = Physics.SphereCastAll(touchWorldPos - vector3.normalized * 500f, radius, vector3.normalized, 1000f, 1 << CameraS.m_mainCamera.gameObject.layer);
					TouchAreaC touchAreaC = null;
					float num2 = 999999f;
					for (int i = 0; i < array.Length; i++)
					{
						RaycastHit raycastHit = array[i];
						float magnitude = (touchWorldPos - raycastHit.transform.position).magnitude;
						if (magnitude < num2)
						{
							num2 = magnitude;
							TouchAreaBootstrap touchAreaBootstrap = raycastHit.transform.GetComponent("TouchAreaBootstrap") as TouchAreaBootstrap;
							touchAreaC = touchAreaBootstrap.m_TAC;
						}
					}
					if (touchAreaC != null)
					{
						touchAreaC.m_touchCount = 1;
						_touches[0].m_primaryArea = touchAreaC;
						touchAreaC.d_TouchEventDelegate(touchAreaC, 1, _touches, _touchPhases, _touchIsSecondary);
						_touchArea.m_touchCount = 0;
						break;
					}
					int layerAtWorldPos = AutoGeometryManager.GetLayerAtWorldPos(TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition, 90f) + (Vector3)brushXYOffset * 0.5f);
					if (layerAtWorldPos < 0)
					{
						layerAtWorldPos = AutoGeometryManager.GetLayerAtWorldPos(TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition, -210f) + (Vector3)brushXYOffset * 0.5f);
					}
					if (layerAtWorldPos >= 0)
					{
						PsState.m_drawLayer = layerAtWorldPos;
						m_drawMenu.OpenDrawWindow();
					}
				}
				break;
			}
		}
	}

	private void PaintWithBrush(Vector2 _touchPos, float _speed, bool _soften)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		bool rectBrush = minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].m_groundC.m_ground.m_rectBrush;
		if (Mathf.Abs(m_lastBrushCameraDistance - CameraS.m_mainCameraDistanceMultipler) > 0.01f || m_lastBrushGroundIndex != PsState.m_drawLayer)
		{
			m_lastBrushCameraDistance = CameraS.m_mainCameraDistanceMultipler;
			m_lastBrushGroundIndex = PsState.m_drawLayer;
			float brushSize = (rectBrush ? Mathf.Lerp(1.5f, 6.25f, m_currentZoomPos) : Mathf.Lerp(2.5f, 7.5f, m_currentZoomPos));
			m_dynamicBrush = new AutoGeometryBrush(brushSize, rectBrush);
		}
		float maxStep = m_dynamicBrush.m_width;
		Vector2 vector = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touchPos, 90f);
		Vector2 vector2 = vector + brushXYOffset - m_brushPos;
		float max = 200f * _speed;
		vector2 = vector2.normalized * ToolBox.limitBetween(vector2.magnitude, 0f, max);
		if (m_brushVisualTC == null)
		{
			Entity entity = EntityManager.AddEntity();
			m_brushVisualTC = TransformS.AddComponent(entity);
			Vector2[] array = null;
			PrefabS.CreatePathPrefabComponentFromVectorArray(_points: (!rectBrush) ? DebugDraw.GetCircle(m_dynamicBrush.m_width * 8, 36, Vector2.zero, false) : DebugDraw.GetRoundedRect(m_dynamicBrush.m_width * 16, m_dynamicBrush.m_width * 16, 8f, 2, Vector2.zero, false), _tc: m_brushVisualTC, _offset: Vector3.zero, _width: 8f, _color: Color.white, _material: ResourceManager.GetMaterial("Framework/Line8Mat"), _camera: CameraS.m_mainCamera, _align: Position.Center, _closed: true);
			m_brushSound = SoundS.AddComponent(_event: (!PsState.m_addDown) ? "/UI/EraseLoop" : minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].m_groundC.m_ground.m_drawSound, _parentTC: m_brushVisualTC);
			SoundS.SetSoundParameter(m_brushSound, "Velocity", 0f);
			SoundS.PlaySound(m_brushSound);
		}
		else
		{
			float positionBetween = ToolBox.getPositionBetween(vector2.magnitude, 0.1f, 20f);
			m_brushSoundVelocity += (positionBetween - m_brushSoundVelocity) * 0.25f;
			SoundS.SetSoundParameter(m_brushSound, "Velocity", m_brushSoundVelocity);
		}
		TransformS.SetPosition(m_brushVisualTC, (Vector3)vector + Vector3.forward * -90f);
		if (vector2.magnitude > 1f)
		{
			m_brushPos = PaintLine(m_brushPos, m_brushPos + vector2, maxStep, _soften);
		}
	}

	private Vector2 PaintLine(Vector2 _startPos, Vector2 _endPos, float _maxStep, bool _soften)
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		Vector2 vector = _endPos - _startPos;
		Vector2 vector2 = _startPos;
		int num = Mathf.FloorToInt(1f + vector.magnitude / _maxStep);
		Vector2 vector3 = vector / num;
		for (int i = 0; i < num; i++)
		{
			vector2 += vector3;
			AutoGeometryManager.PaintWithBrush(minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer], m_dynamicBrush, vector2, PsState.m_addDown, m_dynamicBrush.m_subPixelAccuracy);
		}
		minigame.m_groundNode.m_AGLayer[PsState.m_drawLayer].UpdateSegments();
		return vector2;
	}

	public override void Exit()
	{
		if (m_camTarget != null)
		{
			EntityManager.RemoveEntity(m_camTarget.p_entity);
			m_camTarget = null;
		}
		EntityManager.RemoveEntity(m_fullscreenTAC.p_entity);
		m_fullscreenTAC = null;
		m_minigameMenu.Destroy();
		m_editorMenu.Destroy();
		m_objectMenu.Destroy();
		m_drawMenu.Destroy();
		if (m_drawWindow != null)
		{
			m_drawWindow.Destroy();
		}
	}
}
