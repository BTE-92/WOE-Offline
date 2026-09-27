public interface IPoolable
{
	int m_index { get; set; }

	void Reset();

	void Destroy();
}
