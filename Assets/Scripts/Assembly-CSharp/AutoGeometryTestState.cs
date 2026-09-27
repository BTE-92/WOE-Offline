using UnityEngine;

public class AutoGeometryTestState : BasicState
{
	private AutoGeometryLayer m_agLayer;

	public override void Enter(IStatedObject _parent)
	{
		DebugDraw.m_lineWidth = 0.2f;
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "AutoGeometry Test (LMB - Add, RMB - Sub)");
	}

	public override void Execute()
	{
		if (!Input.GetMouseButton(0))
		{
		}
	}

	public override void Exit()
	{
		AutoGeometryManager.DestroyAllLayers();
		AutoGeometryManager.DestroyAllBrushes();
		EntityManager.RemoveAllEntities();
	}
}
