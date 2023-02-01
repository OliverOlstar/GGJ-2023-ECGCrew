using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;
using Sirenix.OdinInspector;

public class FirstPersonCameraMovement : MonoUtil.MonoBehaviour2
{
	[SerializeField]
	private Transform positionTransform = null;

	[Header("Spring"), SerializeField, Range(0, 90)]
	private float sprintSpring = 45.0f;
	[SerializeField, Range(0, 50)]
	private float springDamper = 10.0f;

	private float defaultHeight = 0.0f;
	private float targetHeight = 0.0f;
	public float TargetHeight => targetHeight;

	protected override MonoUtil.UpdateType UpdateType => MonoUtil.UpdateType.Late;
	protected override float UpdatePriority => MonoUtil.Priorities.PreCamera;

	private float heightVelocity = 0.0f;

	void Start()
	{
		defaultHeight = positionTransform.localPosition.y;
		targetHeight = defaultHeight;
	}

	protected override void Tick(float pDeltaTime)
	{
		DoSpring(positionTransform.localPosition.y, targetHeight, pDeltaTime);
	}

	private void DoSpring(float pY, float pTargetY, float pDeltaTime)
	{
		heightVelocity += sprintSpring * -(pY - pTargetY) * pDeltaTime;
		heightVelocity += springDamper * -heightVelocity * pDeltaTime;
		pY += heightVelocity * pDeltaTime;
		positionTransform.localPosition = pY * Vector3.up;
	}

	public void SetHeight(float pHeight) => targetHeight = pHeight;
	public void ResetHeight() => targetHeight = defaultHeight;
}
