using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	private float baseSoundVolume = 1f;
	[SerializeField]
	private float soundDetectionVolume = 10f;

	public static Action<float, GameObject> OnSoundEmitted;

	public void Emit(float volume)
	{
		// Get the Distance between the Sound Detector and the Sound Source (the Player)
		// Multiply by their state
		OnSoundEmitted?.Invoke(volume, gameObject);
	}
}