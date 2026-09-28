using UnityEngine;

public static class ProjectorS
{
	public static DynamicArray<ProjectorC> m_components;

	public static void Initialize()
	{
		m_components = new DynamicArray<ProjectorC>();
	}

	public static void Update()
	{
		int aliveCount = m_components.m_aliveCount;
		for (int i = 0; i < aliveCount; i++)
		{
			int num = m_components.m_aliveIndices[i];
			ProjectorC projectorC = m_components.m_array[num];
			TransformC p_TC = projectorC.p_TC;
			if (p_TC.updatedPosition)
			{
				projectorC.m_projector.gameObject.transform.position = p_TC.transform.position + projectorC.m_offset;
			}
		}
	}

	public static ProjectorC AddComponent(TransformC _parentTC, Material _mat, int _ignoreLayers, Vector3 _offset)
	{
		ProjectorC projectorC = m_components.AddItem();
		projectorC.p_TC = _parentTC;
		GameObject gameObject = new GameObject(_parentTC.transform.name + " shadow");
		projectorC.m_projector = gameObject.AddComponent<Projector>() as Projector;
		projectorC.m_projector.material = _mat;
		projectorC.m_projector.ignoreLayers = _ignoreLayers;
		projectorC.m_projector.gameObject.transform.Rotate(new Vector3(90f, 0f, 0f));
		projectorC.m_projector.gameObject.layer = CameraS.m_mainCamera.gameObject.layer;
		projectorC.m_offset = _offset;
		projectorC.m_projector.orthographic = true;
		projectorC.m_projector.orthographicSize = 55f;
		projectorC.m_projector.farClipPlane = 200f;
		EntityManager.AddComponentToEntity(_parentTC.p_entity, projectorC);
		return projectorC;
	}

	public static void RemoveComponent(ProjectorC _c)
	{
		Object.DestroyImmediate(_c.m_projector.gameObject);
		_c.m_projector = null;
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}
}
