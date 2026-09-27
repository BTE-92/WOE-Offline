using UnityEngine;

public class SpriteC : BasicComponent
{
	public static int m_componentCount;

	public bool visible;

	public bool wasVisible;

	public Vector3 align;

	public Vector3 offset;

	public Vector3 offsetRight;

	public Vector3 offsetUp;

	public float width;

	public float height;

	public Vector3 scaledRelRight;

	public Vector3 scaledRelUp;

	public Vector3 scaledRelOffset;

	public Vector3 relRight;

	public Vector3 relUp;

	public Vector3 relOffset;

	public float wScale;

	public float hScale;

	public float dimensionScale;

	public float wDimension;

	public float hDimension;

	public TransformC p_TC;

	public SpriteSheet p_spriteSheet;

	public Color color;

	public Frame frame;

	public int meshIndex;

	public int vertDataIndex;

	public float sortValue;

	public bool updatePosition;

	public bool updateUVs;

	public bool updateColors;

	public bool updateRotation;

	public bool updateScale;

	public SpriteC()
		: base(ComponentType.Sprite)
	{
		vertDataIndex = -1;
		Reset();
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		visible = true;
		wasVisible = true;
		updatePosition = true;
		updateColors = true;
		updateRotation = true;
		updateScale = true;
		updateUVs = true;
		offset = Vector3.zero;
		offsetRight = Vector3.right;
		offsetUp = Vector3.up;
		align = Vector3.zero;
		relRight = Vector3.right;
		relUp = Vector3.up;
		scaledRelRight = relRight;
		scaledRelUp = relUp;
		sortValue = 0f;
		wScale = 1f;
		hScale = 1f;
		dimensionScale = 1f;
		wDimension = 0f;
		hDimension = 0f;
		color = SpriteS.m_defaultColor;
	}

	public override void Destroy()
	{
	}

	~SpriteC()
	{
		m_componentCount--;
	}
}
