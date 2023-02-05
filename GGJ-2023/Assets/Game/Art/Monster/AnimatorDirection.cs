using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using OliverLoescher;

[RequireComponent(typeof(Animator))]
public class AnimatorDirection : MonoUtil.MonoBehaviour
{
	[SerializeField]
	private Transform myForward = null;
	[SerializeField]
	private NavMeshAgent agent = null;
	[SerializeField]
	private Animator animator = null;

	[Header("Speed")]
	[SerializeField]
	private float speedScalar = 0.2f;

	protected override void Tick(float pDeltaTime)
	{
		float angle = Vector3.SignedAngle(MainCamera.Camera.transform.forward, myForward.forward, Vector3.up);
		angle = (angle + 180) / 45;
		angle = Mathf.Round(angle);
		if (angle == 8)
		{
			angle = 0;
		}
		animator.SetFloat("Direction", angle);
		
		transform.LookAt(MainCamera.Camera.transform, Vector3.up);
		transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);

		animator.SetFloat("Speed", agent.velocity.magnitude * speedScalar);
	}
}
