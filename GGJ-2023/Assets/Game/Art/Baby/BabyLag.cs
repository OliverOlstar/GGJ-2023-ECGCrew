using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class BabyLag : MonoUtil.MonoBehaviour
{
	// TODO: Watch motion -> Fall behind, plant falls further
	[SerializeField]
	private Transform positionTransform = null;
	[SerializeField]
	private Transform positionTransform2 = null;
	[SerializeField]
	private Transform rotationTransform = null;
	private Vector3 prevPosition = Vector3.zero;
	private Vector3 prevPosition2 = Vector3.zero;

	[SerializeField]
	private Transform babyTransform = null;
	[SerializeField]
	private float mainMagnitude = 0.18f;
	[SerializeField]
	private float mainDampening = 5.0f;
	[SerializeField]
	private float babyMagnitude = 0.18f;
	[SerializeField]
	private float babyDampening = 5.0f;

	[Header("Tilt")]
	[SerializeField] private Vector2 tiltMagnitude = Vector2.one;
	[SerializeField, Min(0)] private Vector2 tiltDampening = Vector2.one;
	[SerializeField] private Vector2 babyTiltMagnitude = Vector2.one;
	[SerializeField, Min(0)] private Vector2 babyTiltDampening = Vector2.one;

	private Vector3 babyInitialPosition = Vector3.zero;
	private Vector3 initialPosition = Vector3.zero;
	private Vector3 lastRotation = Vector3.zero;

	void Start()
	{
		babyInitialPosition = babyTransform.localPosition;
		initialPosition = transform.localPosition;
		prevPosition = positionTransform.localPosition;
		prevPosition2 = positionTransform2.localPosition;
		lastRotation = rotationTransform.eulerAngles;
	}

	protected override void Tick(float pDeltaTime)
	{
		float translation = positionTransform.localPosition.y - prevPosition.y;
		prevPosition = positionTransform.localPosition;
		translation += positionTransform2.localPosition.y - prevPosition2.y;
		prevPosition2 = positionTransform2.localPosition;

		// Vector3 rotTranslation = rotationTransform.eulerAngles - lastRotation;
		float xRot = GetRotationTranslation(lastRotation.x, rotationTransform.eulerAngles.x);
		float yRot = GetRotationTranslation(lastRotation.y, rotationTransform.eulerAngles.y);
		lastRotation = rotationTransform.eulerAngles;

		Vector3 motion = Vector3.zero;
		motion.x += Mathf.Lerp(0.0f, yRot * tiltMagnitude.x, tiltDampening.x * pDeltaTime);
		motion.y += Mathf.Lerp(0.0f, xRot * tiltMagnitude.y, tiltDampening.y * pDeltaTime);

		motion.y += -translation * mainMagnitude;
		transform.localPosition = Vector3.Lerp(transform.localPosition + motion, initialPosition, pDeltaTime * mainDampening);

		motion = Vector3.zero;
		motion.x += Mathf.Lerp(0.0f, yRot * babyTiltMagnitude.x, babyTiltDampening.x * pDeltaTime);
		motion.y += Mathf.Lerp(0.0f, xRot * babyTiltMagnitude.y, babyTiltDampening.y * pDeltaTime);

		motion.y += -translation * babyMagnitude;
		babyTransform.localPosition = Vector3.Lerp(babyTransform.localPosition + motion, babyInitialPosition, pDeltaTime * babyDampening);
	}
	
	private float GetRotationTranslation(float pPrev, float pCurr)
	{
		float translation = pCurr - pPrev;
		if (translation > 180f)
		{
			translation -= 360f;
		}
		else if (translation < -180f)
		{
			translation += 360f;
		}
		return translation;
	}
}
