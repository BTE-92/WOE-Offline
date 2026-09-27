using System;
using UnityEngine;

public class SawBlade : Unit
{
	private ChipmunkBodyC m_cmb;

	private float m_angle;

	public SawBlade(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject("Units/SawBladePrefab");
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f) + GetZBufferBias(), gameObject);
		ucpCircleShape shape = new ucpCircleShape(10f, Vector2.zero, 1f, 0.25f, 0.8f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddRogueBody(transformC, shape, m_unitC);
		CreateEditorTouchArea(prefabC.p_gameObject);
		m_graphElement.m_isRotateable = true;
	}

	public override void Update()
	{
		base.Update();
		if ((PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play) && Player.m_gameIsStarted)
		{
			m_angle = ToolBox.getCappedAngle(m_angle - 0.3f);
			ChipmunkProWrapper.ucpBodySetAngle(m_cmb.body, (m_graphElement.m_rotation.z + m_angle) * ((float)Math.PI / 180f));
		}
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
