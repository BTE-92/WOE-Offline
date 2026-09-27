using UnityEngine;

public class MediumPlank : Unit
{
	private ChipmunkBodyC m_cmb;

	public MediumPlank(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(250f, 10f, Vector2.zero, 12f, 0.25f, 0.8f, (ucpCollisionType)4);
		m_cmb = ChipmunkProS.AddDynamicBody(transformC, shape);
		m_cmb.customComponent = m_unitC;
		PrefabC c = PrefabS.AddComponent(transformC, Vector3.zero, ResourceManager.GetGameObject("Units/Plank250"));
		PrefabS.SetCamera(c, CameraS.m_mainCamera);
		CreateEditorTouchArea(250f, 10f);
		m_graphElement.m_isRotateable = true;
	}

	public override void Kill(DamageType _damageType, float _totalDamage)
	{
		base.Kill(_damageType, _totalDamage);
		Destroy();
	}
}
