using UnityEngine;

public class ProtoBox : Unit
{
	private ChipmunkBodyC m_cmb;

	public ProtoBox(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(50f, 50f, Vector2.zero, 62.500004f, 0.25f, 0.5f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(transformC, shape);
		m_cmb.customComponent = m_unitC;
		PrefabC c = PrefabS.AddComponent(transformC, Vector3.zero, ResourceManager.GetGameObject("DynamicProps/Box"));
		PrefabS.SetCamera(c, CameraS.m_mainCamera);
		SetBaseArmor(DamageType.Impact, 100f);
		CreateEditorTouchArea(50f, 50f);
		m_graphElement.m_isRotateable = true;
	}

	public override void KillingImpact(ChipmunkBodyC _collidingBody, Vector2 _point, Vector2 _normal, Vector2 _impulse)
	{
		Vector3 position = m_cmb.TC.transform.position;
		Vector3 eulerAngles = m_cmb.TC.transform.rotation.eulerAngles;
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetRot(m_cmb.body);
		Vector2 value = ChipmunkProWrapper.ucpBodyGetVel(m_cmb.body);
		if (_impulse.magnitude > 30000f)
		{
			_impulse = _impulse.normalized * 30000f;
		}
		for (int i = 0; i < 3; i++)
		{
			Entity entity = EntityManager.AddEntity("Effect");
			TransformC transformC = TransformS.AddComponent(entity, "Effect", position + (Vector3)vector * (float)(-20 + 20 * i), eulerAngles);
			ucpPolyShape ucpPolyShape2 = new ucpPolyShape(50f, 12.5f, Vector2.zero, 0.78125006f, 0.25f, 0.5f, (ucpCollisionType)4);
			ucpPolyShape2.layers = 16u;
			ucpPolyShape2.group = 100u;
			ChipmunkBodyC chipmunkBodyC = ChipmunkProS.AddDynamicBody(transformC, ucpPolyShape2);
			PrefabC prefabC = PrefabS.AddComponent(transformC, Vector3.forward * Random.Range(-35, -50), ResourceManager.GetGameObject("DynamicProps/BoxPlank"));
			PrefabS.SetCamera(prefabC, CameraS.m_mainCamera);
			prefabC.p_gameObject.transform.localRotation = Quaternion.identity;
			EventC eventC = EventS.AddComponent(entity, "EffectFade", EffectEventHandler, 2f, true, false, false, true);
			ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, value);
			ChipmunkProWrapper.ucpBodyApplyImpulse(chipmunkBodyC.body, _impulse * 0.001f, ChipmunkProWrapper.ucpBodyWorld2Local(chipmunkBodyC.body, _point));
		}
	}

	private void EffectEventHandler(EventC _c)
	{
		EntityManager.RemoveEntity(_c.p_entity);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
