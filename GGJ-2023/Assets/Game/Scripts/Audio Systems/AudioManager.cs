using OliverLoescher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourSingleton<AudioManager>, ISingleton
{
	[SerializeField]
	private AudioSource sfxSource;
	[SerializeField]
	private AudioSource musicSource;
	[SerializeField]
	private AudioSource voicesSource;
	[SerializeField]
	private AudioClip[] audioClips;

	public void PlaySFX(AudioClip sfxClip, bool playOneShot = false)
	{
		if (playOneShot)
		{
			sfxSource.clip = sfxClip;
			sfxSource.PlayOneShot(sfxClip);
		}
		else
		{
			sfxSource.Stop();
			sfxSource.clip = sfxClip;
			sfxSource.Play();
		}
	}

	public void PlayMusic(AudioClip musicClip)
	{
		if (SameAudioClip(musicSource, musicClip)) return;

		musicSource.Stop();
		musicSource.clip = musicClip;
		musicSource.Play();
	}

	private void PlayRandomMusic()
	{
		int randomClipIndex = UnityEngine.Random.Range(0, audioClips.Length);
		if (SameAudioClip(musicSource, audioClips[randomClipIndex])) return;

		musicSource.clip = audioClips[randomClipIndex];
		musicSource.Play();
	}

	private bool SameAudioClip(AudioSource source, AudioClip clip)
	{
		return source.clip == clip;
	}

	public void OnAccessed()
	{

	}
}