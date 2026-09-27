using System;
using UnityEngine;

public class AlienCharacter : Character
{
	private uint m_group;

	public TransformC m_mainTC;

	private PrefabC m_mainPC;

	public Animator m_animator;

	private Animation m_overrideRagdollAnimation;

	private AnimationClip m_overrideRagdollClip;

	private Transform m_hips;

	private Transform m_spine;

	private Transform m_spine1;

	private Transform m_neck;

	private Transform m_head;

	private Transform m_leftLeg;

	private Transform m_leftKnee;

	private Transform m_leftArm;

	private Transform m_leftAlbow;

	private Transform m_leftHand;

	private Transform m_rightLeg;

	private Transform m_rightKnee;

	private Transform m_rightArm;

	private Transform m_rightAlbow;

	private Transform m_rightHand;

	private ChipmunkBodyC m_hipsCBC;

	private ChipmunkBodyC m_spineCBC;

	private ChipmunkBodyC m_spine1CBC;

	private ChipmunkBodyC m_neckCBC;

	private ChipmunkBodyC m_headCBC;

	private ChipmunkBodyC m_leftLegCBC;

	private ChipmunkBodyC m_leftKneeCBC;

	private ChipmunkBodyC m_leftAnkleCBC;

	private ChipmunkBodyC m_leftArmCBC;

	private ChipmunkBodyC m_leftAlbowCBC;

	private ChipmunkBodyC m_leftHandCBC;

	private ChipmunkBodyC m_rightLegCBC;

	private ChipmunkBodyC m_rightKneeCBC;

	private ChipmunkBodyC m_rightArmCBC;

	private ChipmunkBodyC m_rightAlbowCBC;

	private ChipmunkBodyC m_rightHandCBC;

	private Quaternion m_hipsRotationOffset;

	private Quaternion m_spineRotationOffset;

	private Quaternion m_spine1RotationOffset;

	private Quaternion m_neckRotationOffset;

	private Quaternion m_headRotationOffset;

	private Quaternion m_leftLegRotationOffset;

	private Quaternion m_leftKneeRotationOffset;

	private Quaternion m_leftArmRotationOffset;

	private Quaternion m_leftAlbowRotationOffset;

	private Quaternion m_leftHandRotationOffset;

	private Quaternion m_rightLegRotationOffset;

	private Quaternion m_rightKneeRotationOffset;

	private Quaternion m_rightArmRotationOffset;

	private Quaternion m_rightAlbowRotationOffset;

	private Quaternion m_rightHandRotationOffset;

	private static string m_hipsPath = "joint_grp/deform_Hip";

	private static string m_spinePath = "joint_grp/deform_Hip/deform_Spine";

	private static string m_spine1Path = "joint_grp/deform_Hip/deform_Spine/deform_Spine1";

	private static string m_neckPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_Neck";

	private static string m_headPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_Neck/deform_Head";

	private static string m_rightLegPath = "joint_grp/deform_Hip/deform_right_Leg";

	private static string m_rightKneePath = "joint_grp/deform_Hip/deform_right_Leg/deform_right_Knee";

	private static string m_rightArmPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_right_Shoulder/deform_right_Arm";

	private static string m_rightAlbowPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_right_Shoulder/deform_right_Arm/deform_right_Albow";

	private static string m_rightHandPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_right_Shoulder/deform_right_Arm/deform_right_Albow/deform_right_Hand";

	private static string m_leftLegPath = "joint_grp/deform_Hip/deform_left_Leg";

	private static string m_leftKneePath = "joint_grp/deform_Hip/deform_left_Leg/deform_left_Knee";

	private static string m_leftArmPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_left_Shoulder/deform_left_Arm";

	private static string m_leftAlbowPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_left_Shoulder/deform_left_Arm/deform_left_Albow";

	private static string m_leftHandPath = "joint_grp/deform_Hip/deform_Spine/deform_Spine1/deform_left_Shoulder/deform_left_Arm/deform_left_Albow/deform_left_Hand";

