using System;
using UnityEngine;

public class SoundDetector : MonoBehaviour
{
	public Action<CharacterController> OnPlayerDetected;
	public Action<float, GameObject> OnSoundDetected;

	[SerializeField]
	private float soundDetectionDistance = 5f;

	private void OnEnable() => SoundEmitter.OnSoundEmitted += OnSoundEmittedHandler;
	private void OnDisable() => SoundEmitter.OnSoundEmitted -= OnSoundEmittedHandler;

	private void OnTriggerEnter(Collider other)
	{
		/*
		if (other.TryGetComponent(out CharacterController playerController))
		{
			OnPlayerDetected?.Invoke(playerController);
		}
		*/
	}

	private void OnTriggerExit(Collider other)
	{
		/*
		if (other.TryGetComponent(out CharacterController playerController))
		{
			OnPlayerDetected?.Invoke(playerController);
		}
		*/
	}

	private void OnSoundEmittedHandler(float volume, GameObject sourceObject)
	{
		Log($"Sound <color=green>-Emitted-</color> from {sourceObject}, volume: {volume}");
		float distance = Vector3.Distance(transform.position, sourceObject.transform.position);
		if (distance <= soundDetectionDistance)
		{
			Log($"Sound <color=red>-Detected-</color> from {sourceObject}");
			OnSoundDetected?.Invoke(volume, sourceObject);
		}
	}

	private void Log(string log)
	{
		Debug.Log($"|{this}| {log}");
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, soundDetectionDistance);
	}
}