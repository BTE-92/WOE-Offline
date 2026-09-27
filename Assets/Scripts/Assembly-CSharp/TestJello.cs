using System;
using UnityEngine;

public class TestJello : Unit
{
	private TransformC m_tc;

	private ChipmunkBodyC[,] m_bodies;

	public TestJello(GraphElement _graphElement)
		: base(_graphElement, UnitType.Basic)
	{
		m_tc = TransformS.AddComponent(m_entity, _graphElement.m_name, _graphElement.m_position);
		float num = 150f;
		float damping = 1f;
		float num2 = 100f;
		float num3 = 100f;
		int num4 = 20;
		float num5 = (0f - (num2 - (float)num4)) / 2f;
		float num6 = (0f - (num3 - (float)num4)) / 2f;
		int num7 = Mathf.RoundToInt(num2 / (float)num4);
		int num8 = Mathf.RoundToInt(num3 / (float)num4);
		m_bodies = new ChipmunkBodyC[num7, num8];
		for (int i = 0; i < num8; i++)
		{
			for (int j = 0; j < num7; j++)
			{
				Vector3 vector = new Vector3(num5 + (float)(j * num4), num6 + (float)(i * num4), 0f);
				TransformC tc = TransformS.AddComponent(m_entity, _graphElement.m_name, _graphElement.m_position + vector);
				ucpShape shape = new ucpCircleShape(5f, Vector2.zero, 0.1f, 0.5f, 0.5f, (ucpCollisionType)4)
				{
					group = 100u
				};
				m_bodies[j, i] = ChipmunkProS.AddDynamicBody(tc, shape);
				if (j > 0)
				{
					ChipmunkBodyC chipmunkBodyC = m_bodies[j - 1, i];
					ChipmunkBodyC chipmunkBodyC2 = m_bodies[j, i];
					IntPtr constraint = ChipmunkProWrapper.ucpDampedSpringNew(chipmunkBodyC.body, chipmunkBodyC2.body, Vector2.zero, Vector2.zero, num4, num, damping);
					ChipmunkProWrapper.ucpAddConstraint(constraint, -1);
				}
				if (i > 0)
				{
					ChipmunkBodyC chipmunkBodyC3 = m_bodies[j, i - 1];
					ChipmunkBodyC chipmunkBodyC4 = m_bodies[j, i];
					IntPtr constraint2 = ChipmunkProWrapper.ucpDampedSpringNew(chipmunkBodyC3.body, chipmunkBodyC4.body, Vector2.zero, Vector2.zero, num4, num, damping);
					ChipmunkProWrapper.ucpAddConstraint(constraint2, -1);
				}
				if (j > 0 && i > 0)
				{
					ChipmunkBodyC chipmunkBodyC5 = m_bodies[j - 1, i];
					ChipmunkBodyC chipmunkBodyC6 = m_bodies[j, i - 1];
					IntPtr constraint3 = ChipmunkProWrapper.ucpDampedSpringNew(chipmunkBodyC5.body, chipmunkBodyC6.body, Vector2.zero, Vector2.zero, Mathf.Sqrt(num4 * num4 * 2), Mathf.Sqrt(num * num * 2f), damping);
					ChipmunkProWrapper.ucpAddConstraint(constraint3, -1);
					chipmunkBodyC5 = m_bodies[j - 1, i - 1];
					chipmunkBodyC6 = m_bodies[j, i];
					constraint3 = ChipmunkProWrapper.ucpDampedSpringNew(chipmunkBodyC5.body, chipmunkBodyC6.body, Vector2.zero, Vector2.zero, Mathf.Sqrt(num4 * num4 * 2), Mathf.Sqrt(num * num * 2f), damping);
					ChipmunkProWrapper.ucpAddConstraint(constraint3, -1);
				}
			}
		}
		if (PsState.m_gameState == GameState.Test || PsState.m_gameState == GameState.Play)
		{
			CameraTargetC cameraTargetC = CameraS.AddTargetComponent(m_tc, 450f, 450f);
			cameraTargetC.maxAngleChange = new Vector2(0f, 0f);
			CameraS.m_mainCameraMaxVelocity = 200f;
		}
		CreateEditorTouchArea();
	}

	private void CollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		Debug.Log("Goal Reached");
		PsState.m_gameEnded = true;
	}

	public override void Update()
	{
		int num = 4;
		Vector3 vector = m_bodies[0, 0].TC.transform.position + m_bodies[num, 0].TC.transform.position + m_bodies[num, num].TC.transform.position + m_bodies[0, num].TC.transform.position;
		vector /= 4f;
		DebugDraw.Clear(CameraS.m_mainCamera, m_tc);
		for (int i = 1; i < 5; i++)
		{
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_tc, m_bodies[i - 1, 0].TC.transform.position - vector, m_bodies[i, 0].TC.transform.position - vector);
		}
		for (int j = 1; j < 5; j++)
		{
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_tc, m_bodies[num, j - 1].TC.transform.position - vector, m_bodies[num, j].TC.transform.position - vector);
		}
		for (int k = 1; k < 5; k++)
		{
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_tc, m_bodies[num - (k - 1), num].TC.transform.position - vector, m_bodies[num - k, num].TC.transform.position - vector);
		}
		for (int l = 1; l < 5; l++)
		{
			DebugDraw.CreateLine(CameraS.m_mainCamera, m_tc, m_bodies[0, num - (l - 1)].TC.transform.position - vector, m_bodies[0, num - l].TC.transform.position - vector);
		}
		TransformS.SetPosition(m_tc, vector);
	}
}
