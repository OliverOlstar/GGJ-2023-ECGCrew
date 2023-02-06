using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	[SerializeField]
	private Inventory inventory = null;

	public void OnSelected()
	{
		inventory.AddItem(this);
		Destroy(this);
	}
}
