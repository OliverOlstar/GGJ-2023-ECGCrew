using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private List<Item> items = new List<Item>();

	public void AddItem(Item item)
	{
		items.Add(item);
	}

	public int CheckForKeyCards()
	{
		return items.Count;
	}
}
