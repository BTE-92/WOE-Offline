using UnityEngine;

public class Crate : Unit
{
	private ChipmunkBodyC m_cmb;

	public Crate(GraphElement _graphElement, float _size, float _friction = 0.8f, float _elasticity = 0.5f, float _weight = 1f, string _resource = "Units/BasicCratePrefab")
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(_resource);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(_size, _size, Vector2.zero, _weight, _elasticity, _friction, (ucpCollisionType)4);
		ChipmunkBodyC cmb = ChipmunkProS.AddDynamicBody(transformC, shape, m_unitC);
		m_cmb = cmb;
		PrefabC prefabC = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f), gameObject);
		prefabC.p_gameObject.transform.localScale = new Vector3(_size, _size, _size) * 1.15f;
		prefabC.p_gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
		CreateEditorTouchArea(_size, _size);
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		Vector2 vector = ChipmunkProWrapper.ucpBodyGetPos(m_cmb.body);
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject("ParticleFx/WoodenCratePuff"), new Vector3(vector.x, vector.y, 0f), Vector3.zero, 1f, "GTAG_INGAME_PARTICLES");
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
