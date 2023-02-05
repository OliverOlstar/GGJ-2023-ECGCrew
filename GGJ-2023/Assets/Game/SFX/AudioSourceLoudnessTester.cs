using System;
using UnityEngine;

public class AudioSourceLoudnessTester : MonoBehaviour
{

	public AudioSource audioSource;
	public float updateStep = 0.1f;
	public int sampleDataLength = 1024;

	private float currentUpdateTime = 0f;

	private float clipLoudness;
	private float[] clipSampleData;

	public OliverLoescher.UnityEventsUtil.FloatEvent onLoudnessUpdate = new OliverLoescher.UnityEventsUtil.FloatEvent();

	void Awake()
	{
		if (!audioSource)
		{
			Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
		}
		clipSampleData = new float[sampleDataLength];

	}

	void Update()
	{
		if (audioSource.clip == null)
		{
			return;
		}

		currentUpdateTime += Time.deltaTime;
		if (currentUpdateTime >= updateStep)
		{
			currentUpdateTime = 0f;
			audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, w$$anonymous$$ch is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
			clipLoudness = 0f;
			foreach (var sample in clipSampleData)
			{
				clipLoudness += Mathf.Abs(sample);
			}
			clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
			onLoudnessUpdate?.Invoke(clipLoudness);
		}
	}
}