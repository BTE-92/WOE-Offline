public class DrawUndoAction : UndoAction
{
	public ByteBlock m_oldBytes;

	public ByteBlock m_newBytes;

	public AutoGeometryLayer m_layer;

	public DrawUndoAction(AutoGeometryLayer _layer)
	{
		if (_layer.m_snapshotBytes != null)
		{
			m_layer = _layer;
			cpBB undoRect = _layer.GetUndoRect();
			m_oldBytes = _layer.ReadByteBlock(ref _layer.m_snapshotBytes, undoRect);
			m_newBytes = _layer.ReadByteBlock(ref _layer.m_bytes, undoRect);
			UndoManager.Add(this);
		}
	}

	private void MarchByteBlock(ByteBlock _block)
	{
		m_layer.MarchTiles(_block.worldBB);
	}

	public override void ApplyUndo()
	{
		m_layer.WriteByteBlock(m_oldBytes, ref m_layer.m_bytes);
		MarchByteBlock(m_oldBytes);
	}

	public override void ApplyRedo()
	{
		m_layer.WriteByteBlock(m_newBytes, ref m_layer.m_bytes);
		MarchByteBlock(m_newBytes);
	}
}
