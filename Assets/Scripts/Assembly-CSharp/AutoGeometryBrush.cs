using UnityEngine;

public class AutoGeometryBrush
{
	public int m_width;

	public int m_height;

	public byte[] m_bytes;

	public bool m_subPixelAccuracy;

	public Texture2D m_texture;

	public AutoGeometryBrush(float _brushSize, bool _rectBrush, float _flow = 0.5f, float _solidArea = 0f)
	{
		m_subPixelAccuracy = _brushSize < 3f && !_rectBrush;
		m_width = Mathf.FloorToInt(_brushSize * 2f);
		m_height = Mathf.FloorToInt(_brushSize * 2f);
		float num = Mathf.Floor((float)m_width * 0.5f);
		float num2 = Mathf.Floor((float)m_height * 0.5f);
		float num3 = (float)m_width * 0.5f;
		Vector2 zero = Vector2.zero;
		m_bytes = new byte[m_width * m_height];
		if (!_rectBrush)
		{
			for (int i = 0; i < m_height; i++)
			{
				zero.y = (float)i - num2;
				for (int j = 0; j < m_width; j++)
				{
					zero.x = (float)j - num;
					float magnitude = zero.magnitude;
					float min = num3 * _solidArea;
					float num4 = 1f - ToolBox.getPositionBetween(magnitude, min, num3);
					byte b = (byte)(num4 * _flow * 255f);
					int num5 = i * m_width + j;
					m_bytes[num5] = b;
				}
			}
		}
		else
		{
			for (int k = 0; k < m_bytes.Length; k++)
			{
				m_bytes[k] = byte.MaxValue;
			}
		}
	}

	public AutoGeometryBrush(string _resourceName, bool _useSubPixelAccuracy, bool _storeBytes = true)
	{
		m_subPixelAccuracy = _useSubPixelAccuracy;
		if (_resourceName != null)
		{
			m_texture = ResourceManager.GetTexture(_resourceName) as Texture2D;
			m_width = m_texture.width;
			m_height = m_texture.height;
			if (_storeBytes)
			{
				Color[] pixels = m_texture.GetPixels();
				m_bytes = new byte[pixels.Length];
				for (int i = 0; i < pixels.Length; i++)
				{
					m_bytes[i] = (byte)(pixels[i].a * 255f);
				}
			}
		}
		else
		{
			m_width = 1;
			m_height = 1;
			m_bytes = new byte[1];
			m_bytes[0] = byte.MaxValue;
		}
	}

	public void Destroy()
	{
		m_bytes = null;
		m_texture = null;
	}
}
