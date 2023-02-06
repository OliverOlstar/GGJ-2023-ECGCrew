using UnityEngine;

public class InteractableDoor : MonoBehaviour, IPlayerInteractable
{
	[SerializeField]
	private AudioSource audioSource;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece confirmAudio = new OliverLoescher.AudioUtil.AudioPiece();
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece failAudio = new OliverLoescher.AudioUtil.AudioPiece();
	[SerializeField]
	private AudioSource doorAudioSource;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece doorOpenAudio = new OliverLoescher.AudioUtil.AudioPiece();
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece doorCloseAudio = new OliverLoescher.AudioUtil.AudioPiece();
	[SerializeField]
	private Motion door = null;
	[SerializeField]
	private Motion doorB = null;

	[SerializeField]
	private int requiredItemID = -1;
	[SerializeField]
	private int requiredItemCount = 0;

	public bool IsSelectable => false;

	public void Hover() { }
	public void UnHover() { }

	public void Select()
	{
		if (requiredItemID >= 0 && !Item.Inventory.Contains(requiredItemID) || Item.Inventory.Count < requiredItemCount)
		{
			failAudio.Play(audioSource);
			return;
		}

		confirmAudio.Play(audioSource);
		door.Play(!door.reverse);
		if (doorB != null)
		{
			doorB.Play(door.reverse);
		}
		if (door.reverse)
		{
			doorCloseAudio.Play(doorAudioSource);
		}
		else
		{
			doorOpenAudio.Play(doorAudioSource);
		}
	}
	public void UnSelect() { }
}