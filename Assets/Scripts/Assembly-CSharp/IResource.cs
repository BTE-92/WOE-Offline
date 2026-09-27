public interface IResource
{
	IResourceGroup resourceGroup { get; set; }

	string identifier { get; set; }

	object resourceObject { get; set; }

	void Unload();
}
