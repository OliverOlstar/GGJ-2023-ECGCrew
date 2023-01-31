using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;
using Sirenix.OdinInspector;

public class FirstPersonCameraMovement : MonoBehaviour
{
	[SerializeField]
	private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.PreCamera);
	[SerializeField]
	private Transform positionTransform = null;
	[SerializeField]
	private Transform bobbingTransform = null;

	[Header("Spring")]
	[SerializeField, Range(0, 50)]
	private float sprintSpring = 45.0f;
	[SerializeField, Range(0, 50)]
	private float springDamper = 10.0f;

	[Header("Crouch")]
	[SerializeField]
	private float crouchHeight = -1.0f;

	private float defaultHeight = 0.0f;
	private float targetHeight = 0.0f;

	private float heightVelocity = 0.0f;

	void OnEnable()
	{
		updateable.Register(Tick);
	}

	void OnDisable()
	{
		updateable.Deregister();
	}

	void Start()
	{
		defaultHeight = positionTransform.localPosition.y;
		targetHeight = defaultHeight;
	}

	private void Tick(float pDeltaTime)
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

	public void SetCrouching(bool pCrouch)
	{
		targetHeight = pCrouch ? crouchHeight : defaultHeight;
	}
}
