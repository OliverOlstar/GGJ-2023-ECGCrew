using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public static List<int> Inventory = new List<int>();

	[SerializeField]
	private int itemID = 0;
	[SerializeField]
	private AudioSource source = null;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece collectAudio = new OliverLoescher.AudioUtil.AudioPiece();

	public void OnSelected()
	{
		Inventory.Add(itemID);
		source.transform.SetParent(null);
		collectAudio.Play(source);
		Destroy(gameObject);
	}
}
