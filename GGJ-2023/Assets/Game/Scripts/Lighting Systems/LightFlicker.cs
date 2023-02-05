using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	[SerializeField]
	private Light lightObject = null;
	[SerializeField]
	private float minIntensity = 0.0f;
	[SerializeField]
	private float maxIntensity = 10.0f;
	[SerializeField]
	private float duration = 1.0f;


	private void Update()
	{
		// set light color
		float t = Mathf.PingPong(Time.time, duration) / duration;
		lightObject.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
	}

}
