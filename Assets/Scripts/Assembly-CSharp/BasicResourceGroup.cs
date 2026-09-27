using System.Collections.Generic;

public class BasicResourceGroup : IResourceGroup
{
	private static int _instanceCount;

	private string _identifier;

	private List<IResource> _resources;

	public string identifier
	{
		get
		{
			return _identifier;
		}
		set
		{
			_identifier = value;
		}
	}

	public List<IResource> resources
	{
		get
		{
			return _resources;
		}
		set
		{
			_resources = value;
		}
	}

	public BasicResourceGroup(string _identifier)
	{
		identifier = _identifier;
		resources = new List<IResource>();
		_instanceCount++;
	}

	~BasicResourceGroup()
	{
		_instanceCount--;
	}
}
