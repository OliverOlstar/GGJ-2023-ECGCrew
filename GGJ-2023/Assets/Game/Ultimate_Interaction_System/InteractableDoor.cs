using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

	[Space, SerializeField]
	private int requiredItemID = -1;
	[SerializeField]
	private int requiredItemCount = 0;
	[SerializeField]
	private bool canOpen = true;

	[SerializeField]
	private float closeAfterDelay = 0.0f;
	[SerializeField]
	private int closeAfterDelayWithoutItemID = -1;

	[SerializeField]
	private SoundEmitter emitter = null;
	[SerializeField]
	private float soundEmissionVolume = 15f;


	public bool IsSelectable => false;

	public void Hover() { }
	public void UnHover() { }

	[Button]
	public void Select()
	{
		EmitSound();
		if (!canOpen || (requiredItemID >= 0 && !Item.Inventory.Contains(requiredItemID)) || Item.Inventory.Count < requiredItemCount)
		{
			failAudio.Play(audioSource);
			return;
		}

		StopAllCoroutines();

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

			if (closeAfterDelay > 0.0f && !Item.Inventory.Contains(closeAfterDelayWithoutItemID))
			{
				StartCoroutine(CloseAfterDelay());
			}
		}
	}
	public void UnSelect() { }

	private IEnumerator CloseAfterDelay()
	{
		yield return new WaitForSeconds(closeAfterDelay);
		
		if (!door.reverse)
		{
			door.Play(true);
			if (doorB != null)
			{
				doorB.Play(door.reverse);
			}
			doorCloseAudio.Play(doorAudioSource);
		}

		EmitSound();
	}

	private void EmitSound()
	{
		emitter.Emit(soundEmissionVolume);
	}
}