	private AnimationCurve m_hipsCurvePosX;

	private AnimationCurve m_hipsCurvePosY;

	private AnimationCurve m_hipsCurvePosZ;

	private AnimationCurve m_hipsCurveRotX;

	private AnimationCurve m_hipsCurveRotY;

	private AnimationCurve m_hipsCurveRotZ;

	private AnimationCurve m_hipsCurveRotW;

	private AnimationCurve m_spineCurveRotX;

	private AnimationCurve m_spineCurveRotY;

	private AnimationCurve m_spineCurveRotZ;

	private AnimationCurve m_spineCurveRotW;

	private AnimationCurve m_spine1CurveRotX;

	private AnimationCurve m_spine1CurveRotY;

	private AnimationCurve m_spine1CurveRotZ;

	private AnimationCurve m_spine1CurveRotW;

	private AnimationCurve m_neckCurveRotX;

	private AnimationCurve m_neckCurveRotY;

	private AnimationCurve m_neckCurveRotZ;

	private AnimationCurve m_neckCurveRotW;

	private AnimationCurve m_headCurveRotX;

	private AnimationCurve m_headCurveRotY;

	private AnimationCurve m_headCurveRotZ;

	private AnimationCurve m_headCurveRotW;

	private AnimationCurve m_leftLegCurveRotX;

	private AnimationCurve m_leftLegCurveRotY;

	private AnimationCurve m_leftLegCurveRotZ;

	private AnimationCurve m_leftLegCurveRotW;

	private AnimationCurve m_leftKneeCurveRotX;

	private AnimationCurve m_leftKneeCurveRotY;

	private AnimationCurve m_leftKneeCurveRotZ;

	private AnimationCurve m_leftKneeCurveRotW;

	private AnimationCurve m_leftArmCurveRotX;

	private AnimationCurve m_leftArmCurveRotY;

	private AnimationCurve m_leftArmCurveRotZ;

	private AnimationCurve m_leftArmCurveRotW;

	private AnimationCurve m_leftAlbowCurveRotX;

	private AnimationCurve m_leftAlbowCurveRotY;

	private AnimationCurve m_leftAlbowCurveRotZ;

	private AnimationCurve m_leftAlbowCurveRotW;

	private AnimationCurve m_leftHandCurveRotX;

	private AnimationCurve m_leftHandCurveRotY;

	private AnimationCurve m_leftHandCurveRotZ;

	private AnimationCurve m_leftHandCurveRotW;

	private AnimationCurve m_rightLegCurveRotX;

	private AnimationCurve m_rightLegCurveRotY;

	private AnimationCurve m_rightLegCurveRotZ;

	private AnimationCurve m_rightLegCurveRotW;

	private AnimationCurve m_rightKneeCurveRotX;

	private AnimationCurve m_rightKneeCurveRotY;

	private AnimationCurve m_rightKneeCurveRotZ;

	private AnimationCurve m_rightKneeCurveRotW;

	private AnimationCurve m_rightArmCurveRotX;

	private AnimationCurve m_rightArmCurveRotY;

	private AnimationCurve m_rightArmCurveRotZ;

	private AnimationCurve m_rightArmCurveRotW;

	private AnimationCurve m_rightAlbowCurveRotX;

	private AnimationCurve m_rightAlbowCurveRotY;

	private AnimationCurve m_rightAlbowCurveRotZ;

	private AnimationCurve m_rightAlbowCurveRotW;

	private AnimationCurve m_rightHandCurveRotX;

	private AnimationCurve m_rightHandCurveRotY;

	private AnimationCurve m_rightHandCurveRotZ;

	private AnimationCurve m_rightHandCurveRotW;

