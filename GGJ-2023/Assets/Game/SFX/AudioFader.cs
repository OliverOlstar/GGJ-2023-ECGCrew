using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class AudioFader : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource = null;
	[SerializeField]
	private AudioSourceLoudnessTester audioLoudness = null;
	private AudioUtil.AudioPiece currAudio = null;
	private bool fadeOut = false;

	[SerializeField, Min(0.0f)]
	private float magnituteCutoff = 1.0f;

	void Reset()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void Start()
	{
		audioLoudness.onLoudnessUpdate.AddListener(OnLoudnessUpdate);
	}

	void OnDestroy()
	{
		audioLoudness.onLoudnessUpdate.RemoveListener(OnLoudnessUpdate);
	}

	public void PlayInstant(AudioUtil.AudioPiece pPiece)
	{
		pPiece.Play(audioSource);
		currAudio = pPiece;
		fadeOut = false;
	}

	public void Play(AudioUtil.AudioPiece pPiece)
	{
		currAudio = pPiece;
		fadeOut = true;
	}

	void OnLoudnessUpdate(float pLoudness)
	{
		if (fadeOut && currAudio != null && pLoudness <= magnituteCutoff)
		{
			fadeOut = false;
			currAudio.Play(audioSource);
		}
	}
}
