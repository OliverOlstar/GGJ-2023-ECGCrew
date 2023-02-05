using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using OliverLoescher;

public class FirstPersonCharacterStamina : OliverLoescher.CharacterValue
{
	[Header("Breath Particles"), SerializeField]
	private ParticleSystem breathParticles = null;
	[SerializeField]
	private float lowOnBreath = 0.5f;
	[SerializeField]
	private float lowBreathStamina = 50.0f;
	[SerializeField]
	private float outOfBreath = 1.0f;

	[Header("Breath Audio")]
	[SerializeField]
	private AudioFader breathSource = null;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece defaultBreathLoopAudio = null;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece lowBreathLoopAudio = null;
	[SerializeField]
	private OliverLoescher.AudioUtil.AudioPiece outOfBreathAudio = null;

	private float defaultBreath = 0.0f;

	protected override void Start()
	{
		defaultBreath = breathParticles.emissionRate;

		Vector3 localPos = breathParticles.transform.localPosition;
		breathParticles.transform.SetParent(OliverLoescher.MainCamera.Camera.transform);
		breathParticles.transform.localPosition = localPos;

		breathSource.PlayInstant(defaultBreathLoopAudio);

		base.Start();
	}

	public override void OnValueOut()
	{
		OutOfBreath();
		base.OnValueOut();
	}

	public override void OnValueIn()
	{
		ResetBreath();
		base.OnValueIn();
	}

	public override void OnValueChanged(float pValue, float pChange)
	{
		if (!isOut)
		{
			if (pValue > lowBreathStamina)
			{
				ResetBreath();
			}
			else
			{
				LowBreath();
			}
		}

		base.OnValueChanged(pValue, pChange);
	}

	private void LowBreath()
	{
		if (breathParticles.emissionRate == lowOnBreath)
		{
			return;
		}
		breathParticles.emissionRate = lowOnBreath;
		breathSource.Play(lowBreathLoopAudio);
	}

	private void OutOfBreath()
	{
		if (breathParticles.emissionRate == outOfBreath)
		{
			return;
		}
		breathParticles.emissionRate = outOfBreath;
		breathSource.Play(outOfBreathAudio);
	}

	private void ResetBreath()
	{
		if (breathParticles.emissionRate == defaultBreath)
		{
			return;
		}
		breathParticles.emissionRate = defaultBreath;
		breathSource.Play(defaultBreathLoopAudio);
	}
}
