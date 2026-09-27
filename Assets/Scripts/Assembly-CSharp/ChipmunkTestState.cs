using UnityEngine;

public class ChipmunkTestState : BasicState
{
	private SpriteSheet m_spriteSheet;

	private int ticker;

	public void createSprite(string name)
	{
		float num = Random.Range(4, 10);
		Entity entity = EntityManager.AddEntity(name);
		TransformC transformC = TransformS.AddComponent(entity, name);
		SpriteC spriteC = SpriteS.AddComponent(transformC, new Frame(0f, 0f, 5f, 5f), m_spriteSheet);
		SpriteS.SetDimensions(spriteC, num * 1f, num * 1f);
		SpriteS.SetColor(spriteC, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
		TransformS.SetPosition(transformC, new Vector3(Random.Range(-50, 50), Random.Range(100, 250), 0f));
		ucpPolyShape ucpPolyShape2 = new ucpPolyShape(num, num, Vector2.zero, 1f);
		ucpPolyShape2.collisionType = ucpCollisionType.Any;
	}

	private void HandleEvent(EventC _event)
	{
		EntityManager.RemoveEntity(_event.p_entity);
	}

	public override void Enter(IStatedObject _parent)
	{
		m_spriteSheet = FrameworkTestScene.m_spriteSheet;
		ticker = 0;
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "Chipmunk PRO Entities");
		Entity entity = EntityManager.AddEntity();
		TransformC transformComponent = TransformS.AddComponent(entity, "walls");
		ucpPolyShape ucpPolyShape2 = new ucpPolyShape(150f, 10f, Vector2.up * -75f);
		ucpPolyShape ucpPolyShape3 = new ucpPolyShape(10f, 150f, Vector2.right * -75f);
		ucpPolyShape ucpPolyShape4 = new ucpPolyShape(10f, 150f, Vector2.right * 75f);
		ucpShape[] shapes = new ucpShape[3] { ucpPolyShape2, ucpPolyShape3, ucpPolyShape4 };
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddStaticBody(transformComponent, shapes);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformComponent, Vector2.up * -75f, 150f, 10f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformComponent, Vector2.right * -75f, 10f, 150f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformComponent, Vector2.right * 75f, 10f, 150f);
		entity = EntityManager.AddEntity();
		transformComponent = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformComponent, Vector3.up * -75f + Vector3.right * -75f);
		ucpCircleShape shape = new ucpCircleShape(20f, Vector2.zero, 10f, 0f, 0f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddRogueBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 20f);
		TweenC c = TweenS.AddTransformTween(transformComponent, TweenedProperty.Position, TweenStyle.CubicInOut, Vector3.up * -75f + Vector3.right * 75f, 2f, 0f);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
		entity = EntityManager.AddEntity();
		transformComponent = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformComponent, Vector3.up * -20f);
		TransformS.SetRotation(transformComponent, Vector3.forward * -359f);
		ucpPolyShape shape2 = new ucpPolyShape(50f, 10f, Vector2.zero, 10f, 0.75f, 1f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddRogueBody(transformComponent, shape2);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformComponent, Vector2.zero, 50f, 10f);
		c = TweenS.AddTransformTween(transformComponent, TweenedProperty.Position, TweenStyle.CubicInOut, Vector3.up * 20f, 1.5f, 0f);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
		c = TweenS.AddTransformTween(transformComponent, TweenedProperty.Rotation, TweenStyle.QuadInOut, Vector3.forward * 359f, 5f, 0f);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadInOut);
		c = TweenS.AddTransformTween(transformComponent, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.one * 2f, 0.5f, 0f);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicOut);
		entity = EntityManager.AddEntity();
		transformComponent = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformComponent, Vector3.up * 50f + Vector3.right * 5f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)3);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 100f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		TransformC anchor = TransformS.AddComponent(entity, "PinJointAnchor1", transformComponent.transform.position + Vector3.up * 100f);
		TransformC anchor2 = TransformS.AddComponent(entity, "PinJointAnchor2", transformComponent.transform.position + Vector3.up * 5f);
		ChipmunkProS.AddPinJoint(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor, anchor2);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 125f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		anchor = TransformS.AddComponent(entity, "PivotJointAnchor1", transformComponent.transform.position + Vector3.up * 5f);
		ChipmunkProS.AddPivotJoint(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 150f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		anchor = TransformS.AddComponent(entity, "PivotJointAnchor1", transformComponent.transform.position + Vector3.up * 5f);
		anchor2 = TransformS.AddComponent(entity, "PivotJointAnchor2", transformComponent.transform.position + Vector3.up * 5f);
		ChipmunkProS.AddPivotJoint2(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor, anchor2);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 175f + Vector3.up * 150f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		anchor = TransformS.AddComponent(entity, "SlideJointAnchor1", transformComponent.transform.position + Vector3.up * -50f);
		anchor2 = TransformS.AddComponent(entity, "SlideJointAnchor2", transformComponent.transform.position + Vector3.up * -5f);
		ChipmunkProS.AddSlideJoint(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor, anchor2, 25f, 50f);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 200f + Vector3.up * 100f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		anchor = TransformS.AddComponent(entity, "GrooveJointGrooveA", transformComponent.transform.position);
		anchor2 = TransformS.AddComponent(entity, "GrooveJointGrooveB", transformComponent.transform.position + Vector3.up * -100f);
		TransformC anchor3 = TransformS.AddComponent(entity, "GrooveJointAnchor", transformComponent.transform.position);
		ChipmunkProS.AddGrooveJoint(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor, anchor2, anchor3);
		entity = EntityManager.AddEntity("constraintTest");
		transformComponent = TransformS.AddComponent(entity, Vector3.right * 225f + Vector3.up * 100f);
		shape = new ucpCircleShape(10f, Vector2.zero, 10f, 0.5f, 0.5f, (ucpCollisionType)2);
		chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformComponent, shape);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformComponent, Vector2.zero, 10f);
		entity = EntityManager.AddEntity();
		anchor = TransformS.AddComponent(entity, "DampedSpringAnchor1", transformComponent.transform.position);
		anchor2 = TransformS.AddComponent(entity, "DampedSpringAnchor2", transformComponent.transform.position + Vector3.up * 5f);
		ChipmunkProS.AddDampedSpring(ChipmunkProS.m_staticBody, chipmunkBodyC, anchor, anchor2, 50f, 200f, 10f);
	}

	public void handler(ucpCollisionPair _collisionPair, ucpCollisionPhase _collisionPhase)
	{
	}

	public override void Execute()
	{
		ticker++;
		if (ticker == 40)
		{
			ticker = 0;
			Vector2 vector = Vector2.up * -400f;
			Vector2 vector2 = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));
			ChipmunkProWrapper.ucpSpaceSetGravity(vector + vector2);
		}
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
