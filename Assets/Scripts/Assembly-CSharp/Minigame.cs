using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Minigame : Level
{
	public string m_minigameId;

	public LevelGroundNode m_groundNode;

	public bool m_published;

	public Entity m_environmentEntity;

	private ChipmunkBodyC m_groundBody;

	private GroundC m_groundC;

	private SoundC m_music;

	public Ghost m_ghost;

	public Minigame(int _width, int _height)
		: base(_width, _height)
	{
	}

	public Minigame(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override void ApplySettings()
	{
		PsState.m_sessionBestTime = int.MaxValue;
		PsState.m_sessionLongestRunTicks = 0;
		LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
		if (levelPlayerNode != null)
		{
			PsState.m_editorCameraPos = levelPlayerNode.m_position;
		}
		m_environmentEntity = EntityManager.AddEntity("MinigameEnvironmentEntity");
		TransformC transformC = TransformS.AddComponent(m_environmentEntity, "MinigameEnvironmentTC", new Vector3(0f, (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, -10f));
		string text = m_settings["environment"] as string;
		if (text != null && !(text == "default"))
		{
			return;
		}
		m_music = SoundS.AddComponent(transformC, "/Music/DesertAmbience", 1f, true);
		SoundS.PlaySound(m_music);
		LevelLayer currentLayer = LevelManager.m_currentLevel.m_currentLayer;
		Vector2 a = new Vector2((float)currentLayer.m_layerWidth * -0.5f, 0f);
		Vector2 vector = new Vector2((float)currentLayer.m_layerWidth * 0.5f, 0f);
		Vector2 b = new Vector2((float)currentLayer.m_layerWidth * -0.5f, currentLayer.m_layerHeight);
		Vector2 b2 = new Vector2((float)currentLayer.m_layerWidth * 0.5f, currentLayer.m_layerHeight);
		ucpSegmentShape ucpSegmentShape2 = new ucpSegmentShape(a, vector, 30f, Vector2.zero, 0f, 0.5f, 0.8f, (ucpCollisionType)2);
		ucpSegmentShape ucpSegmentShape3 = new ucpSegmentShape(a, b, 30f, Vector2.zero, 0f, 0.5f, 0.8f, (ucpCollisionType)2);
		ucpSegmentShape ucpSegmentShape4 = new ucpSegmentShape(vector, b2, 30f, Vector2.zero, 0f, 0.5f, 0.8f, (ucpCollisionType)2);
		ucpSegmentShape2.layers = 286331153u;
		ucpSegmentShape3.layers = 286331153u;
		ucpSegmentShape4.layers = 286331153u;
		m_groundBody = ChipmunkProS.AddStaticBody(transformC, new ucpShape[3] { ucpSegmentShape2, ucpSegmentShape3, ucpSegmentShape4 });
		m_groundC = PsS.AddGround(m_environmentEntity, new SandGround(m_groundNode));
		m_groundBody.customComponent = m_groundC;
		Vector2[] rect = DebugDraw.GetRect(7500f, 25000f, Vector2.up * 12500f);
		float num = 0.3f;
		float num2 = 0f;
		float num3 = 2500f;
		float num4 = (float)(LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -2) - num3;
		float num5 = LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 2;
		int num6 = 0;
		float z = -25f;
		while (num4 < num5 + num3 * 2f)
		{
			if (num4 > (float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f && num4 < (float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f)
			{
				PrefabC c = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, z) + Vector3.right * num4 + Vector3.up * 27f, ResourceManager.GetGameObject("Desert/DesertRoadTerrain"));
				PrefabS.SetCameraLayer(c, 9);
				PrefabC c2 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, z) + Vector3.right * num4 + Vector3.up * 27f, ResourceManager.GetGameObject("Desert/DesertFrontTerrain"));
				PrefabS.SetCameraLayer(c2, 9);
			}
			if (num6 == 1)
			{
				PrefabC c3 = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, z) + Vector3.right * num4 + Vector3.up * 27f, ResourceManager.GetGameObject("Desert/DesertBackgroundTerrain"));
				PrefabS.SetCameraLayer(c3, 9);
				UVRect normalizeRect = new UVRect(num2, 0f, num, 1f);
				num2 += num;
				PrefabS.CreateFlatPrefabComponentsFromVectorArray(transformC, new Vector3(0f, -7000f, 25000f) + Vector3.right * num4, rect, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), ResourceManager.GetMaterial("Desert/DesertBackgroundMat"), CameraS.m_mainCamera, string.Empty, normalizeRect);
			}
			num4 += num3;
			num6 = ((num6 < 2) ? (num6 + 1) : 0);
		}
		List<cpBB> list = new List<cpBB>();
		UnityEngine.Random.seed = 32123324;
		for (int i = 0; i < 10; i++)
		{
			string resourceIdentifier = "Props/DesertMiddlePropCliff1";
			float num7 = 1500f;
			float num8 = 1000f;
			if (UnityEngine.Random.value < 0.5f)
			{
				resourceIdentifier = "Props/DesertMiddlePropCliff2";
				num7 = 1000f;
				num8 = 400f;
			}
			Vector3 offset = Vector3.zero;
			bool flag = false;
			int num9 = 0;
			while (!flag && num9 < 5000)
			{
				offset = Vector3.right * UnityEngine.Random.Range(-8000, 8000) + Vector3.forward * (1000 + UnityEngine.Random.Range(-200, 1000)) + Vector3.up * 27f;
				cpBB cpBB2 = new cpBB(offset.x + num7 * -0.5f, offset.z + num8 * -0.5f, offset.x + num7 * 0.5f, offset.z + num8 * 0.5f);
				bool flag2 = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (ChipmunkProWrapper.ucpBBIntersects(cpBB2, list[j]))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					list.Add(cpBB2);
					flag = true;
				}
				else
				{
					num9++;
				}
			}
			PrefabC prefabC = PrefabS.AddComponent(transformC, offset, ResourceManager.GetGameObject(resourceIdentifier));
			PrefabS.SetCamera(prefabC, CameraS.m_mainCamera);
			Vector3 localScale = Vector3.one * 0.3f;
			if (UnityEngine.Random.value < 0.5f)
			{
				localScale.x *= -1f;
			}
			prefabC.p_gameObject.transform.localScale = localScale;
		}
	}

	public override void Destroy()
	{
		EntityManager.RemoveEntity(m_environmentEntity);
		m_environmentEntity = null;
		m_groundBody = null;
		m_groundC = null;
		if (m_ghost != null)
		{
			m_ghost.Destroy();
			m_ghost = null;
		}
		base.Destroy();
	}
}
