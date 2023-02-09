using System;
using UnityEngine;

public class SoundDetector : MonoBehaviour
{
	private const float MIN_SOUND_DETECTION_VOLUME = 2f;

	public Action<CharacterController> OnPlayerDetected;
	public Action<float, GameObject> OnSoundDetected;

	[SerializeField]
	private float soundDetectionDistance = 5f;

	private void OnEnable() => SoundEmitter.OnSoundEmitted += OnSoundEmittedHandler;
	private void OnDisable() => SoundEmitter.OnSoundEmitted -= OnSoundEmittedHandler;

	private void OnSoundEmittedHandler(float volume, GameObject sourceObject)
	{
		float distance = Vector3.Distance(transform.position, sourceObject.transform.position);
		float volumeDistance = (volume - distance) + MIN_SOUND_DETECTION_VOLUME;
		if (distance <= soundDetectionDistance && volumeDistance >= MIN_SOUND_DETECTION_VOLUME)
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