using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;
using Sirenix.OdinInspector;

public class FirstPersonCameraMovement : MonoUtil.MonoBehaviour2
{
	[SerializeField]
	private Transform positionTransform = null;
	[SerializeField]
	private Transform bobbingTransform = null;
	[SerializeField]
	private FirstPersonCharacterController controller = null;
	[SerializeField]
	private OnGround grounded = null;

	[Header("Spring"), SerializeField, Range(0, 90)]
	private float sprintSpring = 45.0f;
	[SerializeField, Range(0, 50)]
	private float springDamper = 10.0f;
	[SerializeField]
	private float landingForce = 1.0f;

	[Header("Bobbing"), SerializeField]
	private AnimationCurve bobbingMotionCurve = new AnimationCurve();
	[SerializeField]
	private AnimationCurve bobbingMagnitudeCurve = new AnimationCurve();
	[SerializeField]
	private AnimationCurve bobbingCrouchMagitudeCurve = new AnimationCurve();
	[SerializeField]
	private AnimationCurve bobbingFrequencyCurve = new AnimationCurve();
	[SerializeField]
	private float bobDampening = 1.0f;
	[SerializeField]
	private float velocityMaxSpeed = 10.0f;

	[Space, FoldoutGroup("Events"), SerializeField]
	private UnityEngine.Events.UnityEvent onStep = new UnityEngine.Events.UnityEvent();
	[FoldoutGroup("Events"), SerializeField]
	private UnityEngine.Events.UnityEvent onCrouchStep = new UnityEngine.Events.UnityEvent();
	[FoldoutGroup("Events"), SerializeField]
	private OliverLoescher.UnityEventsUtil.FloatEvent onLanded = new OliverLoescher.UnityEventsUtil.FloatEvent();

	private float defaultHeight = 0.0f;
	private float targetHeight = 0.0f;
	public float TargetHeight => targetHeight;

	private float bobTime = 0.0f;
	private float bobMagnitude = 0.0f;

	protected override MonoUtil.UpdateType UpdateType => MonoUtil.UpdateType.Late;
	protected override float UpdatePriority => MonoUtil.Priorities.PreCamera;

	private float heightVelocity = 0.0f;

	void Start()
	{
		defaultHeight = positionTransform.localPosition.y;
		targetHeight = defaultHeight;

		grounded.OnEnter.AddListener(OnGrounded);
	}

	protected override void Tick(float pDeltaTime)
	{
		DoSpring(positionTransform.localPosition.y, targetHeight, pDeltaTime);
		DoBobbing(pDeltaTime);
	}

	private void OnGrounded()
	{
		heightVelocity += controller.Character.velocity.y * landingForce;
		onLanded?.Invoke(landingForce);
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

	private void DoBobbing(float pDeltaTime)
	{
		float velocity = Util.Horizontal(controller.Velocity).magnitude / velocityMaxSpeed;
		float prevBobTime = bobTime;
		bobTime += pDeltaTime * bobbingFrequencyCurve.Evaluate(velocity);
		CheckForStep(prevBobTime, bobTime);
		while (bobTime >= 1.0f)
		{
			bobTime -= 1.0f;
		}
		float targetMagnitude = 0.0f;
		if (grounded.isGrounded)
		{
			if (controller.Input.Crouch.Input)
			{
				targetMagnitude = bobbingCrouchMagitudeCurve.Evaluate(velocity);
			}
			else
			{
				targetMagnitude = bobbingMagnitudeCurve.Evaluate(velocity);
			}

		}
		bobMagnitude = Mathf.Lerp(bobMagnitude, targetMagnitude, pDeltaTime * bobDampening);
		// Debug.Log($"Mag: {bobMagnitude} - velocity {velocity} - {bobbingFrequencyCurve.Evaluate(velocity)}");
		bobbingTransform.localPosition = Vector3.up * bobbingMotionCurve.Evaluate(bobTime) * bobMagnitude;
	}

	private void CheckForStep(in float pPrev, in float pCurr)
	{
		if (!grounded.isGrounded)
		{
			return;
		}
		if ((pPrev > 0.5f && pCurr <= 0.5f) || (pPrev < 0.5f && pCurr >= 0.5f))
		{
			if (controller.Input.Crouch.Input)
			{
				onCrouchStep?.Invoke();
			}
			else
			{
				onStep?.Invoke();
			}
		}
	}
}
