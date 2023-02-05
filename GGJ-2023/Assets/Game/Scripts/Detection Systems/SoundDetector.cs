using System;
using UnityEngine;

public class SoundDetector : MonoBehaviour
{
	public static Action<FirstPersonCharacterController> OnPlayerDetected;

	[SerializeField]
	private float soundDetectionDistance = 5f;

	private void OnEnable() => SoundEmitter.OnSoundEmitted += OnSoundDetected;
	private void OnDisable() => SoundEmitter.OnSoundEmitted -= OnSoundDetected;

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

	private void OnSoundDetected(float volume, GameObject sourceObject)
	{
		float distance = Vector3.Distance(transform.position, sourceObject.transform.position);
		if (distance <= soundDetectionDistance)
		{
			Log($"Sound detected from {sourceObject}");
		}
	}

	private void Log(string log)
	{
		Debug.Log($"|{this}| {log}");
	}
}