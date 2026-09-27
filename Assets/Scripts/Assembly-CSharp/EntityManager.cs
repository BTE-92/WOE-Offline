using System;
using System.Collections.Generic;
using UnityEngine;

public static class EntityManager
{
	public static DynamicArray<Entity> m_entities;

	public static List<Entity> m_updateList;

	private static List<Entity> m_removeList;

	private static DynamicArray<Tag> m_tags;

	private static List<DestroyLink> m_destroyLinks;

	public static void Initialize()
	{
		m_entities = new DynamicArray<Entity>();
		m_updateList = new List<Entity>();
		m_removeList = new List<Entity>();
		m_tags = new DynamicArray<Tag>();
		m_destroyLinks = new List<DestroyLink>();
	}

	public static Entity AddEntity()
	{
		string[] tags = new string[0];
		return AddEntity(tags);
	}

	public static Entity AddEntity(string _tag)
	{
		if (_tag != string.Empty)
		{
			string[] tags = new string[1] { _tag };
			return AddEntity(tags);
		}
		return AddEntity();
	}

	public static Entity AddEntity(string[] _tags)
	{
		Entity entity = m_entities.AddItem();
		if (_tags != null)
		{
			for (int i = 0; i < _tags.Length; i++)
			{
				AddTagForEntity(entity, _tags[i]);
			}
		}
		entity.m_active = true;
		return entity;
	}

	public static Entity AddTimedFXEntity(GameObject _resource, Vector3 _pos, Vector3 _rotation, float _destroyAfterSecs, string _tag)
	{
		Entity entity = AddEntity(_tag);
		TimerS.AddComponent(entity, "Timer", _destroyAfterSecs, 0f, true);
		TransformC parentTC = TransformS.AddComponent(entity, string.Empty, _pos, _rotation);
		PrefabS.AddComponent(parentTC, Vector3.zero, _resource);
		return entity;
	}

	public static TransformC AddEntityWithTC()
	{
		string[] tags = new string[0];
		return AddEntityWithTC(tags);
	}

	public static TransformC AddEntityWithTC(string _tag)
	{
		string[] tags = new string[1] { _tag };
		return AddEntityWithTC(tags);
	}

	public static TransformC AddEntityWithTC(string[] _tags)
	{
		Entity entity = AddEntity(_tags);
		return TransformS.AddComponent(entity);
	}

	public static void RemoveEntity(Entity _e)
	{
		RemoveEntity(_e, true);
	}

	public static void AddLogicDelegate(Entity _e, EntityLogicDelegate _logicDelegate)
	{
		if (_e.m_entityLogicCount == 0)
		{
			_e.d_entityLogic = _logicDelegate;
		}
		else
		{
			_e.d_entityLogic = (EntityLogicDelegate)Delegate.Combine(_e.d_entityLogic, _logicDelegate);
		}
		if (!m_updateList.Contains(_e))
		{
			m_updateList.Add(_e);
		}
		_e.m_entityLogicCount++;
	}

