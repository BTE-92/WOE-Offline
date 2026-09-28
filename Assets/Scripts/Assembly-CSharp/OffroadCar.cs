using System;
using System.Collections.Generic;
using UnityEngine;

public class OffroadCar : Vehicle
{
	private const float TIRE_FRICTION = 1.5f;

	private const float TIRE_ELASTICITY = 0.2f;

	private const float FRONT_TIRE_RATE = 60f;

	private const float REAR_TIRE_RATE = 60f;

	private const float FRONT_WHEEL_RAD = 30f;

	private const float FRONT_WHEEL_WEIGHT = 5f;

	private const float REAR_WHEEL_RAD = 40f;

	private const float REAR_WHEEL_WEIGHT = 5f;

	private const float CHASSIS_WEIGHT = 100f;

	private const float FRONT_SPRING_FORCE = 11000f;

	private const float FRONT_SPRING_DAMP = 250f;

	private const float REAR_SPRING_FORCE = 11000f;

	private const float REAR_SPRING_DAMP = 250f;

	private const float TIRE_ANGULAR_DAMP = 0.997f;

	private const float CHASSIS_ANGULAR_DAMP = 0.97f;

	private const float CAR_LINEAR_DAMP = 0.997f;

	private const float SELF_BALANCE_FORCE = 80f;

	private const int MIN_RPM = 500;

	private const int MAX_RPM = 4000;

	public Minigame m_minigame;

	private Ghost m_recordingGhost;

	public static Vector2 FRONT_TIRE_FORCES = new Vector2(200000f, 400000f);

	public static Vector3 REAR_TIRE_FORCES = new Vector2(800000f, 400000f);

	public uint m_group;

	public ChipmunkBodyC m_frontWheelBody;

	public ChipmunkBodyC m_rearWheelBody;

	public ChipmunkBodyC m_chassisBody;

	public TransformC m_chassisTC;

	public TransformC m_alienHeadTC;

	public float m_gasAmount;

	public bool m_gasWasPressed;

	public bool m_brakeWasPressed;

	private static GameObject m_mainPrefab = null;

	public AutoGeometryBrush m_tireBrush;

	public PrefabC m_drivingFx;

	public string m_drivingFxName;

	public SoundC m_engineSound;

	public SoundC m_tireRollSound;

	public float m_currentRPM;

	public float m_currentLoad;

	public int m_airTime;

	public bool m_airTimeSoundTriggered;

	private Entity m_ghostEntity;

	private TransformC m_ghostChassisTC;

	private TransformC m_ghostRearWheelTC;

	private TransformC m_ghostFrontWheelTC;

	private TransformC m_ghostHeadTC;

	public int m_destructibleGroundContacts;

	public int m_skiddingEndTimer;

	private Vector2 m_centerOfGravityOffset = new Vector2(0f, 15f);

	private float m_targetFakeRotationX;

	private float m_fakeRotationX;

