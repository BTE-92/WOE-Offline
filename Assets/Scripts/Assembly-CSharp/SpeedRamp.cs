using System;
using UnityEngine;

public class SpeedRamp : Unit
{
	private ChipmunkBodyC m_cmb;

	private Material m_laneMat;

	private int m_soundTriggerTimer;

	private IntPtr m_beltShape;

	public SpeedRamp(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/SpeedRampPrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		GameObject gameObject2 = gameObject.transform.Find("BoostRampBody").gameObject;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 10f) + GetZBufferBias(), gameObject2);
		m_laneMat = gameObject2.transform.Find("BoostRampConveyor").gameObject.GetComponent<Renderer>().sharedMaterial;
		ucpPolyShape ucpPolyShape2 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("CollisionBelt").gameObject, 1f, 0.25f, 1.2f, (ucpCollisionType)4);
		ucpPolyShape ucpPolyShape3 = ChipmunkProS.GeneratePolyShapeFromGameObject(gameObject.transform.Find("CollisionBody").gameObject, 1f, 0.25f, 0.9f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddStaticBody(transformC, new ucpShape[2] { ucpPolyShape2, ucpPolyShape3 }, m_unitC);
		m_beltShape = ucpPolyShape2.shapePtr;
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			ChipmunkProS.AddCollisionHandler(m_cmb, RampCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)3, true, true, true);
			ChipmunkProS.AddCollisionHandler(m_cmb, RampCollisionHandler, (ucpCollisionType)4, (ucpCollisionType)4, true, true, true);
		}
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	private void RampCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		if (unitC != null && unitC.m_unit != null && (_phase == ucpCollisionPhase.Begin || _phase == ucpCollisionPhase.Persist) && _pair.shapeA == m_beltShape)
		{
			if (_phase == ucpCollisionPhase.Begin && m_soundTriggerTimer <= 0 && unitC.m_unit.m_entity == Player.m_mainTransform.p_entity)
			{
				m_soundTriggerTimer = 60;
				SoundS.PlaySingleShot("/Ingame/Units/BoostRamp", new Vector3(_pair.point.x, _pair.point.y, 0f));
			}
			unitC.m_unit.SetAsSpeeding(2, new Vector2(_pair.normal.normalized.y, 0f - _pair.normal.normalized.x) * 20f, 750f);
		}
	}

	public override void Update()
	{
		base.Update();
		m_laneMat.mainTextureOffset = new Vector2(0f, ToolBox.getRolledValue((0f - Main.m_gameTime) / 1.5f, 0f, 1f));
		if (m_soundTriggerTimer > 0)
		{
			m_soundTriggerTimer--;
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
