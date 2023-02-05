using System;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
	public static Action<float, GameObject> OnSoundEmitted;

	public void Emit(float volume)
	{
		// Get the Distance between the Sound Detector and the Sound Source (the Player)
		// Multiply by their state
		OnSoundEmitted?.Invoke(volume, gameObject);
	}
}