	public AlienCharacter(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_group = (uint)(m_entity.m_index + 1000);
		Entity entity = EntityManager.AddEntity("AlienCharacter");
		m_assembledEntities.Add(entity);
		m_mainTC = TransformS.AddComponent(entity);
		TransformS.SetTransform(m_mainTC, _graphElement.m_position, _graphElement.m_rotation);
		m_mainPC = PrefabS.AddComponent(m_mainTC, Vector3.zero, ResourceManager.GetGameObject("Units/test"));
		PrefabS.SetCamera(m_mainPC, CameraS.m_mainCamera);
		m_overrideRagdollAnimation = m_mainPC.p_gameObject.AddComponent<Animation>();
		m_overrideRagdollClip = new AnimationClip();
		m_overrideRagdollAnimation.AddClip(m_overrideRagdollClip, "ragdoll");
		m_overrideRagdollAnimation.Play("ragdoll");
		m_hipsCurvePosX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurvePosY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurvePosZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hipsCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spineCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spineCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spineCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spineCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spine1CurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spine1CurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spine1CurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_spine1CurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_neckCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_neckCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_neckCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_neckCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_headCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_headCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_headCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_headCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftLegCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftLegCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftLegCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftLegCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftKneeCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftKneeCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftKneeCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftKneeCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftArmCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftArmCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftArmCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftArmCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftAlbowCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftAlbowCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftAlbowCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftAlbowCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftHandCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftHandCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftHandCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_leftHandCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightLegCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightLegCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightLegCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightLegCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightKneeCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightKneeCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightKneeCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightKneeCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightArmCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightArmCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightArmCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightArmCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightAlbowCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightAlbowCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightAlbowCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightAlbowCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightHandCurveRotX = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightHandCurveRotY = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightHandCurveRotZ = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_rightHandCurveRotW = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
		m_hips = m_mainPC.p_gameObject.transform.Find(m_hipsPath);
		m_spine = m_mainPC.p_gameObject.transform.Find(m_spinePath);
		m_spine1 = m_mainPC.p_gameObject.transform.Find(m_spine1Path);
		m_neck = m_mainPC.p_gameObject.transform.Find(m_neckPath);
		m_head = m_mainPC.p_gameObject.transform.Find(m_headPath);
		m_rightLeg = m_mainPC.p_gameObject.transform.Find(m_rightLegPath);
		m_rightKnee = m_mainPC.p_gameObject.transform.Find(m_rightKneePath);
		m_rightArm = m_mainPC.p_gameObject.transform.Find(m_rightArmPath);
		m_rightAlbow = m_mainPC.p_gameObject.transform.Find(m_rightAlbowPath);
		m_rightHand = m_mainPC.p_gameObject.transform.Find(m_rightHandPath);
		m_leftLeg = m_mainPC.p_gameObject.transform.Find(m_leftLegPath);
		m_leftKnee = m_mainPC.p_gameObject.transform.Find(m_leftKneePath);
		m_leftArm = m_mainPC.p_gameObject.transform.Find(m_leftArmPath);
		m_leftAlbow = m_mainPC.p_gameObject.transform.Find(m_leftAlbowPath);
		m_leftHand = m_mainPC.p_gameObject.transform.Find(m_leftHandPath);
		m_hipsRotationOffset = m_hips.rotation;
		m_spineRotationOffset = m_spine.localRotation;
		m_spine1RotationOffset = m_spine1.localRotation;
		m_neckRotationOffset = m_neck.localRotation;
		m_headRotationOffset = m_head.localRotation;
		m_rightLegRotationOffset = m_rightLeg.localRotation;
		m_rightKneeRotationOffset = m_rightKnee.localRotation;
		m_rightArmRotationOffset = m_rightArm.localRotation;
		m_rightAlbowRotationOffset = m_rightAlbow.localRotation;
		m_rightHandRotationOffset = m_rightHand.localRotation;
		m_leftLegRotationOffset = m_leftLeg.localRotation;
		m_leftKneeRotationOffset = m_leftKnee.localRotation;
		m_leftArmRotationOffset = m_leftArm.localRotation;
		m_leftAlbowRotationOffset = m_leftAlbow.localRotation;
		m_leftHandRotationOffset = m_leftHand.localRotation;
		m_mainPC.p_gameObject.transform.localRotation = Quaternion.Euler(Vector3.up * 90f);
		float elasticity = 0.25f;
		float friction = 1f;
		float num = 1f;
		float value = 10000f;
		float value2 = 75000f;
		TransformC transformC = TransformS.AddComponent(entity, m_hips.position);
		m_hipsCBC = ChipmunkProS.AddDynamicBody(transformC, new ucpCircleShape(5f, new Vector2(3f, 0f), 5f)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_hipsCBC.body, num);
		TransformC transformC2 = TransformS.AddComponent(entity, m_spine.position + new Vector3(2.75f, 2f));
		m_spineCBC = ChipmunkProS.AddDynamicBody(transformC2, new ucpCircleShape(4f, Vector2.zero, 4f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_spineCBC.body, num);
		IntPtr constraint = ChipmunkProWrapper.ucpPivotJointNew(m_hipsCBC.body, m_spineCBC.body, m_spine.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint, -1);
		IntPtr constraint2 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_hipsCBC.body, m_spineCBC.body, -0.61086524f, 0.61086524f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint2, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint2, -1);
		TransformC transformC3 = TransformS.AddComponent(entity, m_spine1.position + new Vector3(1.5f, 2.5f));
		m_spine1CBC = ChipmunkProS.AddDynamicBody(transformC3, new ucpCircleShape(3f, Vector2.zero, 3f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_spine1CBC.body, num * 2f);
		IntPtr constraint3 = ChipmunkProWrapper.ucpPivotJointNew(m_spineCBC.body, m_spine1CBC.body, m_spine1.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint3, -1);
		IntPtr constraint4 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_spineCBC.body, m_spine1CBC.body, -(float)Math.PI / 12f, (float)Math.PI / 12f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint4, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint4, -1);
		TransformC transformC4 = TransformS.AddComponent(entity, m_neck.position + new Vector3(0.5f, 1f));
		m_neckCBC = ChipmunkProS.AddDynamicBody(transformC4, new ucpCircleShape(2f, Vector2.zero, 2f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_neckCBC.body, num);
		IntPtr constraint5 = ChipmunkProWrapper.ucpPivotJointNew(m_spine1CBC.body, m_neckCBC.body, m_neck.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint5, -1);
		IntPtr constraint6 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_spine1CBC.body, m_neckCBC.body, -0.34906584f, 0.6981317f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint6, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint6, -1);
		TransformC transformC5 = TransformS.AddComponent(entity, m_head.position + new Vector3(0f, 8f));
		ucpCircleShape ucpCircleShape2 = new ucpCircleShape(16f, new Vector2(-5f, 11f), 5f, elasticity, friction)
		{
			group = 0u
		};
		ucpCircleShape ucpCircleShape3 = new ucpCircleShape(7f, new Vector2(6f, -6f), 3f, elasticity, friction)
		{
			group = 0u
		};
		m_headCBC = ChipmunkProS.AddDynamicBody(transformC5, new ucpShape[2] { ucpCircleShape2, ucpCircleShape3 }, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_headCBC.body, num);
		IntPtr constraint7 = ChipmunkProWrapper.ucpPivotJointNew(m_neckCBC.body, m_headCBC.body, m_head.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint7, -1);
		TransformC transformC6 = TransformS.AddComponent(entity, m_leftLeg.position + new Vector3(0f, -3f));
		m_leftLegCBC = ChipmunkProS.AddDynamicBody(transformC6, new ucpCircleShape(2.5f, Vector2.zero, 2.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_leftLegCBC.body, num);
		IntPtr constraint8 = ChipmunkProWrapper.ucpPivotJointNew(m_hipsCBC.body, m_leftLegCBC.body, m_leftLeg.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint8, -1);
		IntPtr constraint9 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_hipsCBC.body, m_leftLegCBC.body, -0.34906584f, (float)Math.PI / 2f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint9, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint9, -1);
		TransformC transformC7 = TransformS.AddComponent(entity, m_leftKnee.position + new Vector3(0f, -2.5f));
		m_leftKneeCBC = ChipmunkProS.AddDynamicBody(transformC7, new ucpCircleShape(2.5f, Vector2.zero, 2.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_leftKneeCBC.body, num);
		IntPtr constraint10 = ChipmunkProWrapper.ucpPivotJointNew(m_leftLegCBC.body, m_leftKneeCBC.body, m_leftKnee.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint10, -1);
		IntPtr constraint11 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_leftLegCBC.body, m_leftKneeCBC.body, -(float)Math.PI / 2f, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint11, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint11, -1);
		TransformC transformC8 = TransformS.AddComponent(entity, m_rightLeg.position + new Vector3(0f, -3f));
		m_rightLegCBC = ChipmunkProS.AddDynamicBody(transformC8, new ucpCircleShape(2.5f, Vector2.zero, 2.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rightLegCBC.body, num);
		IntPtr constraint12 = ChipmunkProWrapper.ucpPivotJointNew(m_hipsCBC.body, m_rightLegCBC.body, m_rightLeg.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint12, -1);
		IntPtr constraint13 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_hipsCBC.body, m_rightLegCBC.body, -0.34906584f, (float)Math.PI / 2f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint13, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint13, -1);
		TransformC transformC9 = TransformS.AddComponent(entity, m_rightKnee.position + new Vector3(0f, -2.5f));
		m_rightKneeCBC = ChipmunkProS.AddDynamicBody(transformC9, new ucpCircleShape(2.5f, Vector2.zero, 3f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rightKneeCBC.body, num);
		IntPtr constraint14 = ChipmunkProWrapper.ucpPivotJointNew(m_rightLegCBC.body, m_rightKneeCBC.body, m_rightKnee.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint14, -1);
		IntPtr constraint15 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_rightLegCBC.body, m_rightKneeCBC.body, -(float)Math.PI / 2f, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint15, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint15, -1);
		TransformC transformC10 = TransformS.AddComponent(entity, m_leftArm.position + new Vector3(0f, -3f));
		m_leftArmCBC = ChipmunkProS.AddDynamicBody(transformC10, new ucpPolyShape(2.5f, 6f, Vector2.zero, 4.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_leftArmCBC.body, num);
		IntPtr constraint16 = ChipmunkProWrapper.ucpPivotJointNew(m_spine1CBC.body, m_leftArmCBC.body, m_leftArm.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint16, -1);
		TransformC transformC11 = TransformS.AddComponent(entity, m_leftAlbow.position + new Vector3(0f, -3f));
		m_leftAlbowCBC = ChipmunkProS.AddDynamicBody(transformC11, new ucpPolyShape(2.5f, 6f, Vector2.zero, 4.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_leftAlbowCBC.body, num);
		IntPtr constraint17 = ChipmunkProWrapper.ucpPivotJointNew(m_leftArmCBC.body, m_leftAlbowCBC.body, m_leftAlbow.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint17, -1);
		IntPtr constraint18 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_leftArmCBC.body, m_leftAlbowCBC.body, 0f, 2.6179938f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint18, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint18, -1);
		TransformC transformC12 = TransformS.AddComponent(entity, m_leftHand.position + new Vector3(0f, -3f));
		m_leftHandCBC = ChipmunkProS.AddDynamicBody(transformC12, new ucpCircleShape(3f, Vector2.zero, 3f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_leftHandCBC.body, num);
		IntPtr constraint19 = ChipmunkProWrapper.ucpPivotJointNew(m_leftAlbowCBC.body, m_leftHandCBC.body, m_leftHand.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint19, -1);
		IntPtr constraint20 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_leftAlbowCBC.body, m_leftHandCBC.body, -(float)Math.PI / 4f, (float)Math.PI / 4f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint20, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint20, -1);
		TransformC transformC13 = TransformS.AddComponent(entity, m_rightArm.position + new Vector3(0f, -3f));
		m_rightArmCBC = ChipmunkProS.AddDynamicBody(transformC13, new ucpPolyShape(2.5f, 6f, Vector2.zero, 4.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rightArmCBC.body, num);
		IntPtr constraint21 = ChipmunkProWrapper.ucpPivotJointNew(m_spine1CBC.body, m_rightArmCBC.body, m_rightArm.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint21, -1);
		IntPtr constraint22 = ChipmunkProWrapper.ucpSimpleMotorNew(m_spine1CBC.body, m_rightArmCBC.body, 0f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint22, value);
		ChipmunkProWrapper.ucpAddConstraint(constraint22, -1);
		TransformC transformC14 = TransformS.AddComponent(entity, m_rightAlbow.position + new Vector3(0f, -3f));
		m_rightAlbowCBC = ChipmunkProS.AddDynamicBody(transformC14, new ucpPolyShape(2.5f, 6f, Vector2.zero, 4.5f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rightAlbowCBC.body, num);
		IntPtr constraint23 = ChipmunkProWrapper.ucpPivotJointNew(m_rightArmCBC.body, m_rightAlbowCBC.body, m_rightAlbow.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint23, -1);
		IntPtr constraint24 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_rightArmCBC.body, m_rightAlbowCBC.body, 0f, 2.6179938f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint24, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint24, -1);
		TransformC transformC15 = TransformS.AddComponent(entity, m_rightHand.position + new Vector3(0f, -3f));
		m_rightHandCBC = ChipmunkProS.AddDynamicBody(transformC15, new ucpCircleShape(3f, Vector2.zero, 2f, elasticity, friction)
		{
			group = m_group
		}, m_unitC);
		ChipmunkProWrapper.ucpBodySetAngularDamp(m_rightHandCBC.body, num);
		IntPtr constraint25 = ChipmunkProWrapper.ucpPivotJointNew(m_rightAlbowCBC.body, m_rightHandCBC.body, m_rightHand.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint25, -1);
		IntPtr constraint26 = ChipmunkProWrapper.ucpRotaryLimitJointNew(m_rightAlbowCBC.body, m_rightHandCBC.body, -(float)Math.PI / 4f, (float)Math.PI / 4f);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint26, value2);
		ChipmunkProWrapper.ucpAddConstraint(constraint26, -1);
		m_mainPC.p_gameObject.transform.localRotation = Quaternion.identity;
		DebugDraw.m_lineWidth = 0.5f;
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC, Vector3.forward * -10f + new Vector3(3f, 0f), 5f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC2, Vector3.forward * -10f, 4f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC3, Vector3.forward * -10f, 3f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC4, Vector3.forward * -10f, 2f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC5, new Vector3(-5f, 11f, -10f), 16f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC5, new Vector3(6f, -6f, -10f), 7f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC6, Vector3.zero, 2.5f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC7, Vector3.zero, 2.5f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC8, Vector3.forward * -10f, 2.5f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC9, Vector3.forward * -10f, 2.5f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformC10, Vector3.forward * -10f, 2.5f, 6f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformC11, Vector3.forward * -10f, 2.5f, 6f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC12, Vector3.forward * -10f, 3f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformC13, Vector3.forward * -10f, 2.5f, 6f);
		DebugDraw.CreateBox(CameraS.m_mainCamera, transformC14, Vector3.forward * -10f, 2.5f, 6f);
		DebugDraw.CreateCircle(CameraS.m_mainCamera, transformC15, Vector3.forward * -10f, 3f);
		CreateEditorTouchArea();
	}