	public static void RemoveLogicDelegate(Entity _e, EntityLogicDelegate _logicDelegate)
	{
		if (_e.d_entityLogic != null)
		{
			Delegate[] invocationList = _e.d_entityLogic.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				if ((EntityLogicDelegate)obj == _logicDelegate)
				{
					_e.d_entityLogic = (EntityLogicDelegate)Delegate.Remove(_e.d_entityLogic, (EntityLogicDelegate)obj);
					_e.m_entityLogicCount--;
					break;
				}
			}
		}
		if (_e.m_entityLogicCount == 0)
		{
			_e.d_entityLogic = null;
			m_updateList.Remove(_e);
		}
	}

	public static void RemoveAllLogicDelegates(Entity _e)
	{
		if (_e.d_entityLogic != null)
		{
			Delegate[] invocationList = _e.d_entityLogic.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				_e.d_entityLogic = (EntityLogicDelegate)Delegate.Remove(_e.d_entityLogic, (EntityLogicDelegate)obj);
			}
		}
		_e.m_entityLogicCount = 0;
		m_updateList.Remove(_e);
	}

	public static void RemoveEntitiesByTransformComponentHierarchy(TransformC _tc, bool _removeParents)
	{
		RemoveEntitiesByTransformComponentHierarchy(_tc, _removeParents, false);
	}

	public static void RemoveEntitiesByTransformComponentHierarchy(TransformC _tc, bool _removeParents, bool _removeImmediately)
	{
		if (_tc != null)
		{
			if (_removeParents)
			{
				_tc = TransformS.GetRootTransformComponent(_tc);
			}
			while (_tc.childs.Count > 0)
			{
				int index = _tc.childs.Count - 1;
				RemoveEntitiesByTransformComponentHierarchy(_tc.childs[index], false, _removeImmediately);
				_tc.childs.RemoveAt(index);
			}
			RemoveEntity(_tc.p_entity, true, _removeImmediately);
		}
	}

	public static void RemoveEntityByTransformComponent(TransformC _tc, bool _removeChildren)
	{
		if (_removeChildren)
		{
			while (_tc.childs.Count > 0)
			{
				RemoveEntitiesByTransformComponentHierarchy(_tc.childs[0], false);
				_tc.childs.Remove(_tc.childs[0]);
			}
		}
		RemoveEntity(_tc.p_entity);
	}

	public static void RemoveEntity(Entity _e, bool _removeFromList)
	{
		RemoveEntity(_e, _removeFromList, false);
	}

	public static void RemoveEntity(Entity _e, bool _removeFromList, bool _removeImmediately)
	{
		_e.m_active = false;
		for (int num = m_destroyLinks.Count - 1; num > -1; num--)
		{
			DestroyLink destroyLink = m_destroyLinks[num];
			if (destroyLink.primary == _e)
			{
				m_destroyLinks.RemoveAt(num);
				RemoveEntity(destroyLink.secondary, _removeFromList, _removeFromList);
			}
			else if (destroyLink.bothWays && destroyLink.secondary == _e)
			{
				m_destroyLinks.RemoveAt(num);
				RemoveEntity(destroyLink.primary, _removeFromList, _removeFromList);
			}
			else if (destroyLink.secondary == _e)
			{
				m_destroyLinks.RemoveAt(num);
			}
		}
		if (!_removeImmediately)
		{
			for (int i = 0; i < _e.m_components.Count; i++)
			{
				_e.m_components[i].m_active = false;
			}
			m_removeList.Add(_e);
			return;
		}
		RemoveAllTagsFromEntity(_e);
		RemoveAllLogicDelegates(_e);
		for (int num2 = _e.m_components.Count - 1; num2 > -1; num2--)
		{
			IComponent component = _e.m_components[num2];
			switch (component.m_componentType)
			{
			case ComponentType.CameraBorder:
				CameraS.RemoveBorderComponent(component as CameraBorderC);
				break;
			case ComponentType.CameraEffect:
				CameraS.RemoveEffectComponent(component as CameraEffectC);
				break;
			case ComponentType.CameraTarget:
				CameraS.RemoveTargetComponent(component as CameraTargetC);
				break;
			case ComponentType.ChipmunkBody:
				ChipmunkProS.RemoveBody(component as ChipmunkBodyC);
				break;
			case ComponentType.ChipmunkConstraint:
				ChipmunkProS.RemoveConstraint(component as ChipmunkConstraintC);
				break;
			case ComponentType.Event:
				EventS.RemoveComponent(component as EventC);
				break;
			case ComponentType.Gpc:
				GpcS.RemoveComponent(component as GpcC);
				break;
			case ComponentType.Prefab:
				PrefabS.RemoveComponent(component as PrefabC);
				break;
			case ComponentType.Sound:
				SoundS.RemoveComponent(component as SoundC);
				break;
			case ComponentType.Sprite:
				SpriteS.RemoveComponent(component as SpriteC);
				break;
			case ComponentType.Text:
				TextS.RemoveComponent(component as TextC);
				break;
			case ComponentType.TextMesh:
				TextMeshS.RemoveComponent(component as TextMeshC);
				break;
			case ComponentType.Timer:
				TimerS.RemoveComponent(component as TimerC);
				break;
			case ComponentType.TouchArea:
				TouchAreaS.RemoveArea(component as TouchAreaC);
				break;
			case ComponentType.Transform:
				TransformS.RemoveComponent(component as TransformC);
				break;
			case ComponentType.Tween:
				TweenS.RemoveComponent(component as TweenC);
				break;
			case ComponentType.Projector:
				ProjectorS.RemoveComponent(component as ProjectorC);
				break;
			default:
				Main.m_currentGame.RemoveComponent(component);
				break;
			}
		}
		if (_removeFromList)
		{
			_e.m_persistent = false;
			m_entities.RemoveItem(_e);
		}
	}

	public static void AddDestroyLink(Entity _primary, Entity _seconday, bool _bothWays)
	{
		DestroyLink item = new DestroyLink
		{
			primary = _primary,
			secondary = _seconday,
			bothWays = _bothWays
		};
		m_destroyLinks.Add(item);
	}

	public static void RemoveDestroyLinks(Entity _entity)
	{
		for (int num = m_destroyLinks.Count - 1; num > -1; num--)
		{
			DestroyLink destroyLink = m_destroyLinks[num];
			if (destroyLink.primary == _entity || destroyLink.secondary == _entity)
			{
				m_destroyLinks.RemoveAt(num);
			}
		}
	}

	public static void RemoveDestroyLink(Entity _primary, Entity _secondary)
	{
		for (int num = m_destroyLinks.Count - 1; num > -1; num--)
		{
			DestroyLink destroyLink = m_destroyLinks[num];
			if (destroyLink.primary == _primary && destroyLink.secondary == _secondary)
			{
				m_destroyLinks.RemoveAt(num);
				break;
			}
		}
	}

	public static void RemoveAllEntities()
	{
		List<Entity> list = new List<Entity>();
		for (int i = 0; i < m_entities.m_aliveCount; i++)
		{
			Entity entity = m_entities.m_array[m_entities.m_aliveIndices[i]];
			if (!entity.m_persistent)
			{
				RemoveEntity(entity, false, true);
				list.Add(entity);
			}
		}
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			m_entities.RemoveItem(list[index]);
			list.RemoveAt(index);
		}
	}

	public static void RemoveEntitiesByTag(string _tag)
	{
		RemoveEntitiesByTag(_tag, false);
	}

	public static void RemoveEntitiesByTag(string _tag, bool _removeImmediately)
	{
		List<Entity> list = new List<Entity>();
		for (int num = m_tags.m_aliveCount - 1; num > -1; num--)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[num]];
			if (tag.m_tag.Equals(_tag))
			{
				list.Add(tag.p_entity);
			}
		}
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			RemoveEntity(list[index], true, _removeImmediately);
			list.RemoveAt(index);
		}
	}

	public static Entity GetEntityByIndex(int _entityIndex)
	{
		return m_entities.m_array[_entityIndex];
	}

	public static List<Entity> GetEntitiesByTag(string _tag)
	{
		List<Entity> list = new List<Entity>();
		for (int i = 0; i < m_tags.m_aliveCount; i++)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[i]];
			if (tag.m_tag == _tag)
			{
				list.Add(tag.p_entity);
			}
		}
		return list;
	}

	public static List<IComponent> GetComponentsByEntity(ComponentType _componentType, Entity _e)
	{
		List<IComponent> list = new List<IComponent>();
		if (_e == null)
		{
			Debug.LogError("Trying to get components from null entity");
			return list;
		}
		for (int i = 0; i < _e.m_components.Count; i++)
		{
			if (_e.m_components[i].m_componentType == _componentType)
			{
				list.Add(_e.m_components[i]);
			}
		}
		return list;
	}

	public static List<IComponent> GetComponentsByType(ComponentType _componentType)
	{
		List<IComponent> list = new List<IComponent>();
		for (int i = 0; i < m_entities.m_aliveCount; i++)
		{
			Entity entity = m_entities.m_array[m_entities.m_aliveIndices[i]];
			for (int j = 0; j < entity.m_components.Count; j++)
			{
				if (entity.m_components[j].m_componentType == _componentType)
				{
					list.Add(entity.m_components[j]);
				}
			}
		}
		return list;
	}

	public static IComponent GetComponentByIdentifier(Entity _e, int _identifier)
	{
		for (int i = 0; i < _e.m_components.Count; i++)
		{
			IComponent component = _e.m_components[i];
			if (component.m_identifier == _identifier)
			{
				return component;
			}
		}
		return null;
	}

	public static void AddTagForEntity(Entity _entity, string _tag)
	{
		Tag tag = m_tags.AddItem();
		tag.m_tag = _tag;
		tag.p_entity = _entity;
	}

	public static void RemoveTagFromEntity(Entity _entity, string _tag)
	{
		for (int i = 0; i < m_tags.m_aliveCount; i++)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[i]];
			if (tag.p_entity == _entity && tag.m_tag == _tag)
			{
				m_tags.RemoveItem(tag);
				break;
			}
		}
	}

	public static void RemoveAllTagsFromEntity(Entity _entity)
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < m_tags.m_aliveCount; i++)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[i]];
			if (tag.p_entity == _entity)
			{
				list.Add(tag);
			}
		}
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			m_tags.RemoveItem(list[index]);
			list.RemoveAt(index);
		}
	}

	public static void SetVisibilityOfEntity(Entity _e, bool _visible)
	{
		for (int i = 0; i < _e.m_components.Count; i++)
		{
			if (_e.m_components[i].m_componentType == ComponentType.Sprite)
			{
				SpriteC spriteC = _e.m_components[i] as SpriteC;
				if (spriteC.wasVisible)
				{
					SpriteS.SetVisibility(spriteC, _visible, false);
				}
			}
			else if (_e.m_components[i].m_componentType == ComponentType.Prefab)
			{
				PrefabC prefabC = _e.m_components[i] as PrefabC;
				if (prefabC.m_wasVisible)
				{
					PrefabS.SetVisibility(prefabC, _visible, false);
				}
			}
			else if (_e.m_components[i].m_componentType == ComponentType.TextMesh)
			{
				TextMeshC textMeshC = _e.m_components[i] as TextMeshC;
				if (textMeshC.m_wasVisible)
				{
					TextMeshS.SetVisibility(textMeshC, _visible, false);
				}
			}
		}
	}

	public static void SetActivityOfEntity(Entity _e, bool _active, bool _affectSpriteAndPrefabVisibility, bool _affectTranformChildren = false, bool _setWasActive = true)
	{
		for (int i = 0; i < _e.m_components.Count; i++)
		{
			IComponent component = _e.m_components[i];
			if (!_setWasActive && !component.m_wasActive)
			{
				continue;
			}
			component.m_active = _active;
			if (_affectSpriteAndPrefabVisibility)
			{
				if (component.m_componentType == ComponentType.Transform && _affectTranformChildren)
				{
					TransformC transformC = component as TransformC;
					for (int j = 0; j < transformC.childs.Count; j++)
					{
						if (transformC.childs[j].p_entity != _e)
						{
							SetActivityOfEntity(transformC.childs[j].p_entity, _active, _affectSpriteAndPrefabVisibility, _affectTranformChildren);
						}
					}
				}
				if (component.m_componentType == ComponentType.Sprite)
				{
					SpriteC spriteC = component as SpriteC;
					if (spriteC.wasVisible)
					{
						SpriteS.SetVisibility(spriteC, _active, false);
					}
				}
				else if (component.m_componentType == ComponentType.Prefab)
				{
					PrefabC prefabC = component as PrefabC;
					if (prefabC.m_wasVisible)
					{
						PrefabS.SetVisibility(prefabC, _active, false);
					}
				}
				else if (component.m_componentType == ComponentType.TextMesh)
				{
					TextMeshC textMeshC = component as TextMeshC;
					if (textMeshC.m_wasVisible)
					{
						TextMeshS.SetVisibility(textMeshC, _active, false);
					}
				}
			}
			if (component.m_componentType == ComponentType.Prefab)
			{
				PrefabS.PauseParticleSystems(component as PrefabC, !_active);
			}
			if (component.m_componentType == ComponentType.Sound)
			{
				SoundC soundC = component as SoundC;
				if (soundC.isPlaying)
				{
					if (_active)
					{
						SoundS.ResumeSound(soundC);
					}
					else
					{
						SoundS.PauseSound(soundC);
					}
				}
			}
			if (_setWasActive)
			{
				component.m_wasActive = _active;
			}
		}
	}

	public static void SetActivityOfAllEntities(bool _active, bool _affectSpriteAndPrefabVisibility)
	{
		int aliveCount = m_entities.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			Entity e = m_entities.m_array[m_entities.m_aliveIndices[i]];
			SetActivityOfEntity(e, _active, _affectSpriteAndPrefabVisibility, false, false);
		}
	}

	public static void SetActivityOfEntitiesWithTag(string _tag, bool _active, bool _affectSpriteAndPrefabVisibility, bool _affectTranformChildren = false)
	{
		for (int i = 0; i < m_tags.m_aliveCount; i++)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[i]];
			if (tag.m_tag == _tag)
			{
				SetActivityOfEntity(tag.p_entity, _active, _affectSpriteAndPrefabVisibility, _affectTranformChildren, false);
			}
		}
	}

	public static void SetActivityOfEntitiesWithTag(string[] _tags, bool _active, bool _affectSpriteAndPrefabVisibility, bool _affectTranformChildren = false)
	{
		for (int i = 0; i < m_tags.m_aliveCount; i++)
		{
			Tag tag = m_tags.m_array[m_tags.m_aliveIndices[i]];
			for (int j = 0; j < _tags.Length; j++)
			{
				if (tag.m_tag == _tags[j])
				{
					SetActivityOfEntity(tag.p_entity, _active, _affectSpriteAndPrefabVisibility, _affectTranformChildren, false);
				}
			}
		}
	}

	public static void AddComponentToEntity(Entity _e, IComponent _c)
	{
		if (_e == null)
		{
			_c.m_active = true;
			_c.m_wasActive = true;
			return;
		}
		_c.m_active = true;
		_c.m_wasActive = true;
		_c.p_entity = _e;
		_e.m_components.Add(_c);
	}

	public static void RemoveComponentFromEntity(IComponent _c)
	{
		if (_c.p_entity != null)
		{
			_c.p_entity.m_components.Remove(_c);
		}
		_c.m_active = false;
		_c.m_wasActive = true;
		_c.p_entity = null;
	}

	public static void RemoveAllComponentsByType(Entity _e, ComponentType _type)
	{
		List<IComponent> componentsByEntity = GetComponentsByEntity(_type, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponentFromEntity(componentsByEntity[index]);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void Update()
	{
		while (m_removeList.Count > 0)
		{
			int index = m_removeList.Count - 1;
			RemoveEntity(m_removeList[index], true, true);
			m_removeList.RemoveAt(index);
		}
		m_entities.Update();
		m_tags.Update();
	}

	public static void UpdateLogic()
	{
		for (int i = 0; i < m_updateList.Count; i++)
		{
			if (m_updateList[i].m_active)
			{
				m_updateList[i].d_entityLogic();
			}
		}
	}
}