	public OffroadCar(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_graphElement.m_isCopyable = false;
		m_graphElement.m_isRemovable = false;
		m_graphElement.m_isRotateable = false;
		m_minigame = LevelManager.m_currentLevel as Minigame;
		m_tireBrush = new AutoGeometryBrush(2f, false, 0.15f);
		if (m_mainPrefab == null)
		{
			m_mainPrefab = ResourceManager.GetGameObject("Units/PoliceCarPrefab");
		}
		DebugDraw.defaultColor = new Color(0f, 0f, 0f, 1f);
		float num = 0.45f;
		_graphElement.m_width = 130f * num;
		_graphElement.m_height = 50f * num;
		m_group = (uint)(m_entity.m_index + 1000);
		TransformC transformC = (m_chassisTC = TransformS.AddComponent(m_entity, _graphElement.m_name));
		TransformS.SetTransform(transformC, _graphElement.m_position - (Vector3)m_centerOfGravityOffset, _graphElement.m_rotation);
		m_chassisBody = ChipmunkProS.AddDynamicBody(transformC, new ucpPolyShape(_graphElement.m_width * 1.5f, _graphElement.m_height * 1.2f, m_centerOfGravityOffset + new Vector2(0f, 5f), 100f, 0.2f, 0.5f, (ucpCollisionType)3)
		{
			group = m_group
		});
		m_chassisBody.customComponent = m_unitC;
		PrefabS.AddComponent(transformC, (Vector3)m_centerOfGravityOffset + new Vector3(0f, -3f, 0f), m_mainPrefab.transform.Find("PoliceCarBody").gameObject);
		ChipmunkProWrapper.ucpBodySetLinearDamp(m_chassisBody.body, new Vector2(0.997f, 0.997f));
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_chassisBody.body, 0.97f);
		m_chassisBody.m_linearDamp = new Vector2(0.997f, 0.997f);
		m_chassisBody.m_angularDamp = 0.97f;
		transformC = (m_alienHeadTC = TransformS.AddComponent(m_entity, "Alien head"));
		Vector3 vector = (Vector3)m_centerOfGravityOffset + m_chassisTC.transform.position + new Vector3(-10f, 53f, 0f);
		TransformS.SetTransform(transformC, vector, _graphElement.m_rotation);
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, new ucpCircleShape(16f, new Vector2(8f, -9f), 5f, 0.5f, 0.5f, (ucpCollisionType)3)
		{
			group = m_group
		});
		chipmunkBodyC.customComponent = m_unitC;
		PrefabS.AddComponent(transformC, new Vector3(8f, -35f), m_mainPrefab.transform.Find("PoliceCarAlien").gameObject);
		TransformC anchor = TransformS.AddComponent(m_entity, "AlienJoint1", vector + new Vector3(-3f, -38f, 0f));
		ChipmunkProS.AddPivotJoint(m_chassisBody, chipmunkBodyC, anchor);
		ChipmunkProS.AddRotaryLimitJoint(m_chassisBody, chipmunkBodyC, anchor, -0.61086524f, 0.61086524f);
		anchor = TransformS.AddComponent(m_entity, "AlienSpring1", vector);
		TransformC anchor2 = TransformS.AddComponent(m_entity, "AlienSpring2", vector);
		ChipmunkProS.AddDampedSpring(m_chassisBody, chipmunkBodyC, anchor, anchor2, 0f, 400f, 50f);
		float num2 = 30f * num;
		float num3 = 40f * num;
		Vector2 vector2 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(_graphElement.m_width * 0.55f, (0f - _graphElement.m_height) * 0.55f - num2 * 0.5f);
		m_frontWheelBody = createTire(m_entity, m_chassisBody, vector2, num2, 5f, true);
		m_frontWheelBody.customComponent = m_unitC;
		anchor = TransformS.AddComponent(m_entity, "FrontWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		anchor2 = TransformS.AddComponent(m_entity, "FrontWheelJoint2", vector2);
		float magnitude = (anchor.transform.position - anchor2.transform.position).magnitude;
		ChipmunkProS.AddSlideJoint(m_chassisBody, m_frontWheelBody, anchor, anchor2, magnitude, magnitude);
		anchor = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor1", vector2);
		anchor2 = TransformS.AddComponent(m_entity, "FrontWheelSpringAnchor2", vector2);
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_frontWheelBody, anchor, anchor2, 0f, 11000f, 250f);
		Vector2 vector3 = m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2((0f - _graphElement.m_width) * 0.5f, (0f - _graphElement.m_height) * 0.3f - num3 * 0.5f);
		m_rearWheelBody = createTire(m_entity, m_chassisBody, vector3, num3, 5f, false);
		m_rearWheelBody.customComponent = m_unitC;
		anchor = TransformS.AddComponent(m_entity, "RearWheelJoint1", m_centerOfGravityOffset + (Vector2)m_chassisTC.transform.position + new Vector2(0f, (0f - _graphElement.m_height) * 0.5f));
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelJoint2", vector3);
		ChipmunkProS.AddPinJoint(m_chassisBody, m_rearWheelBody, anchor, anchor2);
		anchor = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor1", vector3);
		anchor2 = TransformS.AddComponent(m_entity, "RearWheelSpringAnchor2", vector3);
		ChipmunkProS.AddDampedSpring(m_chassisBody, m_rearWheelBody, anchor, anchor2, 0f, 11000f, 250f);
		InitMotors(m_chassisBody, m_rearWheelBody, m_frontWheelBody);
		float num4 = 0.75f;
		SetMotorParameters(FRONT_TIRE_FORCES, 60f, REAR_TIRE_FORCES, 60f * num4);
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			if (!Player.m_isSet)
			{
				m_tireRollSound = null;
				m_engineSound = SoundS.AddComponent(m_chassisTC, "/InGame/Vehicles/OffroadCarEngine");
				SoundS.PlaySound(m_engineSound);
				m_currentRPM = 500f;
				SoundS.SetSoundParameter(m_engineSound, "RPM", m_currentRPM);
				CreateCamera();
				Player.Set(new DualButtonController(), m_chassisTC);
				if (PsState.m_gameState == GameState.Play)
				{
					m_recordingGhost = new Ghost();
					m_recordingGhost.AddNode("chassis", m_chassisTC);
					m_recordingGhost.AddNode("rearWheel", m_rearWheelBody.TC);
					m_recordingGhost.AddNode("frontWheel", m_frontWheelBody.TC);
					m_recordingGhost.AddNode("alienHead", m_alienHeadTC);
				}
			}
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_frontWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_rearWheelBody, TireCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(m_chassisBody, ChassisCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
			ChipmunkProS.AddCollisionHandler(chipmunkBodyC, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)2, true, false, true);
			ChipmunkProS.AddCollisionHandler(chipmunkBodyC, AlienCollisionHandler, (ucpCollisionType)3, (ucpCollisionType)4, true, false, true);
		}
		ProjectorS.AddComponent(m_chassisTC, ResourceManager.GetMaterial("Shadows/Shadow Material"), 256, new Vector3(0f, 40f));
		CreateEditorTouchArea(_graphElement.m_width, _graphElement.m_height);
		m_hitPoints = 1f;
		m_hitPointType = HitPointType.Lives;
		DebugDraw.defaultColor = new Color(1f, 1f, 1f, 1f);
		m_airTimeSoundTriggered = true;
	}

	public override void SetAllBaseArmours()
	{
		base.SetAllBaseArmours();
		SetBaseArmor(DamageType.Weapon, 20f);
		SetBaseArmor(DamageType.Electric, 20f);
	}

	public void CreateGhostEntity()
	{
		if (m_ghostEntity != null)
		{
			return;
		}
		m_ghostEntity = EntityManager.AddEntity();
		m_ghostChassisTC = TransformS.AddComponent(m_ghostEntity, "GhostChassis");
		PrefabS.AddComponent(m_ghostChassisTC, (Vector3)m_centerOfGravityOffset + new Vector3(0f, -3f, 0f), m_mainPrefab.transform.Find("PoliceCarBody").gameObject);
		m_ghostHeadTC = TransformS.AddComponent(m_ghostEntity, "GhostHead");
		PrefabS.AddComponent(m_ghostHeadTC, new Vector3(8f, -35f), m_mainPrefab.transform.Find("PoliceCarAlien").gameObject);
		m_ghostFrontWheelTC = TransformS.AddComponent(m_ghostEntity, "GhostFrontWheel");
		PrefabS.AddComponent(m_ghostFrontWheelTC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("PoliceCarTireF1").gameObject);
		PrefabS.AddComponent(m_ghostFrontWheelTC, new Vector3(0f, 0f, 18f), m_mainPrefab.transform.Find("PoliceCarTireF2").gameObject);
		m_ghostRearWheelTC = TransformS.AddComponent(m_ghostEntity, "GhostRearWheel");
		PrefabS.AddComponent(m_ghostRearWheelTC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("PoliceCarTireB1").gameObject);
		PrefabS.AddComponent(m_ghostRearWheelTC, new Vector3(0f, 0f, 18f), m_mainPrefab.transform.Find("PoliceCarTireB2").gameObject);
		Material material = ResourceManager.GetMaterial("SpecialMaterials/GhostMaterial");
		material.SetFloat("_Brightness", 0f);
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Transform, m_ghostEntity);
		foreach (TransformC item in componentsByEntity)
		{
			GameObject gameObject = item.transform.gameObject;
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(Renderer));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i] as Renderer;
				renderer.material = material;
			}
		}
	}

	public void DestroyGhostEntity()
	{
		if (m_ghostEntity != null)
		{
			EntityManager.RemoveEntity(m_ghostEntity);
			m_ghostEntity = null;
		}
	}

	public void UpdateGhostEntity()
	{
		Ghost ghost = m_minigame.m_ghost;
		if (m_ghostEntity != null && ghost != null && ghost.m_keyframeCount > 0)
		{
			ghost.Update();
			TransformS.SetPosition(m_ghostChassisTC, ghost.GetCurrentPosition("chassis"));
			TransformS.SetRotation(m_ghostChassisTC, new Vector3(0f, 0f, ghost.GetCurrentRotation("chassis")));
			TransformS.SetPosition(m_ghostFrontWheelTC, ghost.GetCurrentPosition("frontWheel"));
			TransformS.SetRotation(m_ghostFrontWheelTC, new Vector3(0f, 0f, ghost.GetCurrentRotation("frontWheel")));
			TransformS.SetPosition(m_ghostRearWheelTC, ghost.GetCurrentPosition("rearWheel"));
			TransformS.SetRotation(m_ghostRearWheelTC, new Vector3(0f, 0f, ghost.GetCurrentRotation("rearWheel")));
			TransformS.SetPosition(m_ghostHeadTC, ghost.GetCurrentPosition("alienHead"));
			TransformS.SetRotation(m_ghostHeadTC, new Vector3(0f, 0f, ghost.GetCurrentRotation("alienHead")));
			Material material = ResourceManager.GetMaterial("SpecialMaterials/GhostMaterial");
			float magnitude = (m_ghostChassisTC.transform.position - m_chassisTC.transform.position).magnitude;
			float positionBetween = ToolBox.getPositionBetween(magnitude, 10f, 130f);
			material.SetFloat("_Brightness", positionBetween * 1.5f);
			if (ghost.PlaybackEnded())
			{
				DestroyGhostEntity();
			}
		}
	}

	public void StopAndSaveRecordingGhost()
	{
		bool flag = PsState.m_gameTicks <= PsState.m_sessionBestTime && PsState.m_playerReachedGoal;
		bool flag2 = PsState.m_gameTicks >= PsState.m_sessionLongestRunTicks;
		bool flag3 = flag || (flag2 && PsState.m_sessionBestTime == int.MaxValue);
		if (m_recordingGhost != null && flag3 && m_recordingGhost.m_recording)
		{
			m_recordingGhost.StopRecord();
			if (m_minigame.m_ghost != null)
			{
				m_minigame.m_ghost.Destroy();
				DestroyGhostEntity();
			}
			m_minigame.m_ghost = m_recordingGhost.DeepCopy();
			m_recordingGhost.Destroy();
			m_recordingGhost = null;
		}
	}

	public void CreateCamera()
	{
		if (!Player.m_isSet)
		{
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_chassisTC, 600f, 400f);
			cameraTargetC.maxAngleChange = new Vector2(0.25f, 0.4f);
			CameraS.m_mainCameraMaxAngle = new Vector3(10f, 15f);
			CameraS.m_mainCameraMaxVelocity = 400f;
			cameraTargetC.lowVelocity.x = 0.3f;
			cameraTargetC.highVelocity.x = 7f;
			cameraTargetC.lowVelocity.y = 1f;
			cameraTargetC.highVelocity.y = 5f;
			cameraTargetC.safeFrame = 0.1f;
			cameraTargetC.velocityScale = 0.75f;
			cameraTargetC.verticalOffset = 0f;
			cameraTargetC.verticalAngle = 10f;
		}
	}

	public override void Update()
	{
		base.Update();
		if (PsState.m_gameState == GameState.Play || PsState.m_gameState == GameState.Test)
		{
			if (!m_isDead)
			{
				bool flag = false;
				bool flag2 = false;
				if (Player.m_controller != null)
				{
					if (Player.m_controller.m_open && Player.m_mainTransform == m_chassisTC)
					{
						flag2 = Player.m_controller.GetButtonState("RightButton") == ControllerButtonState.ON;
						flag = Player.m_controller.GetButtonState("LeftButton") == ControllerButtonState.ON;
					}
					if (flag2 || flag)
					{
						Player.StartRun();
					}
				}
				if (flag2 && !m_gasWasPressed)
				{
					SoundS.PlaySingleShot("/InGame/Vehicles/GasPressed", Vector3.zero);
				}
				if (flag && !m_brakeWasPressed)
				{
					SoundS.PlaySingleShot("/InGame/Vehicles/BrakePressed", Vector3.zero);
				}
				if (m_airTime > 20 && !m_airTimeSoundTriggered)
				{
					cpSegmentQueryInfo info = default(cpSegmentQueryInfo);
					ChipmunkProWrapper.ucpSpaceSegmentQueryFirst(m_chassisTC.transform.position, (Vector2)m_chassisTC.transform.position + new Vector2(0f, -100f), uint.MaxValue, m_group, ref info);
					if (info.t == 1f)
					{
						SoundS.PlaySingleShot("/InGame/BigAirJump", Vector3.zero);
						m_airTimeSoundTriggered = true;
					}
				}
				m_brakeWasPressed = flag;
				m_gasWasPressed = flag2;
				bool flag3 = m_contactState == ContactState.OnAir;
				bool flag4 = GetContactState(m_frontWheelBody) == ContactState.OnAir && GetContactState(m_rearWheelBody) == ContactState.OnAir;
				Vector2 vector = ChipmunkProWrapper.ucpBodyGetVel(m_chassisBody.body);
				if (flag3)
				{
					m_airTime++;
					m_targetFakeRotationX = ToolBox.limitBetween(vector.y / 20f, -10f, 10f);
				}
				else
				{
					m_targetFakeRotationX = 0f;
					m_airTime = 0;
					m_airTimeSoundTriggered = false;
				}
				m_fakeRotationX -= (m_fakeRotationX - m_targetFakeRotationX) * 0.1f;
				m_chassisTC.transform.Rotate(new Vector3(1f, 0f, 0f), m_fakeRotationX);
				float target = 0f;
				if (flag2 && !flag)
				{
					m_gasAmount = ToolBox.limitBetween(m_gasAmount + 0.05f, 0f, 1f);
					if (flag3)
					{
						target = 20f;
					}
				}
				else if (flag && !flag2)
				{
					m_gasAmount = ToolBox.limitBetween(m_gasAmount - 0.05f, -1f, 0f);
					if (flag3)
					{
						target = -20f;
					}
				}
				else
				{
					m_gasAmount *= 0.98f;
				}
				UpdateMotors((!flag2 && !flag) ? 0f : m_gasAmount);
				bool flag5 = false;
				if (m_destructibleGroundContacts > 0 && (flag2 || flag))
				{
					flag5 = true;
					m_skiddingEndTimer = 15;
					SkidTire(m_frontWheelBody);
					SkidTire(m_rearWheelBody);
					foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
					{
						if (layer.m_groundC.m_ground.m_destructible)
						{
							layer.UpdateSegments();
						}
					}
				}
				if (m_skiddingEndTimer > 0)
				{
					m_skiddingEndTimer--;
				}
				float z = m_chassisBody.TC.transform.rotation.eulerAngles.z;
				bool flag6 = !flag3 && flag4 && z > 90f && z < 270f && (flag2 || flag);
				ChipmunkProWrapper.ucpBodyResetForces(m_chassisBody.body);
				if (flag3 || flag6)
				{
					float num = 80f;
					if (flag6)
					{
						num *= 5f;
					}
					float f = m_chassisBody.TC.transform.rotation.eulerAngles.z * ((float)Math.PI / 180f);
					Vector2 normalized = new Vector2(Mathf.Sin(f), 0f - Mathf.Cos(f)).normalized;
					float num2 = ToolBox.limitBetween(Mathf.DeltaAngle(z, target), -90f, 90f);
					ChipmunkProWrapper.ucpBodyApplyForce(m_chassisBody.body, normalized * num2 * num, ChipmunkProWrapper.ucpBodyLocal2World(m_chassisBody.body, new Vector2((0f - m_graphElement.m_width) * 0.5f, 0f)));
					ChipmunkProWrapper.ucpBodyApplyForce(m_chassisBody.body, -normalized * num2 * num, ChipmunkProWrapper.ucpBodyLocal2World(m_chassisBody.body, new Vector2(m_graphElement.m_width * 0.5f, 0f)));
				}
				if (m_engineSound != null)
				{
					float val = Mathf.Abs(ChipmunkProWrapper.ucpBodyGetAngVel(m_rearWheelBody.body));
					float positionBetween = ToolBox.getPositionBetween(val, 0f, 50f);
					float num3 = ((!flag2 && !flag) ? 500f : Mathf.Lerp(500f, 4000f, positionBetween));
					float num4 = ((!flag4) ? 1 : 0);
					m_currentRPM += (num3 - m_currentRPM) * 0.1f;
					m_currentLoad += (num4 - m_currentLoad) * 0.1f;
					SoundS.SetSoundParameter(m_engineSound, "RPM", m_currentRPM);
					SoundS.SetSoundParameter(m_engineSound, "Load", m_currentLoad);
				}
				if (flag4 && m_drivingFx != null)
				{
					RemoveDriveFx();
				}
				else if (m_drivingFx != null)
				{
					ParticleSystem particleSystem = m_drivingFx.p_gameObject.GetComponent<ParticleSystem>();
					if (m_drivingFxName.Equals("ParticleFx/MudSplatter"))
					{
						if (!flag5 && m_skiddingEndTimer == 0 && particleSystem.isPlaying)
						{
							particleSystem.Stop();
						}
						else if (flag5 && particleSystem.isStopped)
						{
							particleSystem.Play();
						}
					}
					else
					{
						float emissionRate = ToolBox.getPositionBetween(m_currentRPM, 500f, 5000f) * 30f;
						particleSystem.emissionRate = emissionRate;
					}
				}
				if (m_tireRollSound != null)
				{
					if (flag4)
					{
						RemoveTireRollSound();
					}
					else
					{
						SoundS.SetSoundParameter(m_tireRollSound, "RPM", m_currentRPM);
					}
				}
			}
			if (Player.m_mainTransform == m_chassisTC && PsState.m_gameStarted && PsState.m_gameState == GameState.Play)
			{
				if (m_minigame.m_ghost != null && m_ghostEntity == null && PsState.m_gameTicks == 0)
				{
					m_minigame.m_ghost.m_playbackTick = 0;
					CreateGhostEntity();
				}
				if (m_recordingGhost != null)
				{
					m_recordingGhost.Update();
				}
				UpdateGhostEntity();
			}
		}
		if (PsState.m_gameEnded || m_isDead)
		{
			StopAndSaveRecordingGhost();
		}
	}

	private void StartDriveFx(bool _destructibleGround, string _fx)
	{
		if (m_drivingFx == null && !m_isDead && _fx != null)
		{
			Vector3 zero = Vector3.zero;
			m_drivingFx = PrefabS.AddComponent(_offset: _destructibleGround ? new Vector3(0f, 0f, -2.91f) : new Vector3(-33.13f, 0f, -2.91f), _parentTC: m_chassisTC, _gameObject: ResourceManager.GetGameObject(_fx), _identifier: string.Empty, _resetLocalRotation: false);
			m_drivingFxName = _fx;
		}
	}

	private void RemoveDriveFx()
	{
		if (m_drivingFx != null)
		{
			TimerC timerC = TimerS.AddComponent(m_entity, "RemoveTimerForDriveFxEmitter", 2f, 0f, false, RemoveDriveFxTimerDelegate);
			timerC.customComponent = m_drivingFx;
			m_drivingFx.p_gameObject.GetComponent<ParticleSystem>().Stop();
			m_drivingFx = null;
			m_drivingFxName = string.Empty;
		}
	}

	public void RemoveDriveFxTimerDelegate(TimerC _c)
	{
		PrefabS.RemoveComponent(_c.customComponent as PrefabC);
		TimerS.RemoveComponent(_c);
	}

	private void StartTireRollSound(string _sound)
	{
		if (m_tireRollSound == null && !m_isDead)
		{
			m_tireRollSound = SoundS.AddComponent(m_chassisTC, _sound);
			SoundS.SetSoundParameter(m_tireRollSound, "RPM", m_currentRPM);
			SoundS.PlaySound(m_tireRollSound);
		}
	}

	private void RemoveTireRollSound()
	{
		if (m_tireRollSound != null)
		{
			SoundS.RemoveComponent(m_tireRollSound);
			m_tireRollSound = null;
		}
	}

	public void SkidTire(ChipmunkBodyC body)
	{
		float f = ChipmunkProWrapper.ucpBodyGetAngVel(body.body);
		if (!(Mathf.Abs(f) > 30f))
		{
			return;
		}
		Vector2 pos = ChipmunkProWrapper.ucpBodyGetPos(body.body);
		foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
		{
			if (layer.m_groundC.m_ground.m_destructible)
			{
				layer.PaintWithBrush(m_tireBrush, pos, AGDrawMode.SUB, m_tireBrush.m_subPixelAccuracy, ref layer.m_bytes);
			}
		}
	}

	public ChipmunkBodyC createTire(Entity _e, ChipmunkBodyC _chassis, Vector2 _pos, float _rad, float _weight, bool _front)
	{
		TransformC transformC = TransformS.AddComponent(_e, "Car Tire");
		TransformS.SetTransform(transformC, _pos, Vector2.zero);
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(_rad, Vector2.zero, _weight, 0.2f, 1.5f, (ucpCollisionType)3);
		ucpCircleShape2.group = m_group;
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpCircleShape2);
		if (_front)
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("PoliceCarTireF1").gameObject);
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 18f), m_mainPrefab.transform.Find("PoliceCarTireF2").gameObject);
		}
		else
		{
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, -23f), m_mainPrefab.transform.Find("PoliceCarTireB1").gameObject);
			PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 18f), m_mainPrefab.transform.Find("PoliceCarTireB2").gameObject);
		}
		ChipmunkProWrapper.ucpBodySetLinearDamp(chipmunkBodyC.body, new Vector2(0.997f, 0.997f));
		ChipmunkProWrapper.ucpBodySetAngularDamp(chipmunkBodyC.body, 0.997f);
		chipmunkBodyC.m_linearDamp = new Vector2(0.997f, 0.997f);
		chipmunkBodyC.m_angularDamp = 0.997f;
		return chipmunkBodyC;
	}

	public override void GroundContactStart(ContactInfo _contactInfo)
	{
		base.GroundContactStart(_contactInfo);
		Ground ground = _contactInfo.m_ground;
		if (_contactInfo.m_contactBody.body == m_rearWheelBody.body || _contactInfo.m_contactBody.body == m_frontWheelBody.body)
		{
			RemoveDriveFx();
			RemoveTireRollSound();
			StartDriveFx(ground.m_destructible, ground.m_driveFX);
			StartTireRollSound(ground.m_tireRollSound);
			if (ground.m_destructible)
			{
				m_destructibleGroundContacts++;
			}
		}
	}

	public override void GroundContactEnd(ContactInfo _contactInfo)
	{
		base.GroundContactEnd(_contactInfo);
		Ground ground = _contactInfo.m_ground;
		if (!(_contactInfo.m_contactBody.body == m_rearWheelBody.body) && !(_contactInfo.m_contactBody.body == m_frontWheelBody.body))
		{
			return;
		}
		if (ground.m_destructible)
		{
			m_destructibleGroundContacts--;
		}
		if (m_drivingFx == null || ground.m_driveFX == null || !ground.m_driveFX.Equals(m_drivingFxName))
		{
			return;
		}
		Ground ground2 = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_ground != ground)
			{
				ground2 = m_contacts[num].m_ground;
				break;
			}
		}
		RemoveDriveFx();
		RemoveTireRollSound();
		if (ground2 != null)
		{
			if (ground2.m_driveFX != null)
			{
				StartDriveFx(ground2.m_destructible, ground2.m_driveFX);
			}
			if (ground2.m_tireRollSound != null)
			{
				StartTireRollSound(ground2.m_tireRollSound);
			}
		}
	}

	private void TireCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin && Mathf.Abs(_pair.impulse.normalized.y) > 0.5f)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 1000f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/TireImpact", Vector3.zero, "Force", positionBetween);
			}
		}
	}

	private void ChassisCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 2500f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/ChassisImpact", Vector3.zero, "Force", positionBetween);
			}
		}
	}

	private void AlienCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		if (_phase == ucpCollisionPhase.Begin)
		{
			float positionBetween = ToolBox.getPositionBetween(_pair.impulse.magnitude, 2500f, 10000f);
			if (positionBetween > 0f)
			{
				SoundS.PlaySingleShotWithParameter("/InGame/Vehicles/AlienHeadImpact", Vector3.zero, "Force", positionBetween);
				EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/HeadHit"), new Vector3(_pair.point.x, _pair.point.y, 0f), Vector3.zero, 2f, "GTAG_INGAME_PARTICLES");
			}
		}
	}

	public override void EmergencyKill()
	{
		Kill(DamageType.Impact, float.MaxValue);
		Destroy();
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		UpdateMotors(0f);
		Player.CloseController();
		SoundS.StopSound(m_engineSound);
		SoundS.PlaySingleShot("/InGame/Vehicles/OffroadCarDeath", Vector3.zero);
		PrefabS.AddComponent(m_chassisTC, new Vector3(33.13f, 30.11f, -2.91f), ResourceManager.GetGameObject("ParticleFx/EngineBreakdown"));
		RemoveTireRollSound();
		RemoveDriveFx();
	}

	public override void Destroy()
	{
		StopAndSaveRecordingGhost();
		Player.StopRun();
		DestroyGhostEntity();
		if (m_recordingGhost != null)
		{
			m_recordingGhost.Destroy();
			m_recordingGhost = null;
		}
		m_tireBrush.Destroy();
		base.Destroy();
	}
}
