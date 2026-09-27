using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelGroundNode : SimpleLevelNode
{
	public const int LAYERS = 5;

	public AutoGeometryLayer[] m_AGLayer = new AutoGeometryLayer[5];

	public byte[][] m_AGLayerData = new byte[5][];

	public static int m_AGLayerCount;

	public Entity m_entity;

	public LevelGroundNode()
		: base(null, "LevelGround", Vector3.zero, Vector3.zero, Vector3.one)
	{
		m_assembleClassType = typeof(BasicAssembledClass);
		m_AGLayerCount = m_AGLayerData.Length;
	}

	public LevelGroundNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		try
		{
			m_AGLayerCount = (int)info.GetValue("layerCount", typeof(int));
		}
		catch
		{
			m_AGLayerCount = 5;
		}
		m_AGLayer = new AutoGeometryLayer[m_AGLayerCount];
		m_AGLayerData = new byte[m_AGLayerCount][];
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			m_AGLayerData[i] = (byte[])info.GetValue("layer" + i + "Data", typeof(byte[]));
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("layerCount", m_AGLayerCount);
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			info.AddValue("layer" + i + "Data", m_AGLayer[i].m_bytes);
		}
	}

	public new LevelGroundNode DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			return (LevelGroundNode)binaryFormatter.Deserialize(memoryStream);
		}
	}

	public override void Assemble()
	{
		if (m_assembled)
		{
			return;
		}
		AutoGeometryManager.Initialize(LevelManager.m_currentLevel.m_currentLayer.m_layerWidth, LevelManager.m_currentLevel.m_currentLayer.m_layerHeight);
		Ground[] array = new Ground[5]
		{
			new SandGround(this),
			new IceGround(this),
			new MudGround(this),
			new MetalGround(this),
			new DangerousGround(this)
		};
		bool flag = m_AGLayerData[0] != null;
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			m_AGLayer[i] = AutoGeometryManager.AddLayer(array[i], 16, 17, 3f);
			if (flag)
			{
				m_AGLayerData[i].CopyTo(m_AGLayer[i].m_bytes, 0);
				m_AGLayer[i].MarchTiles(new cpBB(0f, 0f, AutoGeometryManager.m_width, AutoGeometryManager.m_height));
				GC.Collect();
			}
			else
			{
				m_AGLayerData[i] = new byte[m_AGLayer[i].m_bytes.Length];
				m_AGLayer[i].m_bytes.CopyTo(m_AGLayerData[i], 0);
			}
			m_AGLayer[i].TakeSnapshot();
			m_AGLayer[i].CopyByteArrayToMaskTexture(m_AGLayer[i].m_maskTexture, m_AGLayer[i].m_bytes);
		}
		for (int j = 0; j < m_AGLayerCount; j++)
		{
			AutoGeometryManager.UpdateMaxValueLookupTable(m_AGLayer[j]);
		}
		AutoGeometryManager.ClearTileDirtyFlags();
		m_assembled = true;
	}

	public void SaveGroundBeforePlay()
	{
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			Array.Copy(m_AGLayer[i].m_bytes, m_AGLayerData[i], m_AGLayer[i].m_bytes.Length);
		}
		AutoGeometryManager.ClearTileDirtyFlags();
	}

	public void RevertGroundFromPlay()
	{
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			m_AGLayer[i].RevertAllDirtyTiles(m_AGLayerData[i]);
		}
		AutoGeometryManager.ClearTileDirtyFlags();
	}

	public override void Clear(bool _isReset)
	{
		if (!_isReset)
		{
			AutoGeometryManager.DestroyAllLayers();
			(LevelManager.m_currentLevel as Minigame).m_groundNode = null;
			for (int i = 0; i < m_AGLayerCount; i++)
			{
				m_AGLayer[i] = null;
			}
			base.Clear(_isReset);
		}
	}
}
