using UnityEngine;

public class NeppisCar : Vehicle
{
	public ChipmunkBodyC m_frontWheelBody;

	public ChipmunkBodyC m_rearWheelBody;

	public ChipmunkBodyC m_chassisBody;

	public TransformC m_chassisTC;

	private bool m_stopped;

	private bool m_dragStartedAfterStop;

	public NeppisCar(GraphElement _graphElement)
		: base(_graphElement)
	{
		_graphElement.m_width = 120f;
		_graphElement.m_height = 50f;
		uint num = (uint)(m_entity.m_index + 1000);
		m_chassisTC = TransformS.AddComponent(m_entity, _graphElement.m_name, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(100f, 30f, new Vector2(0f, 20f), 100f, 0.2f, 0.4f, (ucpCollisionType)3)
		{
			group = num
		};
		m_chassisBody = ChipmunkProS.AddDynamicBody(m_chassisTC, shape, m_unitC);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_chassisTC, Vector3.up * 20f, 100f, 30f);
		SpriteS.ConvertSpritesToPrefabComponent(m_chassisTC, true);
		ChipmunkProWrapper.ucpBodySetLinearDamp(m_chassisBody.body, new Vector2(0.998f, 0.998f));
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_chassisBody.body, 0.97f);
		m_chassisBody.m_linearDamp = new Vector2(0.998f, 0.998f);
		m_chassisBody.m_angularDamp = 0.97f;
		float stiffness = 10000f;
		float damping = 250f;
		float num2 = 20f;
		float num3 = -5f;
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name + "RearTire", _graphElement.m_position + new Vector3(-40f, num3));
		m_rearWheelBody = ChipmunkProS.AddDynamicBody(transformC, new ucpCircleShape(20f, Vector2.zero, 5f, 0f, 1.5f, (ucpCollisionType)3)
		{
			group = num
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rearWheelBody.body, 0.7f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC, Vector3.zero, 20f);
		TransformC grooveA = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(-40f, num3 + num2));
		TransformC grooveB = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(-40f, num3 - 20f));
		TransformC anchor = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(-40f, num3));
		ChipmunkProS.AddGrooveJoint(m_chassisBody, m_rearWheelBody, grooveA, grooveB, anchor);
		TransformC anchor2 = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(-40f, num2));
		TransformC anchor3 = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(-40f, num3));
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_rearWheelBody, anchor2, anchor3, num2 + Mathf.Abs(num3), stiffness, damping);
		TransformC transformC2 = TransformS.AddComponent(m_entity, _graphElement.m_name + "FrontTire", _graphElement.m_position + new Vector3(40f, num3));
		m_frontWheelBody = ChipmunkProS.AddDynamicBody(transformC2, new ucpCircleShape(20f, Vector2.zero, 5f, 0f, 1.5f, (ucpCollisionType)3)
		{
			group = num
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_frontWheelBody.body, 0.7f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC2, Vector3.zero, 20f);
		grooveA = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(40f, num3 + num2));
		grooveB = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(40f, num3 - 20f));
		anchor = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(40f, num3));
		ChipmunkProS.AddGrooveJoint(m_chassisBody, m_frontWheelBody, grooveA, grooveB, anchor);
		anchor2 = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(40f, num2));
		anchor3 = TransformS.AddComponent(m_entity, _graphElement.m_position + new Vector3(40f, num3));
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_frontWheelBody, anchor2, anchor3, num2 + Mathf.Abs(num3), stiffness, damping);
		InitMotors(m_chassisBody, m_rearWheelBody, m_frontWheelBody);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			CreateCamera();
			if (!Player.m_isSet)
			{
				Player.Set(new UnitTouchController(), m_chassisTC);
			}
			TouchAreaC c = TouchAreaS.AddCircleArea(m_chassisTC, "Sling", 100f, CameraS.m_mainCamera);
			TouchAreaS.AddTouchEventListener(c, TouchHandler);
		}
		CreateEditorTouchArea(_graphElement.m_width, _graphElement.m_height);
	}

	public void CreateCamera()
	{
		CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_chassisTC, 1000f, 800f);
		cameraTargetC.maxAngleChange = new Vector2(0.1f, 0.1f);
		CameraS.m_mainCameraMaxAngle = Vector2.zero;
		CameraS.m_mainCameraMaxVelocity = 400f;
		cameraTargetC.lowVelocity.x = 0.3f;
		cameraTargetC.highVelocity.x = 7f;
		cameraTargetC.lowVelocity.y = 1f;
		cameraTargetC.highVelocity.y = 5f;
		cameraTargetC.safeFrame = 0.1f;
		cameraTargetC.velocityScale = 1.8f;
	}

	public override void Update()
	{
		base.Update();
		if ((PsState.m_gameState == GameState.Play || PsState.m_gameState == GameState.Test) && ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body).magnitude < 25f)
		{
			SetHandBrake(true);
			m_stopped = true;
		}
	}

	private void TouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchIsSecondary[0])
		{
			return;
		}
		if (_touchPhases[0] == TouchAreaPhase.Began && m_stopped)
		{
			m_dragStartedAfterStop = true;
		}
		if (!m_dragStartedAfterStop)
		{
			return;
		}
		if (_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.MoveOut)
		{
			float f = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
			Vector2 lhs = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			Vector2 vector = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
			Vector2 vector2 = vector - (Vector2)m_chassisTC.transform.position;
			float num = Vector2.Dot(lhs, vector2.normalized);
			float num2 = vector2.magnitude * num;
			if (num2 > 250f)
			{
				num2 = 250f;
			}
			else if (num2 < -250f)
			{
				num2 = -250f;
			}
			DebugDraw.Clear(CameraS.m_mainCamera, m_chassisTC);
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_chassisTC, Vector3.zero, Vector3.right * num2);
		}
		else if (_touchPhases[0] == TouchAreaPhase.ReleaseIn || _touchPhases[0] == TouchAreaPhase.ReleaseOut)
		{
			if (!Player.m_gameIsStarted)
			{
				Player.StartRun();
			}
			float f2 = ChipmunkProWrapper.ucpBodyGetAngle(m_chassisBody.body);
			Vector2 vector3 = new Vector2(Mathf.Cos(f2), Mathf.Sin(f2));
			Vector2 vector4 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, _touches[0].m_currentPosition);
			Vector2 vector5 = vector4 - (Vector2)m_chassisTC.transform.position;
			float num3 = Vector2.Dot(vector3, vector5.normalized);
			float num4 = vector5.magnitude * num3;
			if (num4 > 250f)
			{
				num4 = 250f;
			}
			else if (num4 < -250f)
			{
				num4 = -250f;
			}
			SetHandBrake(false);
			ChipmunkProWrapper.ucpBodyApplyImpulse(m_chassisBody.body, vector3 * num4 * -400f, Vector2.zero);
			m_dragStartedAfterStop = false;
			m_stopped = false;
			DebugDraw.Clear(CameraS.m_mainCamera, m_chassisTC);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void GroundContactStart(ContactInfo _contactInfo)
	{
		base.GroundContactStart(_contactInfo);
		if (_contactInfo.m_ground != null)
		{
			if (_contactInfo.m_ground.m_angularDampEffect != 1f)
			{
				ChipmunkProWrapper.ucpBodySetAngularDamp(_contactInfo.m_contactBody.body, _contactInfo.m_ground.m_angularDampEffect);
			}
			if (_contactInfo.m_ground.m_linearDampEffect != Vector2.one)
			{
				ChipmunkProWrapper.ucpBodySetLinearDamp(_contactInfo.m_contactBody.body, _contactInfo.m_ground.m_linearDampEffect);
			}
		}
	}

	public override void GroundContactEnd(ContactInfo _contactInfo)
	{
		base.GroundContactEnd(_contactInfo);
		ChipmunkProWrapper.ucpBodySetAngularDamp(_contactInfo.m_contactBody.body, _contactInfo.m_contactBody.m_angularDamp);
		ChipmunkProWrapper.ucpBodySetLinearDamp(_contactInfo.m_contactBody.body, _contactInfo.m_contactBody.m_linearDamp);
	}
}
