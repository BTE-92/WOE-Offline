using UnityEngine;

public class TestItem : Item
{
	public TestItem(GraphElement _graphElement)
		: base(_graphElement, ItemType.Basic)
	{
		float num = 10f;
		Entity entity = EntityManager.AddEntity(_graphElement.m_name);
		m_assembledEntities.Add(entity);
		PsS.AddItem(entity, this);
		TransformC transformC = TransformS.AddComponent(entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		ucpShape shape = new ucpCircleShape(num, Vector2.zero, 1f, 0.5f, 0.5f, (ucpCollisionType)5);
		ChipmunkBodyC c = ChipmunkProS.AddDynamicBody(transformC, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC, Vector2.zero, num);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(c, CollisionHandler, (ucpCollisionType)5, (ucpCollisionType)3, true, false, false);
		}
		else if (PsState.m_gameState == GameState.Edit)
		{
			CreateGraphElementTouchArea(num);
		}
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		Debug.Log("item touched");
	}

	public override void Update()
	{
	}
}
