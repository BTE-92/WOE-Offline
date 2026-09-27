using UnityEngine;

public class Frame
{
	public float x;

	public float y;

	public float width;

	public float height;

	public bool flipX;

	public bool flipY;

	public Vector2 offset;

	public Frame(float x, float y, float width, float height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		flipX = false;
		flipY = false;
		offset = Vector2.zero;
	}

	public Frame(float x, float y, float width, float height, bool flipX, bool flipY)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.flipX = flipX;
		this.flipY = flipY;
		offset = Vector2.zero;
	}

	public float GetWidthRelativeTo(float value)
	{
		return width / value;
	}

	public float GetHeightRelativeTo(float value)
	{
		return height / value;
	}
}
