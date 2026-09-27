using UnityEngine;

public class UnityResource : IResource
{
	private static int _instanceCount;

	private IResourceGroup _resourceGroup;

	private string _identifier;

	private object _resourceObject;

	public IResourceGroup resourceGroup
	{
		get
		{
			return _resourceGroup;
		}
		set
		{
			_resourceGroup = value;
		}
	}

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

	public object resourceObject
	{
		get
		{
			return _resourceObject;
		}
		set
		{
			_resourceObject = value;
		}
	}

	public UnityResource(string _identifier, string _path)
	{
		identifier = _identifier;
		Load(_path);
		_instanceCount++;
	}

	public UnityResource(Object _asset, string _bundleName)
	{
		identifier = _bundleName + "/" + _asset.name;
		resourceObject = _asset;
		_instanceCount++;
	}

	private void Load(string _path)
	{
		if (_path != null)
		{
			resourceObject = Resources.Load(_path);
		}
	}

	public void Unload()
	{
		Object obj = resourceObject as Object;
		if (obj.GetType() != typeof(GameObject))
		{
			Resources.UnloadAsset(obj);
		}
		resourceObject = null;
	}

	~UnityResource()
	{
		_instanceCount--;
	}
}
