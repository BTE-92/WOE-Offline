using System.Collections.Generic;
using UnityEngine;

public class TransformC : BasicComponent
{
	public static int m_componentCount;

	public bool updatePosition;

	public bool updateRotation;

	public bool updateScale;

	public bool updatedPosition;

	public bool updatedRotation;

	public bool updatedScale;

	public TransformC parent;

	public List<TransformC> childs;

	public int level;

	public bool parentedToPhysics;

	public Vector3 lastPos;

	public Vector3 delta;

	public bool forceRotation;

	public Quaternion forcedRotation = Quaternion.identity;

	public bool forceScale;

	public Vector3 forcedScale = Vector3.one;

	public Transform transform;

	public TransformC()
		: base(ComponentType.Transform)
	{
		transform = (Object.Instantiate(TransformS.m_transformHelper) as GameObject).transform;
		childs = new List<TransformC>();
		Reset();
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		transform.name = "TransformComponent";
		forceRotation = false;
		forceScale = false;
		parent = null;
		m_active = false;
		updatedPosition = false;
		updatedRotation = false;
		updatedScale = false;
		updatePosition = true;
		updateRotation = true;
		updateScale = true;
		transform.localScale = Vector3.one;
		transform.position = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		forcedRotation = Quaternion.identity;
		forcedScale = Vector3.one;
		level = 0;
		parentedToPhysics = false;
		delta = Vector3.zero;
		lastPos = Vector3.zero;
	}

	public override void Destroy()
	{
		Object.Destroy(transform.gameObject);
	}

	~TransformC()
	{
		m_componentCount--;
	}
}
