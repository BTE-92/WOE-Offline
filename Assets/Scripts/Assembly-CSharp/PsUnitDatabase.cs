using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public static class PsUnitDatabase
{
	public static List<PsUnitCategory> m_categories;

	public static void Initialize(string _JSON = null)
	{
		int num = 0;
		int num2 = 0;
		m_categories = new List<PsUnitCategory>();
		if (_JSON == null)
		{
			TextAsset textAsset = Resources.Load("PlaySomething/Units/UnitDatabase") as TextAsset;
			_JSON = textAsset.ToString();
		}
		Dictionary<string, object> dictionary = Json.Deserialize(_JSON) as Dictionary<string, object>;
		Dictionary<string, object> dictionary2 = dictionary["Units"] as Dictionary<string, object>;
		foreach (KeyValuePair<string, object> item in dictionary2)
		{
			PsUnitCategory psUnitCategory = new PsUnitCategory(item.Key);
			num++;
			Dictionary<string, object> dictionary3 = item.Value as Dictionary<string, object>;
			Dictionary<string, object> dictionary4 = dictionary3["Items"] as Dictionary<string, object>;
			psUnitCategory.description = (string)dictionary3["Description"];
			psUnitCategory.categoryType = (string)dictionary3["CategoryType"];
			foreach (KeyValuePair<string, object> item2 in dictionary4)
			{
				PsUnitCategoryItem psUnitCategoryItem = new PsUnitCategoryItem();
				psUnitCategoryItem.name = item2.Key;
				Dictionary<string, object> dictionary5 = item2.Value as Dictionary<string, object>;
				psUnitCategoryItem.description = (string)dictionary5["Description"];
				psUnitCategoryItem.levelRequirement = (int)(long)dictionary5["LevelRequirement"];
				psUnitCategoryItem.researchCost = (int)(long)dictionary5["ResearchCost"];
				psUnitCategoryItem.className = (string)dictionary5["ClassName"];
				psUnitCategoryItem.orderIndex = (int)(long)dictionary5["OrderIndex"];
				if (dictionary5.ContainsKey("IconImage"))
				{
					psUnitCategoryItem.iconImage = (string)dictionary5["IconImage"];
				}
				else
				{
					psUnitCategoryItem.iconImage = null;
				}
				psUnitCategory.items.Add(psUnitCategoryItem);
				num2++;
			}
			m_categories.Add(psUnitCategory);
			PsUnitCategoryItem[] array = psUnitCategory.items.ToArray();
			int[] array2 = new int[psUnitCategory.items.Count];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = psUnitCategory.items[i].orderIndex;
			}
			Array.Sort(array2, array);
			psUnitCategory.items = new List<PsUnitCategoryItem>(array);
		}
		Debug.Log("PsUnitDatabase parsed. Categories: " + num + " -- Units: " + num2);
	}

	public static PsUnitCategoryItem GetItemByClassName(string _className)
	{
		foreach (PsUnitCategory category in m_categories)
		{
			foreach (PsUnitCategoryItem item in category.items)
			{
				if (_className.Equals(item.className))
				{
					return item;
				}
			}
		}
		return null;
	}
}
