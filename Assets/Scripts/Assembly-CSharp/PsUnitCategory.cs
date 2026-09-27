using System.Collections.Generic;

public class PsUnitCategory
{
	public string name;

	public string description;

	public string categoryType;

	public List<PsUnitCategoryItem> items;

	public PsUnitCategory(string name)
	{
		this.name = null;
		categoryType = string.Empty;
		description = string.Empty;
		items = new List<PsUnitCategoryItem>();
	}
}
