public class CameraLayer : IPoolable
{
	private int _index;

	public int m_index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public void Reset()
	{
	}

	public void Destroy()
	{
	}
}
