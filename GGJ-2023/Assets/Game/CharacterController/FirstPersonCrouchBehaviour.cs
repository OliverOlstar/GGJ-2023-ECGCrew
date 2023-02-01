using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;
using Sirenix.OdinInspector;

public class FirstPersonCrouchBehaviour : MonoUtil.MonoBehaviour2
{
	[Header("Components"), SerializeField]
	private InputBridge_FirstPersonPlayer input = null;
	[SerializeField]
	private new FirstPersonCameraMovement camera = null;
	[SerializeField]
	private CharacterController character = null;
	[SerializeField]
	private FirstPersonCharacterController controller = null;

	[Header("Values"), SerializeField]
	private float crouchCharacterHeight = 1.2f;
	[SerializeField, MinMaxSlider(0.0f, 2.0f, true)]
	private Vector2 crouchCameraHeight = new Vector2(-1.6f, -0.8f);
	[SerializeField, Min(0.00001f)]
	private float crouchCameraRadius = 0.1f;
	[SerializeField]
	private LayerMask crouchCameraLayerMask = new LayerMask();

	private float defaultCharacterHeight;
	private float defaultCameraHeight;

	public float CrouchPercent
	{
		get
		{
			float v = Util.SmoothStep(crouchCameraHeight.x, crouchCameraHeight.y, camera.TargetHeight);
			Debug.Log(v);
			return v;
		}
	}

	protected override MonoUtil.UpdateType UpdateType => MonoUtil.UpdateType.Fixed;
	protected override float UpdatePriority => MonoUtil.Priorities.PreCamera;

	private void Reset()
	{
		input = GetComponentInChildren<InputBridge_FirstPersonPlayer>();
		camera = GetComponentInChildren<FirstPersonCameraMovement>();
		character = GetComponentInChildren<CharacterController>();
		controller = GetComponentInChildren<FirstPersonCharacterController>();
	}

	void Start()
	{
		defaultCharacterHeight = character.height;
		
		input.Crouch.onPerformed.AddListener(SetCrouched);
		input.Crouch.onCanceled.AddListener(ClearCrouched);
		input.Sprint.onPerformed.AddListener(ClearCrouched);
	}

	private void SetCrouched()
	{
		character.height = crouchCharacterHeight;
		character.center = Vector3.up * crouchCharacterHeight * 0.5f;
	}

	private void ClearCrouched()
	{
		if (!CanCancelCrounch())
		{
			input.Crouch.SetInput(true);
			return;
		}
		input.Crouch.SetInput(false);
		
		camera.ResetHeight();
		character.height = defaultCharacterHeight;
		character.center = Vector3.up * defaultCharacterHeight * 0.5f;
	}

	protected override void Tick(float pDeltaTime)
	{
		if (!input.Crouch.Input)
		{
			return; // Not crouching
		}

		float y = crouchCameraHeight.y;
		Transform t = character.transform;
		Vector3 posA = t.position + new Vector3(0f, crouchCameraHeight.x + crouchCameraRadius, 0f);
		Vector3 posB = t.position + new Vector3(0f, crouchCameraHeight.y + crouchCameraRadius, 0f);
		if (Physics.Linecast(posA, posB, out RaycastHit hit, crouchCameraLayerMask))
		{
			y = t.InverseTransformPoint(hit.point).y - crouchCameraRadius;
		}
		if (controller.Velocity.sqrMagnitude > Util.NEARZERO)
		{
			Vector3 offset = Util.Horizontal(controller.Velocity) * 0.0065f;
			posA += offset;
			posB += offset;
			if (Physics.Linecast(posA, posB, out hit, crouchCameraLayerMask))
			{
				y = Mathf.Min(y, t.InverseTransformPoint(hit.point).y - crouchCameraRadius);
			}
		}
		camera.SetHeight(y);
	}

	private bool CanCancelCrounch()
	{
        Vector3 p1 = character.transform.position;
        Vector3 p2 = p1 + Vector3.up * defaultCharacterHeight;
		return !Physics.Linecast(p1, p2, crouchCameraLayerMask);
	}

	private void OnDrawGizmosSelected()
	{
		Transform t = character.transform;
		Vector3 posA = t.position + new Vector3(0f, crouchCameraHeight.x, 0f);
		Vector3 posB = t.position + new Vector3(0f, crouchCameraHeight.y, 0f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(posA, posB);

		if (controller != null && controller.Velocity.sqrMagnitude > Util.NEARZERO)
		{
			Vector3 offset = Util.Horizontal(controller.Velocity) * 0.0065f;
			posA += offset;
			posB += offset;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(posA, posB);
		}
	}
}