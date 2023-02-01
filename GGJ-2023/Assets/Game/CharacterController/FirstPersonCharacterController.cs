using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class FirstPersonCharacterController : MonoUtil.MonoBehaviour2
{
	[Header("Components")]
	public InputBridge_FirstPersonPlayer Input = null;
	public CharacterController Character = null;
	public FirstPersonCrouchBehaviour Crouch = null;

	[Header("Stats")]
	[SerializeField, Min(0.0f)]
	private float moveSpeed = 5;
	[SerializeField, Min(0.0f)]
	private float moveDrag = 2;

	[SerializeField, Min(0.0f)]
	private float sprintSpeed = 15;
	[SerializeField, Min(0.0f)]
	private float sprintDrag = 5;
	
	[SerializeField, Min(0.0f)]
	private float crouchSpeed = 5;
	[SerializeField, Min(0.0f)]
	private float crouchDrag = 3;

	private float speed => !Input.Crouch.Input ? !Input.Sprint.Input ? moveSpeed : sprintSpeed : (crouchSpeed * (Crouch.CrouchPercent * 0.75f + 0.25f));
	private float drag => !Input.Crouch.Input ? !Input.Sprint.Input ? moveDrag : sprintDrag : (crouchDrag);

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
        AddMovement(pDeltaTime);
		velocity -= velocity * drag * pDeltaTime;
		Character.SimpleMove(velocity * pDeltaTime);
    }

	private void AddMovement(float pDeltaTime)
	{
		Vector2 input = Input.Move.Input;
		if (input.sqrMagnitude > 0.15f)
		{
			Vector3 move = Util.Horizontalize(ForwardTransform.forward) * input.y;
			move += Util.Horizontalize(ForwardTransform.right) * input.x;
			velocity += move * pDeltaTime * speed;
		}
	}
}
