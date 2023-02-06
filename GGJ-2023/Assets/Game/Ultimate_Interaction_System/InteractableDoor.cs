using System.Collections;
using System.Collections.Generic;
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

	public bool canOpen = true;

	public bool IsSelectable => false;

	public void Hover() { }
	public void UnHover() { }

	public void Select()
	{
		if (!canOpen)
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
