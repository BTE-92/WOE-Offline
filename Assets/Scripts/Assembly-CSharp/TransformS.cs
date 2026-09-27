using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformS
{
	public static DynamicArray<TransformC> m_components;

	public static GameObject m_transformHelper = new GameObject("TransformComponent");

	private static TransformC c;

	private static TransformC p;

	public static void Initialize()
	{
		m_components = new DynamicArray<TransformC>();
	}

	public static TransformC AddComponent(Entity _entity)
	{
		return AddComponent(_entity, string.Empty, Vector3.zero, Vector3.zero);
	}

	public static TransformC AddComponent(Entity _entity, string _name)
	{
		return AddComponent(_entity, _name, Vector3.zero, Vector3.zero);
	}

	public static TransformC AddComponent(Entity _entity, Vector3 _pos)
	{
		return AddComponent(_entity, string.Empty, _pos, Vector3.zero);
	}

	public static TransformC AddComponent(Entity _entity, string _name, Vector3 _pos)
	{
		return AddComponent(_entity, _name, _pos, Vector3.zero);
	}

	public static TransformC AddComponent(Entity _entity, string _name, Vector3 _pos, Vector3 _rot)
	{
		TransformC transformC = m_components.AddItem();
		if (_name != string.Empty)
		{
			transformC.transform.name = _name;
		}
		if (_pos != Vector3.zero)
		{
			transformC.transform.position = _pos;
		}
		if (_rot != Vector3.zero)
		{
			transformC.transform.rotation = Quaternion.Euler(_rot);
		}
		EntityManager.AddComponentToEntity(_entity, transformC);
		return transformC;
	}

	public static void RemoveComponent(TransformC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		if (_c.parent != null)
		{
			_c.parent.childs.Remove(_c);
			_c.parent = null;
		}
		_c.transform.parent = null;
		while (_c.childs.Count > 0)
		{
			int index = _c.childs.Count - 1;
			_c.childs[index].parent = null;
			_c.childs[index].transform.parent = null;
			_c.childs[index].level = 0;
			_c.childs.RemoveAt(index);
		}
		_c.transform.name = "TransformComponent";
		_c.transform.gameObject.layer = 1;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	private static void UpdateChildHierarchyLevel(TransformC _parent)
	{
		for (int i = 0; i < _parent.childs.Count; i++)
		{
			_parent.childs[i].level = _parent.level + 1;
			UpdateChildHierarchyLevel(_parent.childs[i]);
		}
	}

	public static void ParentComponent(TransformC _c, TransformC _parent)
	{
		ParentComponent(_c, _parent, _c.transform.position - _parent.transform.position);
	}

	public static void ParentComponent(TransformC _c, TransformC _parent, Vector3 _childLocalPos)
	{
		if (_c.transform.parent != null)
		{
			UnparentComponent(_c);
		}
		_c.transform.parent = _parent.transform;
		_parent.childs.Add(_c);
		_c.parent = _parent;
		_c.updatePosition = true;
		_c.updateRotation = true;
		_c.updateScale = true;
		UpdateChildHierarchyLevel(_parent);
		SetPosition(_c, _childLocalPos);
		LoosenPhysicsConnections(_c);
	}

	public static void UnparentComponent(TransformC _c)
	{
		if (_c.parent != null)
		{
			_c.parent.childs.Remove(_c);
		}
		_c.parent = null;
		_c.transform.parent = null;
		_c.level = 0;
		_c.updatePosition = true;
		_c.updateRotation = true;
		_c.updateScale = true;
		UpdateChildHierarchyLevel(_c);
	}

	public static void LoosenPhysicsConnections(TransformC _c)
	{
		for (int i = 0; i < _c.childs.Count; i++)
		{
			LoosenPhysicsConnections(_c.childs[i]);
		}
		_c.lastPos = _c.transform.localPosition;
	}

	public static TransformC GetRootTransformComponent(TransformC _tc)
	{
		if (_tc.parent != null)
		{
			return GetRootTransformComponent(_tc.parent);
		}
		return _tc;
	}

	public static TransformC GetParentTransformComponent(TransformC _tc)
	{
		if (_tc.parent != null)
		{
			return _tc.parent;
		}
		return _tc;
	}

	public static void SetTransform(TransformC _c, Vector3 _position, Vector3 _rotation, Vector3 _scale)
	{
		SetPosition(_c, _position);
		SetRotation(_c, _rotation);
		SetScale(_c, _scale);
	}

	public static void SetTransform(TransformC _c, Vector3 _position, Vector3 _rotation)
	{
		SetPosition(_c, _position);
		SetRotation(_c, _rotation);
	}

	public static void SetGlobalTransform(TransformC _c, Vector3 _position, Vector3 _rotation)
	{
		SetGlobalPosition(_c, _position);
		SetGlobalRotation(_c, _rotation);
	}

	public static void SetTransform(TransformC _c, Vector3 _position, Vector3 _rotation, IntPtr _cpBody)
	{
		SetPosition(_c, _position);
		SetRotation(_c, _rotation);
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetPos(_cpBody, _position);
			ChipmunkProWrapper.ucpBodySetAngle(_cpBody, _rotation.z * ((float)Math.PI / 180f));
		}
	}

	public static void SetPosition(TransformC _c, Vector3 _position)
	{
		_c.transform.localPosition = _position;
		_c.updatePosition = true;
	}

	public static void SetGlobalPositionWithoutChildren(TransformC _c, Vector3 _position)
	{
		SetGlobalPositionWithoutChildren(_c, _position, IntPtr.Zero);
	}

	public static void SetGlobalPositionWithoutChildren(TransformC _c, Vector3 _position, IntPtr _cpBody)
	{
		for (int i = 0; i < _c.childs.Count; i++)
		{
			_c.childs[i].transform.parent = null;
		}
		_c.transform.position = _position;
		_c.updatePosition = true;
		for (int j = 0; j < _c.childs.Count; j++)
		{
			_c.childs[j].transform.parent = _c.transform;
		}
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetPos(_cpBody, _position);
		}
	}

	public static void SetGlobalPosition(TransformC _c, Vector3 _position)
	{
		_c.transform.position = _position;
		_c.updatePosition = true;
	}

	public static void SetGlobalPosition(TransformC _c, Vector3 _position, IntPtr _cpBody)
	{
		_c.transform.position = _position;
		_c.updatePosition = true;
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetPos(_cpBody, _position);
		}
	}

	public static void Move(TransformC _c, Vector3 _step)
	{
		_c.transform.localPosition += _step;
		_c.updatePosition = true;
	}

	public static void GlobalMove(TransformC _c, Vector3 _step)
	{
		_c.transform.position += _step;
		_c.updatePosition = true;
	}

	public static void SetRotation(TransformC _c, Vector3 _rotation)
	{
		if (_c.forceRotation)
		{
			_c.forcedRotation = Quaternion.Euler(_rotation);
		}
		else
		{
			_c.transform.localRotation = Quaternion.Euler(_rotation);
		}
		_c.updateRotation = true;
	}

	public static void SetRotation(TransformC _c, Vector3 _rotation, IntPtr _cpBody)
	{
		if (_c.forceRotation)
		{
			_c.forcedRotation = Quaternion.Euler(_rotation);
		}
		else
		{
			_c.transform.localRotation = Quaternion.Euler(_rotation);
		}
		_c.updateRotation = true;
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetAngle(_cpBody, _rotation.z * ((float)Math.PI / 180f));
		}
	}

	public static void SetGlobalRotation(TransformC _c, Vector3 _rotation)
	{
		_c.transform.rotation = Quaternion.Euler(_rotation);
		_c.updateRotation = true;
	}

	public static void SetGlobalRotation(TransformC _c, Vector3 _rotation, IntPtr _cpBody)
	{
		_c.transform.rotation = Quaternion.Euler(_rotation);
		_c.updateRotation = true;
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetAngle(_cpBody, _rotation.z * ((float)Math.PI / 180f));
		}
	}

	public static void SetGlobalRotationWithoutChildren(TransformC _c, Vector3 _rotation)
	{
		SetGlobalRotationWithoutChildren(_c, _rotation, IntPtr.Zero);
	}

	public static void SetGlobalRotationWithoutChildren(TransformC _c, Vector3 _rotation, IntPtr _cpBody)
	{
		List<Vector3> list = new List<Vector3>();
		List<Quaternion> list2 = new List<Quaternion>();
		for (int i = 0; i < _c.childs.Count; i++)
		{
			list.Add(_c.childs[i].transform.position);
			list2.Add(_c.childs[i].transform.rotation);
			_c.childs[i].transform.parent = null;
		}
		_c.transform.rotation = Quaternion.Euler(_rotation);
		_c.updateRotation = true;
		for (int j = 0; j < _c.childs.Count; j++)
		{
			_c.childs[j].transform.parent = _c.transform;
			_c.childs[j].transform.position = list[j];
			_c.childs[j].transform.rotation = list2[j];
		}
		if (_cpBody != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpBodySetAngle(_cpBody, _rotation.z * ((float)Math.PI / 180f));
		}
	}

	public static Vector3 Rotate(TransformC _c, Vector3 _rotation)
	{
		if (_c.forceRotation)
		{
			_c.forcedRotation.eulerAngles += _rotation;
		}
		else
		{
			_c.transform.Rotate(_rotation);
		}
		_c.updateRotation = true;
		if (_c.forceRotation)
		{
			return _c.forcedRotation.eulerAngles;
		}
		return _c.transform.rotation.eulerAngles;
	}

	public static void SetScale(TransformC _c, Vector3 _scale)
	{
		if (_c.forceScale)
		{
			_c.forcedScale = _scale;
		}
		else
		{
			_c.transform.localScale = _scale;
		}
		_c.updateScale = true;
	}

	public static void SetScale(TransformC _c, float _scale)
	{
		if (_c.forceScale)
		{
			_c.forcedScale = Vector3.one * _scale;
		}
		else
		{
			_c.transform.localScale = Vector3.one * _scale;
		}
		_c.updateScale = true;
	}

	public static void Scale(TransformC _c, float _scale)
	{
		if (_c.forceScale)
		{
			_c.forcedScale *= _scale;
		}
		else
		{
			_c.transform.localScale *= _scale;
		}
		_c.updateScale = true;
	}

	public static void Scale(TransformC _c, Vector3 _scale)
	{
		if (_c.forceScale)
		{
			_c.forcedScale.x *= _scale.x;
			_c.forcedScale.y *= _scale.y;
			_c.forcedScale.z *= _scale.z;
		}
		else
		{
			Vector3 localScale = _c.transform.localScale;
			localScale.x *= _scale.x;
			localScale.y *= _scale.y;
			localScale.z *= _scale.z;
			_c.transform.localScale = localScale;
		}
		_c.updateScale = true;
	}

	public static void Update()
	{
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			c = m_components.m_array[m_components.m_aliveIndices[i]];
			if (!c.m_active)
			{
				continue;
			}
			if (!c.updatePosition && c.updatedPosition)
			{
				c.updatedPosition = false;
			}
			if (!c.updateRotation && c.updatedRotation)
			{
				c.updatedRotation = false;
			}
			if (!c.updateScale && c.updatedScale)
			{
				c.updatedScale = false;
			}
			if (c.parent != null)
			{
				p = c.parent;
				if (p.updatedPosition)
				{
					c.updatePosition = true;
				}
				if (p.updatedRotation)
				{
					c.updatePosition = true;
					if (!c.forceRotation)
					{
						c.updateRotation = true;
					}
					else
					{
						c.transform.rotation = c.forcedRotation;
					}
				}
				if (p.updatedScale)
				{
					c.updatePosition = true;
					if (!c.forceScale)
					{
						c.updateScale = true;
					}
					else
					{
						c.transform.localScale = c.forcedScale;
					}
				}
				if (c.parentedToPhysics)
				{
					c.updatedPosition = true;
					c.delta = c.transform.localPosition - c.lastPos;
					SetPosition(c, c.delta);
					c.lastPos = c.transform.localPosition;
				}
			}
			if (c.updateRotation)
			{
				if (c.forceRotation)
				{
					c.transform.rotation = c.forcedRotation;
				}
				c.updateRotation = false;
				c.updatedRotation = true;
			}
			if (c.updateScale)
			{
				if (c.forceScale)
				{
					c.transform.localScale = c.forcedScale;
				}
				c.updateScale = false;
				c.updatedScale = true;
			}
			if (c.updatePosition)
			{
				c.updatePosition = false;
				c.updatedPosition = true;
			}
		}
		m_components.Update();
	}
}
