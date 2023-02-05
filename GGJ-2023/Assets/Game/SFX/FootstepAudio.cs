using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class FootstepAudio : MonoBehaviour
{
	[SerializeField]
	private AudioSourcePool audioSource = null;
	[SerializeField]
	private FirstPersonCameraMovement movement = null;
	[SerializeField]
	private FirstPersonCharacterController controller = null;

	[Space, SerializeField]
	private AudioUtil.AudioPiece stepAudio = new AudioUtil.AudioPiece();
	[SerializeField]
	private AudioUtil.AudioPiece sprintAudio = new AudioUtil.AudioPiece();
	[SerializeField]
	private AudioUtil.AudioPiece crouchAudio = new AudioUtil.AudioPiece();
	[SerializeField]
	private AudioUtil.AudioPiece landAudio = new AudioUtil.AudioPiece();

    void Start()
	{
		movement.onStep.AddListener(OnStep);
		movement.onCrouchStep.AddListener(OnCrouchStep);
		movement.onLanded.AddListener(OnLanded);
	}

	private void OnStep()
	{
		if (controller.IsSprinting)
		{
			sprintAudio.Play(audioSource.GetSource());
		}
		else
		{
			stepAudio.Play(audioSource.GetSource());
		}
	}

	private void OnCrouchStep()
	{
		crouchAudio.Play(audioSource.GetSource());
	}

	private void OnLanded(float pForce)
	{
		AudioSource source = audioSource.GetSource();
		landAudio.Play(source);
		// Debug.Log($"Landed: {pForce}");
		source.volume *= source.volume * Mathf.Min(1.0f, -pForce * 0.25f);
	}
}
