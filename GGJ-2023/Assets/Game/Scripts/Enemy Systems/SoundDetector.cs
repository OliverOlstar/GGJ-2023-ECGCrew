using System;
using UnityEngine;

public class SoundDetector : MonoBehaviour
{
	public static Action<FirstPersonCharacterController> OnPlayerDetected;

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out FirstPersonCharacterController playerController))
		{
			OnPlayerDetected?.Invoke(playerController);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out FirstPersonCharacterController playerController))
		{
			OnPlayerDetected?.Invoke(playerController);
		}
	}
}