using System.Collections.Generic;
using UnityEngine;

public static class PsS
{
	public static DynamicArray<UnitC> m_units;

	public static DynamicArray<ItemC> m_items;

	public static DynamicArray<GroundC> m_grounds;

	public static void Initialize()
	{
		m_units = new DynamicArray<UnitC>();
		m_items = new DynamicArray<ItemC>();
		m_grounds = new DynamicArray<GroundC>();
	}

	public static UnitC AddUnit(Entity _entity, Unit _unitClass)
	{
		UnitC unitC = m_units.AddItem();
		unitC.m_unit = _unitClass;
		EntityManager.AddComponentToEntity(_entity, unitC);
		return unitC;
	}

	public static void RemoveUnit(UnitC _c)
	{
		_c.m_unit.RemoveAllContacts();
		EntityManager.RemoveComponentFromEntity(_c);
		m_units.RemoveItem(_c);
	}

	public static ItemC AddItem(Entity _entity, Item _itemClass)
	{
		ItemC itemC = m_items.AddItem();
		itemC.m_item = _itemClass;
		EntityManager.AddComponentToEntity(_entity, itemC);
		return itemC;
	}

	public static void RemoveItem(ItemC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_items.RemoveItem(_c);
	}

	public static GroundC AddGround(Entity _entity, Ground _groundClass)
	{
		GroundC groundC = m_grounds.AddItem();
		groundC.m_ground = _groundClass;
		EntityManager.AddComponentToEntity(_entity, groundC);
		return groundC;
	}

	public static void RemoveGround(GroundC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_grounds.RemoveItem(_c);
	}

	public static void Update()
	{
		int aliveCount = m_units.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			UnitC unitC = m_units.m_array[m_units.m_aliveIndices[i]];
			if (unitC.m_active)
			{
				unitC.m_unit.Update();
			}
		}
		m_units.Update();
		aliveCount = m_items.m_aliveCount;
		for (int j = 0; j < aliveCount; j++)
		{
			ItemC itemC = m_items.m_array[m_items.m_aliveIndices[j]];
			if (itemC.m_active)
			{
				itemC.m_item.Update();
			}
		}
		m_units.Update();
		aliveCount = m_grounds.m_aliveCount;
		for (int k = 0; k < aliveCount; k++)
		{
			GroundC groundC = m_grounds.m_array[m_grounds.m_aliveIndices[k]];
			if (groundC.m_active)
			{
				groundC.m_ground.Update();
			}
		}
		m_units.Update();
	}

	public static void ApplyBlastWave(Vector2 _blastCenter, float _blastForce, float _blastRadius, float _destroyGroundRadius = 0f, float _damage = 0f)
	{
		int aliveCount = m_units.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			UnitC unitC = m_units.m_array[m_units.m_aliveIndices[i]];
			if (!unitC.m_active)
			{
				continue;
			}
			Unit unit = unitC.m_unit;
			Vector2 zero = Vector2.zero;
			int num = 0;
			for (int j = 0; j < unit.m_assembledEntities.Count; j++)
			{
				Entity e = unit.m_assembledEntities[j];
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, e);
				for (int k = 0; k < componentsByEntity.Count; k++)
				{
					ChipmunkBodyC chipmunkBodyC = componentsByEntity[k] as ChipmunkBodyC;
					Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
					Vector2 vector2 = -(_blastCenter - vector).normalized;
					float magnitude = (_blastCenter - vector).magnitude;
					float num2 = 1f - Mathf.Min(magnitude / _blastRadius, 1f);
					zero += vector;
					num++;
					ChipmunkProWrapper.ucpBodyApplyImpulse(chipmunkBodyC.body, vector2 * _blastForce * num2, Vector2.zero);
				}
			}
			if (_damage > 0f && num > 0)
			{
				zero /= (float)num;
				float magnitude2 = (_blastCenter - zero).magnitude;
				float num3 = 1f - Mathf.Min(magnitude2 / _blastRadius, 1f);
				unit.Damage(new Damage(DamageType.Weapon, _damage * num3));
			}
		}
		if (!(_destroyGroundRadius > 0f))
		{
			return;
		}
		AutoGeometryBrush brush = new AutoGeometryBrush(Mathf.Max(1.5f, _destroyGroundRadius / 16f), false, 1f, 0.66f);
		foreach (AutoGeometryLayer layer in AutoGeometryManager.m_layers)
		{
			if (layer.m_groundC.m_ground.m_destructible)
			{
				layer.PaintWithBrush(brush, _blastCenter, AGDrawMode.SUB, false, ref layer.m_bytes);
				layer.UpdateSegments();
			}
		}
	}
}
