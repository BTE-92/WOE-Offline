public struct cpBB
{
	public float l;

	public float b;

	public float r;

	public float t;

	public cpBB(float _value)
	{
		l = _value;
		r = _value;
		t = _value;
		b = _value;
	}

	public cpBB(float _l, float _b, float _r, float _t)
	{
		l = _l;
		b = _b;
		r = _r;
		t = _t;
	}

	public override string ToString()
	{
		return "(" + l + ", " + b + ", " + r + ", " + t + ")";
	}
}
