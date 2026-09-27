using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : Character
{
	private const float WEIGHT = 50f;

	private const float FRICTION = 0.9f;

	public TransformC m_mainTC;

	public ChipmunkBodyC m_mainBody;

	public ucpCircleShape m_feetShape;

	public ucpPolyShape m_torsoShape;

	public uint m_group;

	public float m_scale;

	public bool m_leftPressed;

	public bool m_rightPressed;

	public bool m_jumpPressed;

	public bool m_jumpReleased;

	public int m_ticksSinceLastJumpPress;

	public bool m_isOnGround;

	public Vector2 m_torsoNormal;

	public bool m_isJumping;

	public bool m_airJumpUsed;

	public int m_jumpDelay;

	public int m_airTimer;

	public int m_groundTimer;

	public bool m_feetTouchingGround;

	public Vector2 m_feetNormal;

	public List<Vector2> m_collisionWorldPoint = new List<Vector2>();

	public List<ChipmunkBodyC> m_collisionTargetBody = new List<ChipmunkBodyC>();

	public TestCharacter(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_group = (uint)(m_entity.m_index + 1000);
		m_scale = 0.8f;
		float num = 50f * m_scale;
		float num2 = 60f * m_scale;
		m_mainTC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(m_mainTC, _graphElement.m_position, _graphElement.m_rotation);
		m_torsoShape = new ucpPolyShape(num, num2, Vector2.zero, 16.666666f, 0f, 0f, (ucpCollisionType)3);
		m_torsoShape.group = m_group;
		float num3 = num * 0.5f;
		Vector2 vector = new Vector2(0f, num2 * 0.5f);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(num3, vector, 16.666666f, 0f, 0f, (ucpCollisionType)3)
		{
			group = m_group
		};
		m_feetShape = new ucpCircleShape(num3, -vector, 16.666666f, 0f, 0.9f, (ucpCollisionType)3);
		m_feetShape.group = m_group;
		m_mainBody = ChipmunkProS.AddDynamicBody(m_mainTC, new ucpShape[3] { m_torsoShape, ucpCircleShape2, m_feetShape });
		m_mainBody.customComponent = m_unitC;
		DebugDraw.defaultColor = new Color(0f, 0f, 0f, 1f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, m_mainTC, new Vector2(0f, 0f), num, num2);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, m_mainTC, vector, num3);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, m_mainTC, -vector, num3);
		ChipmunkProWrapper.ucpBodySetMoment(m_mainBody.body, float.PositiveInfinity);
		ChipmunkProWrapper.ucpBodySetLinearDamp(m_mainBody.body, new Vector2(0.98f, 1f));
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_mainBody, CollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_mainBody, CollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, true, false);
			ChipmunkProS.AddCollisionHandler(m_mainBody, CollisionHandler, (ucpCollisionType)3, (ucpCollisionType)3, true, true, false);
			CreateCamera();
			if (!Player.m_isSet)
			{
				Player.Set(new TwoPlusOneButtonController(), m_mainTC);
			}
		}
		CreateEditorTouchArea(num, num2 + num3);
	}

	public void CreateCamera()
	{
		if (!Player.m_isSet)
		{
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_mainTC, 450f, 300f);
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
	}

	public override void Update()
	{
		base.Update();
		if (PsState.m_gameState != GameState.Play && PsState.m_gameState != GameState.Test)
		{
			return;
		}
		if (Player.m_controller.m_open && Player.m_mainTransform == m_mainTC)
		{
			m_leftPressed = Player.m_controller.GetButtonState("LeftButton1") == ControllerButtonState.ON;
			m_rightPressed = Player.m_controller.GetButtonState("LeftButton2") == ControllerButtonState.ON;
			m_jumpPressed = Player.m_controller.GetButtonState("RightButton") == ControllerButtonState.ON;
		}
		else
		{
			m_leftPressed = false;
			m_rightPressed = false;
			m_jumpPressed = false;
		}
		m_ticksSinceLastJumpPress++;
		if (m_jumpPressed)
		{
			m_ticksSinceLastJumpPress = 0;
		}
		if (m_leftPressed || m_rightPressed || m_jumpPressed)
		{
			Player.StartRun();
		}
		m_isOnGround = m_contactState == ContactState.OnContact;
		m_feetNormal = m_feetNormal.normalized;
		m_torsoNormal = m_torsoNormal.normalized;
		if (Mathf.Abs(m_feetNormal.x) > 0.9f)
		{
			m_feetTouchingGround = false;
		}
		float num = Mathf.Lerp(1f, 1.5f, Mathf.Sqrt(Mathf.Abs(m_feetNormal.x))) * 8f;
		float num2 = 0f;
		if (m_leftPressed)
		{
			num2 = -0.5f;
		}
		else if (m_rightPressed)
		{
			num2 = 0.5f;
		}
		if (num2 == 0f)
		{
			ChipmunkProWrapper.ucpShapeSetFriction(m_feetShape.shapePtr, 0.9f);
			ChipmunkProWrapper.ucpBodySetVelLimit(m_mainBody.body, 9999f);
		}
		else if (m_isOnGround && !m_isJumping)
		{
			ChipmunkProWrapper.ucpBodySetVelLimit(m_mainBody.body, 200f);
			if (Mathf.Abs(m_torsoNormal.x) < 0.8f)
			{
				m_feetNormal.x = ToolBox.limitBetween(m_feetNormal.x, -0.5f, 0.5f);
			}
			else
			{
				m_feetNormal.x = 0f;
			}
			Vector2 vector = -new Vector2(m_feetNormal.y, 0f - m_feetNormal.x);
			Vector2 vector2 = vector * num2 * num;
			ChipmunkProWrapper.ucpShapeSetFriction(m_feetShape.shapePtr, 0f);
			ChipmunkProWrapper.ucpBodyApplyImpulse(m_mainBody.body, vector2 * 100f, Vector2.zero);
			if (m_feetTouchingGround && m_collisionTargetBody.Count > 0)
			{
				for (int i = 0; i < m_collisionTargetBody.Count; i++)
				{
					Vector2 vector3 = vector2 * ChipmunkProWrapper.ucpBodyGetMass(m_collisionTargetBody[i].body) * 0.9f;
					ChipmunkProWrapper.ucpBodyApplyImpulse(m_collisionTargetBody[i].body, -vector3, ChipmunkProWrapper.ucpBodyWorld2Local(m_collisionTargetBody[i].body, m_collisionWorldPoint[i]));
				}
			}
		}
		else
		{
			ChipmunkProWrapper.ucpShapeSetFriction(m_feetShape.shapePtr, 0f);
			ChipmunkProWrapper.ucpBodySetVelLimit(m_mainBody.body, 9999f);
			float num3 = 100f;
			ChipmunkProWrapper.ucpBodyApplyImpulse(m_mainBody.body, new Vector2(5f, 0f) * num2 * num3, Vector2.zero);
		}
		bool flag = (m_jumpPressed && m_feetTouchingGround) || m_ticksSinceLastJumpPress < 5;
		if (flag && !m_isJumping && m_isOnGround)
		{
			ChipmunkProWrapper.ucpBodySetVelLimit(m_mainBody.body, 9999f);
			Vector2 vector4 = ChipmunkProWrapper.ucpBodyGetVel(m_mainBody.body);
			ChipmunkProWrapper.ucpBodySetVel(m_mainBody.body, new Vector2(vector4.x, 0f));
			ChipmunkProWrapper.ucpBodyApplyImpulse(m_mainBody.body, new Vector2(0f, 130f) * 100f, Vector2.zero);
			m_isJumping = true;
			m_isOnGround = false;
			m_jumpDelay = 10;
			m_jumpReleased = false;
			m_ticksSinceLastJumpPress = 999;
		}
		if (!m_jumpPressed)
		{
			m_jumpReleased = true;
		}
		if (m_jumpPressed && m_isJumping && !m_airJumpUsed && m_jumpReleased)
		{
			Vector2 vector5 = ChipmunkProWrapper.ucpBodyGetVel(m_mainBody.body);
			ChipmunkProWrapper.ucpBodySetVel(m_mainBody.body, new Vector2(vector5.x, 0f));
			ChipmunkProWrapper.ucpBodyApplyImpulse(m_mainBody.body, new Vector2(0f, 130f) * 100f, Vector2.zero);
			m_airJumpUsed = true;
			m_isJumping = true;
			m_jumpDelay = 10;
			m_jumpReleased = false;
			m_ticksSinceLastJumpPress = 999;
		}
		if (m_isJumping)
		{
			if (m_jumpDelay > 0)
			{
				m_jumpDelay--;
			}
			if (m_jumpDelay == 0 && m_feetTouchingGround && m_isOnGround && !flag)
			{
				m_isJumping = false;
				m_airJumpUsed = false;
			}
		}
		m_feetTouchingGround = false;
		m_feetNormal = Vector2.zero;
		m_torsoNormal = Vector2.zero;
		m_collisionWorldPoint.Clear();
		m_collisionTargetBody.Clear();
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_pair.shapeA == m_feetShape.shapePtr)
		{
			ucpCollisionPhase ucpCollisionPhase2 = _phase;
			if (ucpCollisionPhase2 == ucpCollisionPhase.Begin || ucpCollisionPhase2 == ucpCollisionPhase.Persist)
			{
				m_feetTouchingGround = true;
				m_feetNormal += _pair.normal;
				ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
				if (!ChipmunkProWrapper.ucpBodyIsStatic(chipmunkBodyC.body) && !m_collisionTargetBody.Contains(chipmunkBodyC))
				{
					m_collisionWorldPoint.Add(_pair.point);
					m_collisionTargetBody.Add(chipmunkBodyC);
				}
			}
		}
		if (_pair.shapeA == m_torsoShape.shapePtr)
		{
			ucpCollisionPhase ucpCollisionPhase2 = _phase;
			if (ucpCollisionPhase2 == ucpCollisionPhase.Begin || ucpCollisionPhase2 == ucpCollisionPhase.Persist)
			{
				m_torsoNormal += _pair.normal;
			}
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
