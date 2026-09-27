using UnityEngine;

public class Goal : Unit
{
	private const int RADIUS = 45;

	private ChipmunkBodyC m_goalCmb;

	private PrefabC m_prefabC;

	private TweenC m_tween;

	private Animator m_animator;

	private TransformC m_mainTransform;

	private ucpShape m_colShape;

	private int m_colShapeRemovedTimer;

	private GameObject m_confettiLocator;

	private uint m_playerGroup;

	public Goal(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		GameObject gameObject = ResourceManager.GetGameObject("Units/GoalBotPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position + new Vector3(0f, 0f, 50f), _graphElement.m_rotation);
		m_mainTransform = transformC;
		float num = 45f;
		m_goalCmb = ChipmunkProS.AddDynamicBody(transformC, m_colShape = new ucpCircleShape(num, Vector2.zero, 500f, 0.5f, 0.9f, (ucpCollisionType)6));
		ChipmunkProWrapper.ucpBodySetGravity(m_goalCmb.body, Vector2.zero);
		ChipmunkProWrapper.ucpBodySetLinearDamp(m_goalCmb.body, new Vector2(0.99f, 0.99f));
		m_prefabC = PrefabS.AddComponent(transformC, Vector3.zero, gameObject);
		m_prefabC.p_gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
		m_confettiLocator = m_prefabC.p_gameObject.transform.FindChild("GoalBotBody/ConfettiLocator").gameObject;
		m_animator = m_prefabC.p_gameObject.GetComponent("Animator") as Animator;
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_goalCmb, CollisionHandler, (ucpCollisionType)6, (ucpCollisionType)3, true, false, false);
		}
		m_tween = TweenS.AddTween(TweenStyle.QuadInOut, -10f, 10f, 2f, 0f);
		TweenS.SetAdditionalTweenProperties(m_tween, -1, true, TweenStyle.QuadInOut);
		EntityManager.AddComponentToEntity(m_entity, m_tween);
		m_colShapeRemovedTimer = 5;
		CreateEditorTouchArea(num, num);
	}

	public override void CreateEditorTouchArea(float _width, float _height)
	{
		if (PsState.m_gameState == GameState.Edit)
		{
			CreateGraphElementTouchArea(_width);
		}
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (!PsState.m_gameEnded)
		{
			PsState.m_playerReachedGoal = true;
			if (PsState.m_gameState == GameState.Play)
			{
				GameScene.WinGame();
			}
			else if (PsState.m_gameState == GameState.Test)
			{
				EditorScene.WinGame();
			}
			m_animator.SetTrigger("GameEnd");
			ChipmunkProS.RemoveCollisionHandler(m_goalCmb, CollisionHandler);
			SoundS.PlaySingleShot("/InGame/GameEnd", Vector3.zero);
			PrefabC prefabC = PrefabS.AddComponent(m_mainTransform, Vector3.zero, ResourceManager.GetGameObject("ParticleFx/ConfettiBurst"));
			prefabC.p_gameObject.transform.position = m_confettiLocator.transform.position;
			prefabC.p_gameObject.transform.Rotate(new Vector3(-90f, 0f, 0f), Space.Self);
			m_playerGroup = ChipmunkProWrapper.ucpShapeGetGroup(ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB].shapes[0]);
		}
	}

	public override void Update()
	{
		if ((PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play) && PsState.m_gameEnded)
		{
			m_colShapeRemovedTimer--;
			if (m_colShapeRemovedTimer == 0)
			{
				ChipmunkProWrapper.ucpShapeSetGroup(m_colShape.shapePtr, m_playerGroup);
			}
		}
		m_prefabC.p_gameObject.transform.localPosition = new Vector3(0f, m_tween.currentValue.x, 0f);
		if (Player.m_mainTransform != null)
		{
			m_mainTransform.transform.LookAt(Player.m_mainTransform.transform.position + new Vector3(0f, 0f, -200f));
			return;
		}
		LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
		if (levelPlayerNode != null)
		{
			m_mainTransform.transform.LookAt(levelPlayerNode.m_position + new Vector3(0f, 0f, -200f));
		}
	}
}
