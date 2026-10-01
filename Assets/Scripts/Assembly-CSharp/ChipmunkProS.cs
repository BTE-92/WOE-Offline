using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChipmunkProS
{
	public const bool CHIPMUNK_DEBUG_DRAW = false;

	public static DynamicArray<ChipmunkBodyC> m_bodies;

	public static DynamicArray<ChipmunkConstraintC> m_constraints;

	private static int m_maxRemovedConstraintResultsCount = 50;

	private static ucpConstraintData[] m_removedConstraintResults = new ucpConstraintData[m_maxRemovedConstraintResultsCount];

	private static CollisionDelegate[,,] m_globalCollisionDelegates;

	private static uint[,] m_collisionDelegatePhases;

	private static int[,] m_collisionDelegateCounts;

	public static int m_collisionTypeCount;

	public static ChipmunkBodyC m_staticBody;

	public static TransformC m_staticBodyTC;

	public static void Initialize(int _collisionTypeCount)
	{
		m_collisionTypeCount = _collisionTypeCount + 1;
		m_bodies = new DynamicArray<ChipmunkBodyC>();
		m_constraints = new DynamicArray<ChipmunkConstraintC>();
		ChipmunkProWrapper.ucpInitialize();
		ChipmunkProWrapper.ucpSpaceSetGravity(Vector2.up * -400f);
		ChipmunkProWrapper.ucpSpaceSetCollisionBias(0.001f);
		Entity entity = EntityManager.AddEntity();
		entity.m_persistent = true;
		m_staticBodyTC = TransformS.AddComponent(entity, "Chipmunk Static Body", Vector3.zero);
		m_staticBody = AddStaticBody(m_staticBodyTC);
		m_globalCollisionDelegates = new CollisionDelegate[m_collisionTypeCount, m_collisionTypeCount, 3];
		m_collisionDelegatePhases = new uint[m_collisionTypeCount, m_collisionTypeCount];
		m_collisionDelegateCounts = new int[m_collisionTypeCount, m_collisionTypeCount];
	}

	public static IntPtr AddShapeToBody(ChipmunkBodyC _cmb, ucpShape _shape, float _currentMass, float _currentMoment)
	{
		IntPtr intPtr = IntPtr.Zero;
		float num = _currentMoment;
		if (_shape.shapeType == ucpShapeType.Circle)
		{
			ucpCircleShape ucpCircleShape2 = _shape as ucpCircleShape;
			num += ChipmunkProWrapper.ucpMomentForCircle(ucpCircleShape2.mass, ucpCircleShape2.innerRadius, ucpCircleShape2.outerRadius, ucpCircleShape2.offset);
			intPtr = ChipmunkProWrapper.ucpCircleShapeNew(_cmb.body, ucpCircleShape2.outerRadius, ucpCircleShape2.offset, ucpCircleShape2.collisionType);
		}
		else if (_shape.shapeType == ucpShapeType.Poly)
		{
			ucpPolyShape ucpPolyShape2 = _shape as ucpPolyShape;
			num += ChipmunkProWrapper.ucpMomentForBox(ucpPolyShape2.mass, ucpPolyShape2.width, ucpPolyShape2.height);
			intPtr = ChipmunkProWrapper.ucpPolyShapeNew(_cmb.body, ucpPolyShape2.numVerts, ucpPolyShape2.verts, ucpPolyShape2.offset, ucpPolyShape2.collisionType);
		}
		else if (_shape.shapeType == ucpShapeType.Segment)
		{
			ucpSegmentShape ucpSegmentShape2 = _shape as ucpSegmentShape;
			num += ChipmunkProWrapper.ucpMomentForSegment(ucpSegmentShape2.mass, ucpSegmentShape2.a, ucpSegmentShape2.b);
			intPtr = ChipmunkProWrapper.ucpSegmentShapeNew(_cmb.body, ucpSegmentShape2.a, ucpSegmentShape2.b, ucpSegmentShape2.radius, ucpSegmentShape2.collisionType);
		}
		ChipmunkProWrapper.ucpSpaceAddShape(intPtr);
		if (!ChipmunkProWrapper.ucpBodyIsStatic(_cmb.body) && !ChipmunkProWrapper.ucpBodyIsRogue(_cmb.body))
		{
			ChipmunkProWrapper.ucpBodySetMass(_cmb.body, _currentMass + _shape.mass);
			_cmb.m_mass = _currentMass + _shape.mass;
			ChipmunkProWrapper.ucpBodySetMoment(_cmb.body, num);
			_cmb.m_moment = num;
		}
		ChipmunkProWrapper.ucpShapeSetSensor(intPtr, _shape.sensor);
		ChipmunkProWrapper.ucpShapeSetElasticity(intPtr, _shape.elasticity);
		ChipmunkProWrapper.ucpShapeSetFriction(intPtr, _shape.friction);
		ChipmunkProWrapper.ucpShapeSetGroup(intPtr, _shape.group);
		ChipmunkProWrapper.ucpShapeSetLayers(intPtr, _shape.layers);
		ChipmunkProWrapper.ucpShapeSetSurfaceVelocity(intPtr, _shape.surfaceVelocity);
		_cmb.shapes.Add(intPtr);
		_shape.shapePtr = intPtr;
		return intPtr;
	}

	public static ChipmunkBodyC AddDynamicBody(TransformC _tc, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddDynamicBody(_tc, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddDynamicBody(TransformC _tc, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _tc;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddBody(1f, 1f, chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, _tc.transform.position);
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, _tc.transform.rotation.eulerAngles.z * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				float currentMass = 0f;
				float currentMoment = 0f;
				if (chipmunkBodyC.shapes.Count > 0)
				{
					currentMass = ChipmunkProWrapper.ucpBodyGetMass(chipmunkBodyC.body);
					currentMoment = ChipmunkProWrapper.ucpBodyGetMoment(chipmunkBodyC.body);
				}
				AddShapeToBody(chipmunkBodyC, _shapes[i], currentMass, currentMoment);
			}
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static ChipmunkBodyC AddRogueBody(TransformC _tc, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddRogueBody(_tc, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddRogueBody(TransformC _tc, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _tc;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddRogueBody(chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, _tc.transform.position);
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, _tc.transform.rotation.eulerAngles.z * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				float currentMass = 0f;
				float currentMoment = 0f;
				if (chipmunkBodyC.shapes.Count > 0)
				{
					currentMass = ChipmunkProWrapper.ucpBodyGetMass(chipmunkBodyC.body);
					currentMoment = ChipmunkProWrapper.ucpBodyGetMoment(chipmunkBodyC.body);
				}
				AddShapeToBody(chipmunkBodyC, _shapes[i], currentMass, currentMoment);
			}
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, IComponent _customComponent = null)
	{
		ucpShape[] shapes = null;
		return AddStaticBody(_transformComponent, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddStaticBody(_transformComponent, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _transformComponent;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddStaticBody(chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position);
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, chipmunkBodyC.TC.transform.eulerAngles.z * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				AddShapeToBody(chipmunkBodyC, _shapes[i], 0f, 0f);
			}
		}
		EntityManager.AddComponentToEntity(_transformComponent.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static ChipmunkConstraintC AddDampedRotarySpring(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _restAngle, float _stiffness, float _damping)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpDampedRotarySpringNew(_bodyA.body, _bodyB.body, _restAngle, _stiffness, _damping);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.type = ucpConstraintType.DampedRotarySpring;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddDampedSpring(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2, float _restLength, float _stiffness, float _damping)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpDampedSpringNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition, _restLength, _stiffness, _damping);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.DampedSpring;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddGearJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _phase, float _ratio)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpGearJointNew(_bodyA.body, _bodyB.body, _phase, _ratio);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.GearJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddGrooveJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _grooveA, TransformC _grooveB, TransformC _anchor)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_grooveA, _bodyA.TC);
		TransformS.ParentComponent(_grooveB, _bodyA.TC);
		TransformS.ParentComponent(_anchor, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpGrooveJointNew(_bodyA.body, _bodyB.body, _grooveA.transform.localPosition, _grooveB.transform.localPosition, _anchor.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.GrooveJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.grooveA = _grooveA;
		chipmunkConstraintC.grooveB = _grooveB;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPinJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpPinJointNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PinJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPivotJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpPivotJointNew(_bodyA.body, _bodyB.body, _anchor.transform.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PivotJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPivotJoint2(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpPivotJointNew2(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PivotJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddRatchetJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _phase, float _ratchet)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpRatchetJointNew(_bodyA.body, _bodyB.body, _phase, _ratchet);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.RatchetJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddRotaryLimitJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _min, float _max)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpRotaryLimitJointNew(_bodyA.body, _bodyB.body, _min, _max);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.RotaryLimitJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddSimpleMotor(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _rate, float _maxForce)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpSimpleMotorNew(_bodyA.body, _bodyB.body, _rate);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint, _maxForce);
		chipmunkConstraintC.type = ucpConstraintType.SimpleMotor;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddSlideJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2, float _min, float _max)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpSlideJointNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition, _min, _max);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.SlideJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static void RemoveBody(ChipmunkBodyC _c)
	{
		ChipmunkProWrapper.ucpClearCollisionLists();
		int num = ChipmunkProWrapper.ucpRemoveBody(_c.body, m_removedConstraintResults, m_maxRemovedConstraintResultsCount);
		HandleCollisionEvents();
		_c.TC = null;
		_c.customComponent = null;
		for (int i = 0; i < num; i++)
		{
			if (m_removedConstraintResults[i].ucpComponentIndex > -1)
			{
				ChipmunkConstraintC chipmunkConstraintC = m_constraints.m_array[m_removedConstraintResults[i].ucpComponentIndex];
				chipmunkConstraintC.constraint = IntPtr.Zero;
				RemoveConstraint(chipmunkConstraintC);
			}
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_bodies.RemoveItem(_c);
	}

	public static void RemoveConstraintsFromBody(ChipmunkBodyC _c)
	{
		for (int num = m_constraints.m_aliveCount - 1; num > -1; num--)
		{
			ChipmunkConstraintC chipmunkConstraintC = m_constraints.m_array[m_constraints.m_aliveIndices[num]];
			if (chipmunkConstraintC.bodyA == _c || chipmunkConstraintC.bodyB == _c)
			{
				RemoveConstraint(chipmunkConstraintC);
			}
		}
	}

	public static void RemoveConstraint(ChipmunkConstraintC _c)
	{
		if (_c.constraint != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpRemoveConstraint(_c.constraint);
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_constraints.RemoveItem(_c);
	}

	public static void AddGlobalCollisionHandler(CollisionDelegate _collisionHandler, ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _handleBegin, bool _handlePersist, bool _handleSeparate)
	{
		if (_handleBegin)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 0] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num | 0x100;
		}
		if (_handlePersist)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 1] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num2 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num2 | 0x10;
		}
		if (_handleSeparate)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 2] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num3 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num3 | 1;
		}
		uint num4 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
		ChipmunkProWrapper.ucpSpaceAddCollisionHandler(_collisionTypeA, _collisionTypeB, (num4 & 0x100) == 256, (num4 & 0x10) == 16, (num4 & 1) == 1);
	}

	public static void AddCollisionHandler(ChipmunkBodyC _c, CollisionDelegate _collisionHandler, ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _handleBegin, bool _handlePersist, bool _handleSeparate)
	{
		if (_handleBegin)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 0] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 0] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num | 0x100;
		}
		if (_handlePersist)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 1] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 1] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num2 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num2 | 0x10;
		}
		if (_handleSeparate)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 2] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 2] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num3 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num3 | 1;
		}
		uint num4 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
		ChipmunkProWrapper.ucpSpaceAddCollisionHandler(_collisionTypeA, _collisionTypeB, (num4 & 0x100) == 256, (num4 & 0x10) == 16, (num4 & 1) == 1);
	}

	public static void RemoveGlobalCollisionHandler(ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _began, bool _persist, bool _separate)
	{
		if (_began)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 0] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
		if (_persist)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 1] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
		if (_separate)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 2] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
	}

	public static void RemoveCollisionHandler(ChipmunkBodyC _c, CollisionDelegate _collisionHandler)
	{
		CollisionDelegate[,] collisionDelegates = _c.m_collisionDelegates;
		for (int i = 0; i < collisionDelegates.GetLength(0); i++)
		{
			if (collisionDelegates[i, 0] == _collisionHandler)
			{
				collisionDelegates[i, 0] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 0], (ucpCollisionType)i);
				}
			}
			if (collisionDelegates[i, 1] == _collisionHandler)
			{
				collisionDelegates[i, 1] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 1], (ucpCollisionType)i);
				}
			}
			if (collisionDelegates[i, 2] == _collisionHandler)
			{
				collisionDelegates[i, 2] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 2], (ucpCollisionType)i);
				}
			}
		}
	}

	private static ucpCollisionPair ReverseCollisionPair(ucpCollisionPair _pair)
	{
		ucpCollisionPair result = _pair;
		result.shapeA = _pair.shapeB;
		result.shapeB = _pair.shapeA;
		result.ucpComponentIndexA = _pair.ucpComponentIndexB;
		result.ucpComponentIndexB = _pair.ucpComponentIndexA;
		result.normal = -_pair.normal;
		result.impulse = -_pair.impulse;
		result.friction = -_pair.friction;
		return result;
	}

	public static int HandleCollisionEvents()
	{
		int num = ChipmunkProWrapper.ucpGetBeginCollisionCount();
		ucpCollisionPair[] array = new ucpCollisionPair[num];
		ChipmunkProWrapper.ucpGetBeginCollisions(array, array.Length);
		int num2 = ChipmunkProWrapper.ucpGetPersistCollisionCount();
		ucpCollisionPair[] array2 = new ucpCollisionPair[num2];
		ChipmunkProWrapper.ucpGetPersistCollisions(array2, array2.Length);
		int num3 = ChipmunkProWrapper.ucpGetSeparateCollisionCount();
		ucpCollisionPair[] array3 = new ucpCollisionPair[num3];
		ChipmunkProWrapper.ucpGetSeparateCollisions(array3, array3.Length);
		for (int i = 0; i < num; i++)
		{
			ucpCollisionPair ucpCollisionPair2 = array[i];
			ChipmunkBodyC chipmunkBodyC = m_bodies.m_array[ucpCollisionPair2.ucpComponentIndexA];
			ChipmunkBodyC chipmunkBodyC2 = m_bodies.m_array[ucpCollisionPair2.ucpComponentIndexB];
			uint typeA = ucpCollisionPair2.typeA;
			uint typeB = ucpCollisionPair2.typeB;
			if (typeA <= m_collisionTypeCount && typeB <= m_collisionTypeCount)
			{
				if (m_globalCollisionDelegates[typeA, typeB, 0] != null)
				{
					m_globalCollisionDelegates[typeA, typeB, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
				}
				if (typeA != typeB && m_globalCollisionDelegates[typeB, typeA, 0] != null)
				{
					m_globalCollisionDelegates[typeB, typeA, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
				}
				if (chipmunkBodyC.m_collisionDelegates[typeB, 0] != null)
				{
					chipmunkBodyC.m_collisionDelegates[typeB, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
				}
				if (chipmunkBodyC2.m_collisionDelegates[typeA, 0] != null)
				{
					chipmunkBodyC2.m_collisionDelegates[typeA, 0](ReverseCollisionPair(ucpCollisionPair2), ucpCollisionPhase.Begin);
				}
			}
		}
		for (int j = 0; j < num2; j++)
		{
			ucpCollisionPair ucpCollisionPair3 = array2[j];
			ChipmunkBodyC chipmunkBodyC3 = m_bodies.m_array[ucpCollisionPair3.ucpComponentIndexA];
			ChipmunkBodyC chipmunkBodyC4 = m_bodies.m_array[ucpCollisionPair3.ucpComponentIndexB];
			uint typeA2 = ucpCollisionPair3.typeA;
			uint typeB2 = ucpCollisionPair3.typeB;
			if (typeA2 <= m_collisionTypeCount && typeB2 <= m_collisionTypeCount)
			{
				if (m_globalCollisionDelegates[typeA2, typeB2, 1] != null)
				{
					m_globalCollisionDelegates[typeA2, typeB2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
				}
				if (typeA2 != typeB2 && m_globalCollisionDelegates[typeB2, typeA2, 1] != null)
				{
					m_globalCollisionDelegates[typeB2, typeA2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
				}
				if (chipmunkBodyC3.m_collisionDelegates[typeB2, 1] != null)
				{
					chipmunkBodyC3.m_collisionDelegates[typeB2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
				}
				if (chipmunkBodyC4.m_collisionDelegates[typeA2, 1] != null)
				{
					chipmunkBodyC4.m_collisionDelegates[typeA2, 1](ReverseCollisionPair(ucpCollisionPair3), ucpCollisionPhase.Persist);
				}
			}
		}
		for (int k = 0; k < num3; k++)
		{
			ucpCollisionPair ucpCollisionPair4 = array3[k];
			ChipmunkBodyC chipmunkBodyC5 = m_bodies.m_array[ucpCollisionPair4.ucpComponentIndexA];
			ChipmunkBodyC chipmunkBodyC6 = m_bodies.m_array[ucpCollisionPair4.ucpComponentIndexB];
			uint typeA3 = ucpCollisionPair4.typeA;
			uint typeB3 = ucpCollisionPair4.typeB;
			if (typeA3 <= m_collisionTypeCount && typeB3 <= m_collisionTypeCount)
			{
				if (m_globalCollisionDelegates[typeA3, typeB3, 2] != null)
				{
					m_globalCollisionDelegates[typeA3, typeB3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
				}
				if (typeA3 != typeB3 && m_globalCollisionDelegates[typeB3, typeA3, 2] != null)
				{
					m_globalCollisionDelegates[typeB3, typeA3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
				}
				if (chipmunkBodyC5.m_collisionDelegates[typeB3, 2] != null)
				{
					chipmunkBodyC5.m_collisionDelegates[typeB3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
				}
				if (chipmunkBodyC6.m_collisionDelegates[typeA3, 2] != null)
				{
					chipmunkBodyC6.m_collisionDelegates[typeA3, 2](ReverseCollisionPair(ucpCollisionPair4), ucpCollisionPhase.Separate);
				}
			}
		}
		return num + num3 + num2;
	}

	public static void Update(float _dt)
	{
		ChipmunkProWrapper.ucpClearCollisionLists();
		ChipmunkProWrapper.ucpSpaceStep(_dt);
		HandleCollisionEvents();
		ChipmunkProWrapper.ucpClearCollisionLists();
		int aliveCount = m_bodies.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			ChipmunkBodyC chipmunkBodyC = m_bodies.m_array[m_bodies.m_aliveIndices[i]];
			if (!chipmunkBodyC.m_active)
			{
				continue;
			}
			if (ChipmunkProWrapper.ucpBodyIsRogue(chipmunkBodyC.body))
			{
				bool flag = ChipmunkProWrapper.ucpBodyIsStatic(chipmunkBodyC.body);
				bool flag2 = false;
				Vector3 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
				float num = ChipmunkProWrapper.ucpBodyGetAngle(chipmunkBodyC.body);
				float num2 = ChipmunkProWrapper.ucpBodyGetScale(chipmunkBodyC.body);
				Vector2 vector2 = chipmunkBodyC.TC.transform.position - vector;
				float num3 = Mathf.DeltaAngle(chipmunkBodyC.TC.transform.rotation.eulerAngles.z, num * 57.29578f);
				float num4 = chipmunkBodyC.TC.transform.lossyScale.z - num2;
				if (num4 != 0f)
				{
					ChipmunkProWrapper.ucpBodySetScale(chipmunkBodyC.body, chipmunkBodyC.TC.transform.lossyScale.z);
					flag2 = true;
				}
				if (Mathf.Abs(num3) > 0.0001f)
				{
					if (!flag)
					{
						ChipmunkProWrapper.ucpBodySetAngVel(chipmunkBodyC.body, num3 * ((float)Math.PI / 180f) / _dt);
					}
					flag2 = true;
				}
				if (vector2 != Vector2.zero)
				{
					if (!flag)
					{
						ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, vector2 / _dt);
					}
					flag2 = true;
				}
				if (flag)
				{
					if (flag2)
					{
						Debug.LogWarning("Moving static body. This is expensive!");
						ChipmunkProWrapper.ucpSpaceReindexShapesForBody(chipmunkBodyC.body);
						TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num * 57.29578f);
					}
				}
				else if (flag2)
				{
					ChipmunkProWrapper.ucpBodyUpdatePosition(chipmunkBodyC.body, _dt);
					TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num * 57.29578f);
				}
				else
				{
					ChipmunkProWrapper.ucpBodyResetForces(chipmunkBodyC.body);
					ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, Vector2.zero);
					ChipmunkProWrapper.ucpBodySetAngVel(chipmunkBodyC.body, 0f);
				}
			}
			else
			{
				Vector3 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
				vector.z = chipmunkBodyC.TC.transform.position.z;
				float num = ChipmunkProWrapper.ucpBodyGetAngle(chipmunkBodyC.body) * 57.29578f;
				float num2 = ChipmunkProWrapper.ucpBodyGetScale(chipmunkBodyC.body);
				float num5 = chipmunkBodyC.TC.transform.lossyScale.z - num2;
				if (num5 != 0f)
				{
					ChipmunkProWrapper.ucpBodySetScale(chipmunkBodyC.body, chipmunkBodyC.TC.transform.lossyScale.z);
					ChipmunkProWrapper.ucpSpaceReindexShapesForBody(chipmunkBodyC.body);
				}
				TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num);
			}
		}
		m_bodies.Update();
		m_constraints.Update();
	}

	public static ucpPolyShape GeneratePolyShapeFromGameObject(GameObject _gameObject, float _mass, float _elasticity, float _friction, ucpCollisionType _collisionType, bool _zyOrientation = false)
	{
		Component component = _gameObject.GetComponent(typeof(MeshFilter));
		Mesh sharedMesh = (component as MeshFilter).sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Vector2[] array = new Vector2[vertices.Length];
		int num = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			if (!_zyOrientation)
			{
				array[i].x = vertices[i].x;
			}
			else
			{
				array[i].x = vertices[i].z;
			}
			array[i].y = vertices[i].y;
			if (Mathf.Abs(array[i].x) < 0.1f)
			{
				num++;
			}
		}
		array = RemoveDuplicateVerts(array);
		if (num == vertices.Length)
		{
			Debug.LogError("Invalid vertex data. Wrong local orientation!");
			return null;
		}
		array = SortVerticeArray(array);
		return new ucpPolyShape(array, Vector2.zero, _mass, _elasticity, _friction, _collisionType);
	}

	public static ucpPolyShape[] GeneratePolyShapesFromChildren(GameObject _parentGameObject, float _mass, float _elasticity, float _friction, ucpCollisionType _collisionType, bool _zyOrientation = false)
	{
		Component[] componentsInChildren = _parentGameObject.GetComponentsInChildren(typeof(MeshFilter), true);
		ucpPolyShape[] array = new ucpPolyShape[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Mesh sharedMesh = (componentsInChildren[i] as MeshFilter).sharedMesh;
			Vector3[] vertices = sharedMesh.vertices;
			Vector2[] array2 = new Vector2[vertices.Length];
			int num = 0;
			for (int j = 0; j < vertices.Length; j++)
			{
				if (!_zyOrientation)
				{
					array2[j].x = vertices[j].x;
				}
				else
				{
					array2[j].x = vertices[j].z;
				}
				array2[j].y = vertices[j].y;
				if (Mathf.Abs(array2[j].x) < 0.1f)
				{
					num++;
				}
			}
			array2 = RemoveDuplicateVerts(array2);
			if (num == vertices.Length)
			{
				Debug.LogError("Invalid vertex data. Wrong local orientation!");
				continue;
			}
			array2 = SortVerticeArray(array2);
			Debug.Log(array2.Length);
			array[i] = new ucpPolyShape(array2, Vector2.zero, _mass, _elasticity, _friction, _collisionType);
		}
		return array;
	}

	public static Vector2[] RemoveDuplicateVerts(Vector2[] _verts)
	{
		List<Vector2> list = new List<Vector2>();
		foreach (Vector2 item in _verts)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

    public static Vector2[] SortVerticeArray(Vector2[] _verts)
    {
        Vector2 center = Vector2.zero;
        for (int i = 0; i < _verts.Length; i++)
        {
            center += _verts[i];
        }
        center /= (float)_verts.Length;

        // Replaces Array.Sort with exact Mono 2.6 QuickSort behavior. Silly Unity 2018 changed the sorting algorithm and broke the vertex order for some meshes. This is a workaround to restore the old behavior.
        ToolBox.LegacyArraySort(_verts, (Vector2 v1, Vector2 v2) => (!vertSortIsLess(v1, v2, center)) ? 1 : (-1));

        return _verts;
    }

    private static bool vertSortIsLess(Vector2 a, Vector2 b, Vector2 center)
	{
		int num = (int)((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
		if (num < 0)
		{
			return true;
		}
		if (num > 0)
		{
			return false;
		}
		int num2 = (int)((a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y));
		int num3 = (int)((b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y));
		return num2 > num3;
	}
}
