using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Motion : MonoBehaviour
{
	[SerializeField]
	private bool useX = false;
	[ShowIf("useX"), SerializeField]
	private AnimationCurve xMotion = new AnimationCurve();
	[ShowIf("useX"), SerializeField]
	private float xScalar = 5.0f;
	[ShowIf("useX"), SerializeField]
	private float xFrequency = 5.0f;

	[Space, SerializeField]
	private bool useY = true;
	[ShowIf("useY"), SerializeField]
	private AnimationCurve yMotion = new AnimationCurve();
	[ShowIf("useY"), SerializeField]
	private float yScalar = 5.0f;
	[ShowIf("useY"), SerializeField]
	private float yFrequency = 5.0f;

	[Space, SerializeField]
	private bool useZ = false;
	[ShowIf("useZ"), SerializeField]
	private AnimationCurve zMotion = new AnimationCurve();
	[ShowIf("useZ"), SerializeField]
	private float zScalar = 5.0f;
	[ShowIf("useZ"), SerializeField]
	private float zFrequency = 5.0f;

	private Vector3 initialPosition = Vector3.zero;
	private float time = 0.0f;

	[Space, SerializeField]
	private bool useClamp = false;
	[SerializeField]
	private Vector2 timeClamp = Vector2.up;
	public bool reverse = false;

	void OnEnable()
	{
		initialPosition = transform.localPosition;
	}

	void Update()
	{
		Vector3 pos = Vector3.zero;
		time += Time.deltaTime * (reverse ? -1 : 1);
		time = Mathf.Clamp(time, timeClamp.x, timeClamp.y);
		if (useX)
		{
			pos.x = xMotion.Evaluate(time * xFrequency) * xScalar;
		}
		if (useY)
		{
			pos.y = yMotion.Evaluate(time * yFrequency) * yScalar;
		}
		if (useZ)
		{
			pos.z = zMotion.Evaluate(time * zFrequency) * zScalar;
		}
		transform.localPosition = pos + initialPosition;
	}

	public void Play(bool pReverse)
	{
		enabled = true;
		reverse = pReverse;
	}

	public void Stop()
	{
		enabled = false;
	}
}
