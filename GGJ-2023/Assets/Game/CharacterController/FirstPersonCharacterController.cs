using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public enum PlayerMovementState
{
	NONE = -1,
	HIDING = 0,
	CROUCHING = 2,
	WALKING = 5,
	RUNNING = 10,
}

public class FirstPersonCharacterController : MonoUtil.MonoBehaviour2
{
	[Header("Components")]
	public InputBridge_FirstPersonPlayer Input = null;
	public CharacterController Character = null;
	public FirstPersonCrouchBehaviour Crouch = null;
	public FirstPersonCharacterStamina Stamina = null;

	[Header("Stats")]
	[SerializeField, Min(0.0f)]
	private float moveSpeed = 5;
	[SerializeField, Min(0.0f)]
	private float moveDrag = 2;

	[Space, SerializeField, Min(0.0f)]
	private float sprintSpeed = 15;
	[SerializeField, Min(0.0f)]
	private float sprintDrag = 5;
	[SerializeField]
	private float staminaPerSecond = 1;
	[SerializeField]
	private float staminaOutSpeedScalar = 0.7f;
	
	[Space, SerializeField, Min(0.0f)]
	private float crouchSpeed = 5;
	[SerializeField, Min(0.0f)]
	private float crouchDrag = 3;

	private bool IsSprinting => !Stamina.isOut && Input.Sprint.Input;
	private bool sprintInput = false;
	private float speed => (!Input.Crouch.Input ? !IsSprinting ? moveSpeed : sprintSpeed : (crouchSpeed * (Crouch.CrouchPercent * 0.5f + 0.5f))) * StaminaSpeedScalar();
	private float drag => !Input.Crouch.Input ? !IsSprinting ? moveDrag : sprintDrag : crouchDrag;

	private float StaminaSpeedScalar()
	{
		if (!IsSprinting && (Stamina.isOut || Stamina.Get() < 0.25f))
		{
			return staminaOutSpeedScalar;
		}
		return 1.0f;
	}

	private Vector3 velocity = Vector3.zero;
	public Vector3 Velocity => velocity;

	public Transform ForwardTransform => MainCamera.Camera != null ? MainCamera.Camera.transform : Character.transform;

	protected override MonoUtil.UpdateType UpdateType => MonoUtil.UpdateType.Fixed;
	protected override float UpdatePriority => MonoUtil.Priorities.CharacterController;

	private void Reset()
	{
		Input = GetComponentInChildren<InputBridge_FirstPersonPlayer>();
		Character = GetComponentInChildren<CharacterController>();
	}

    protected override void Tick(float pDeltaTime)
    {
		if (Input.Crouch.Input)
		{
			if (Stamina.Get() > 0.25f)
			{
				sprintInput = true;
			}
		}
		else
		{
			sprintInput = false;
		}

        if (AddMovement(pDeltaTime) && IsSprinting)
		{
			Stamina.Modify(pDeltaTime * -staminaPerSecond);
		}
		velocity -= velocity * drag * pDeltaTime;
		Character.SimpleMove(velocity * pDeltaTime);
    }

	private bool AddMovement(float pDeltaTime)
	{
		Vector2 input = Input.Move.Input;
		if (input.sqrMagnitude > 0.15f)
		{
			Vector3 move = Util.Horizontalize(ForwardTransform.forward) * input.y;
			move += Util.Horizontalize(ForwardTransform.right) * input.x;
			velocity += move * pDeltaTime * speed;
			return true;
		}
		return false;
	}
}