	public override void Update()
	{
		base.Update();
		if (PsState.m_gameState == GameState.Play || PsState.m_gameState == GameState.Test)
		{
			m_hips.transform.position = m_hipsCBC.TC.transform.position;
			AnimationClip overrideRagdollClip = m_overrideRagdollClip;
			Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, m_hipsCBC.TC.transform.eulerAngles.z));
			Quaternion rotation2 = Quaternion.Euler(m_spineCBC.TC.transform.rotation.eulerAngles - m_hipsCBC.TC.transform.rotation.eulerAngles) * m_spineRotationOffset;
			Quaternion rotation3 = Quaternion.Euler(m_spine1CBC.TC.transform.rotation.eulerAngles - m_spineCBC.TC.transform.rotation.eulerAngles) * m_spine1RotationOffset;
			Quaternion rotation4 = Quaternion.Euler(m_neckCBC.TC.transform.rotation.eulerAngles - m_spine1CBC.TC.transform.rotation.eulerAngles) * m_neckRotationOffset;
			Quaternion rotation5 = Quaternion.Euler(m_headCBC.TC.transform.rotation.eulerAngles - m_neckCBC.TC.transform.rotation.eulerAngles) * m_headRotationOffset;
			Quaternion rotation6 = Quaternion.Euler(m_leftLegCBC.TC.transform.rotation.eulerAngles - m_hipsCBC.TC.transform.rotation.eulerAngles) * m_leftLegRotationOffset;
			Quaternion rotation7 = Quaternion.Euler(m_leftKneeCBC.TC.transform.rotation.eulerAngles - m_leftLegCBC.TC.transform.rotation.eulerAngles) * m_leftKneeRotationOffset;
			Quaternion rotation8 = Quaternion.Euler(0f, -75f, (m_leftArmCBC.TC.transform.rotation.eulerAngles - m_spine1CBC.TC.transform.rotation.eulerAngles).z);
			Quaternion rotation9 = Quaternion.Euler(m_leftAlbowCBC.TC.transform.rotation.eulerAngles - m_leftArmCBC.TC.transform.rotation.eulerAngles) * m_leftAlbowRotationOffset;
			Quaternion rotation10 = Quaternion.Euler(m_leftHandCBC.TC.transform.rotation.eulerAngles - m_leftAlbowCBC.TC.transform.rotation.eulerAngles) * m_leftHandRotationOffset;
			Quaternion rotation11 = Quaternion.Euler(m_rightLegCBC.TC.transform.rotation.eulerAngles - m_hipsCBC.TC.transform.rotation.eulerAngles) * m_rightLegRotationOffset;
			Quaternion rotation12 = Quaternion.Euler(m_rightKneeCBC.TC.transform.rotation.eulerAngles - m_rightLegCBC.TC.transform.rotation.eulerAngles) * m_rightKneeRotationOffset;
			Quaternion rotation13 = Quaternion.Euler(0f, -75f, (m_rightArmCBC.TC.transform.rotation.eulerAngles - m_spine1CBC.TC.transform.rotation.eulerAngles).z);
			Quaternion rotation14 = Quaternion.Euler(m_rightAlbowCBC.TC.transform.rotation.eulerAngles - m_rightArmCBC.TC.transform.rotation.eulerAngles) * m_rightAlbowRotationOffset;
			Quaternion rotation15 = Quaternion.Euler(m_rightHandCBC.TC.transform.rotation.eulerAngles - m_rightAlbowCBC.TC.transform.rotation.eulerAngles) * m_rightHandRotationOffset;
			SetRotationCurves(overrideRagdollClip, m_hipsPath, m_hipsCurveRotX, m_hipsCurveRotY, m_hipsCurveRotZ, m_hipsCurveRotW, rotation);
			SetRotationCurves(overrideRagdollClip, m_spinePath, m_spineCurveRotX, m_spineCurveRotY, m_spineCurveRotZ, m_spineCurveRotW, rotation2);
			SetRotationCurves(overrideRagdollClip, m_spine1Path, m_spine1CurveRotX, m_spine1CurveRotY, m_spine1CurveRotZ, m_spine1CurveRotW, rotation3);
			SetRotationCurves(overrideRagdollClip, m_neckPath, m_neckCurveRotX, m_neckCurveRotY, m_neckCurveRotZ, m_neckCurveRotW, rotation4);
			SetRotationCurves(overrideRagdollClip, m_headPath, m_headCurveRotX, m_headCurveRotY, m_headCurveRotZ, m_headCurveRotW, rotation5);
			SetRotationCurves(overrideRagdollClip, m_leftLegPath, m_leftLegCurveRotX, m_leftLegCurveRotY, m_leftLegCurveRotZ, m_leftLegCurveRotW, rotation6);
			SetRotationCurves(overrideRagdollClip, m_leftKneePath, m_leftKneeCurveRotX, m_leftKneeCurveRotY, m_leftKneeCurveRotZ, m_leftKneeCurveRotW, rotation7);
			SetRotationCurves(overrideRagdollClip, m_leftArmPath, m_leftArmCurveRotX, m_leftArmCurveRotY, m_leftArmCurveRotZ, m_leftArmCurveRotW, rotation8);
			SetRotationCurves(overrideRagdollClip, m_leftAlbowPath, m_leftAlbowCurveRotX, m_leftAlbowCurveRotY, m_leftAlbowCurveRotZ, m_leftAlbowCurveRotW, rotation9);
			SetRotationCurves(overrideRagdollClip, m_leftHandPath, m_leftHandCurveRotX, m_leftHandCurveRotY, m_leftHandCurveRotZ, m_leftHandCurveRotW, rotation10);
			SetRotationCurves(overrideRagdollClip, m_rightLegPath, m_rightLegCurveRotX, m_rightLegCurveRotY, m_rightLegCurveRotZ, m_rightLegCurveRotW, rotation11);
			SetRotationCurves(overrideRagdollClip, m_rightKneePath, m_rightKneeCurveRotX, m_rightKneeCurveRotY, m_rightKneeCurveRotZ, m_rightKneeCurveRotW, rotation12);
			SetRotationCurves(overrideRagdollClip, m_rightArmPath, m_rightArmCurveRotX, m_rightArmCurveRotY, m_rightArmCurveRotZ, m_rightArmCurveRotW, rotation13);
			SetRotationCurves(overrideRagdollClip, m_rightAlbowPath, m_rightAlbowCurveRotX, m_rightAlbowCurveRotY, m_rightAlbowCurveRotZ, m_rightAlbowCurveRotW, rotation14);
			SetRotationCurves(overrideRagdollClip, m_rightHandPath, m_rightHandCurveRotX, m_rightHandCurveRotY, m_rightHandCurveRotZ, m_rightHandCurveRotW, rotation15);
		}
	}

	private void SetRotationCurves(AnimationClip _clip, string _targetPath, AnimationCurve _x, AnimationCurve _y, AnimationCurve _z, AnimationCurve _w, Quaternion _rotation)
	{
		_x.MoveKey(0, new Keyframe(0f, _rotation.x));
		_x.MoveKey(1, new Keyframe(1f, _rotation.x));
		_y.MoveKey(0, new Keyframe(0f, _rotation.y));
		_y.MoveKey(1, new Keyframe(1f, _rotation.y));
		_z.MoveKey(0, new Keyframe(0f, _rotation.z));
		_z.MoveKey(1, new Keyframe(1f, _rotation.z));
		_w.MoveKey(0, new Keyframe(0f, _rotation.w));
		_w.MoveKey(1, new Keyframe(1f, _rotation.w));
		_clip.SetCurve(_targetPath, typeof(Transform), "localRotation.x", _x);
		_clip.SetCurve(_targetPath, typeof(Transform), "localRotation.y", _y);
		_clip.SetCurve(_targetPath, typeof(Transform), "localRotation.z", _z);
		_clip.SetCurve(_targetPath, typeof(Transform), "localRotation.w", _w);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
