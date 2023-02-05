using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class EnemyFootsteps : MonoBehaviour
{
    [SerializeField]
	private AudioSourcePool audioSource = null;
    [SerializeField]
	private EnemyController controller = null;

	[Space, SerializeField]
	private AudioUtil.AudioPiece stepAudio = new AudioUtil.AudioPiece();
	[SerializeField]
	private AudioUtil.AudioPiece chaseAudio = new AudioUtil.AudioPiece();

	private void OnStep()
	{
		if (controller.CharacterState == EnemyPatrolState.CHASE)
		{
			chaseAudio.Play(audioSource.GetSource());
		}
		else
		{
			stepAudio.Play(audioSource.GetSource());
		}
	}
}
