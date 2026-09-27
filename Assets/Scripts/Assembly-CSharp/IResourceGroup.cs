using System.Collections.Generic;

public interface IResourceGroup
{
	string identifier { get; set; }

	List<IResource> resources { get; set; }
}
