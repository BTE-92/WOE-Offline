public class UnitTouchController : Controller
{
	public override void Open()
	{
		m_open = true;
	}

	public override void Close()
	{
		m_open = false;
	}
}
