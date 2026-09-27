using UnityEngine;

public class TouchAreaTestState : BasicState
{
	public override void Enter(IStatedObject _parent)
	{
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "Touch Areas");
		Entity entity = EntityManager.AddEntity();
		TransformC transformC = TransformS.AddComponent(entity);
		TouchAreaC touchAreaC = TouchAreaS.AddRectArea(transformC, "first", 100f, 100f, CameraS.m_mainCamera);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformC, Vector2.zero, 100f, 100f);
		touchAreaC.m_allowSecondary = true;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(150f, 0f, 0f));
		touchAreaC = TouchAreaS.AddRectArea(transformC, "second", 70f, 20f, CameraS.m_uiCamera);
		DebugDraw.CreateBox(CameraS.m_uiCamera, transformC, Vector2.zero, 70f, 20f);
		touchAreaC.m_allowSecondary = true;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(-50f, -50f, 10f));
		touchAreaC = TouchAreaS.AddCircleArea(transformC, "third", 50f, CameraS.m_mainCamera);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC, Vector2.zero, 50f);
		touchAreaC.m_allowSecondary = true;
		entity = EntityManager.AddEntity();
		transformC = TransformS.AddComponent(entity);
		TransformS.SetPosition(transformC, new Vector3(-50f, 0f, 10f));
		touchAreaC = TouchAreaS.AddCircleArea(transformC, "third", 25f, CameraS.m_mainCamera);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC, Vector2.zero, 25f);
		touchAreaC.m_allowSecondary = true;
	}

	private void Handler(TLTouch _t, bool _secondary)
	{
		if (!_secondary)
		{
			TouchAreaC primaryArea = _t.m_primaryArea;
			TouchAreaPhase primaryPhase = _t.m_primaryPhase;
			if (primaryArea.m_touchCount == 1)
			{
				switch (primaryPhase)
				{
				case TouchAreaPhase.Began:
				case TouchAreaPhase.RollIn:
					SpriteS.SetColorByTransformComponent(primaryArea.m_TC, Color.green);
					break;
				case TouchAreaPhase.RollOut:
					SpriteS.SetColorByTransformComponent(primaryArea.m_TC, Color.yellow);
					break;
				}
			}
			else if (primaryArea.m_touchCount == 0 && (primaryPhase == TouchAreaPhase.ReleaseIn || primaryPhase == TouchAreaPhase.ReleaseOut))
			{
				SpriteS.SetColorByTransformComponent(primaryArea.m_TC, Color.grey);
			}
			Debug.Log("Active: " + _t.m_primaryPhase);
			return;
		}
		TouchAreaC secondaryArea = _t.m_secondaryArea;
		TouchAreaPhase secondaryPhase = _t.m_secondaryPhase;
		if (secondaryArea.m_touchCount == 1)
		{
			if (secondaryPhase == TouchAreaPhase.RollIn)
			{
				SpriteS.SetColorByTransformComponent(secondaryArea.m_TC, Color.cyan);
			}
		}
		else if (secondaryArea.m_touchCount == 0 && secondaryPhase == TouchAreaPhase.RollOut)
		{
			SpriteS.SetColorByTransformComponent(secondaryArea.m_TC, Color.grey);
		}
		Debug.Log("Hot: " + _t.m_secondaryPhase);
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
