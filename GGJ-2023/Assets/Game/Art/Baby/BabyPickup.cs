using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyPickup : MonoBehaviour, IPlayerInteractable
{
	[SerializeField]
	private OliverLoescher.HighlightModelBehaviour highlight = null;
	[SerializeField]
	private GameObject toggleOnSelect = null;
	[SerializeField]
	private int itemID = 99;

	public bool IsSelectable => false;

	void Start()
	{
		highlight.Clear();
		toggleOnSelect.SetActive(false);
	}

	public void Hover() { highlight.Clear(); }

	public void Select()
	{
		highlight.Clear();
		toggleOnSelect.SetActive(true);
		Item.Inventory.Add(itemID);
		Destroy(gameObject);
	}

	public void UnHover() { highlight.Set(Color.green); }

	public void UnSelect() {}
}
