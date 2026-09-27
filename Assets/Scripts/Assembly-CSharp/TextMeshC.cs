using UnityEngine;

public class TextMeshC : BasicComponent
{
	public bool m_wasVisible;

	public GameObject m_go;

	public Renderer m_renderer;

	public TextMesh m_textMesh;

	public TransformC m_TC;

	public float m_textboxWidth;

	public float m_textboxHeight;

	public float m_textHeight;

	public float m_textWidth;

	public Align m_horizontalAlign;

	public Align m_verticalAlign;

	public TextMeshC()
		: base(ComponentType.TextMesh)
	{
	}

	public override void Reset()
	{
		base.Reset();
		m_textboxWidth = 0f;
		m_textboxHeight = 0f;
		m_textWidth = 0f;
		m_textHeight = 0f;
	}
}
