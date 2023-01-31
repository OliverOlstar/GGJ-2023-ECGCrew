using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class FirstPersonCharacterController : MonoBehaviour
{
	[Header("Components")]
	public InputBridge_FirstPersonPlayer Input = null;
	public CharacterController Character = null;
	public MonoUtil.Updateable Updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Fixed, MonoUtil.Priorities.CharacterController);

	[Header("Stats")]
	[SerializeField, Min(0.0f)]
	private float moveSpeed = 5;
	[SerializeField, Min(0.0f)]
	private float drag = 2;

	private Vector3 velocity = Vector3.zero;

	public Transform ForwardTransform => MainCamera.Camera != null ? MainCamera.Camera.transform : Character.transform;

	private void Reset()
	{
		Input = GetComponentInChildren<InputBridge_FirstPersonPlayer>();
		Character = GetComponentInChildren<CharacterController>();
	}

	void OnEnable()
	{
		Updateable.Register(Tick);
	}
	void OnDisable()
	{
		Updateable.Deregister();
	}

    void Tick(float pDeltaTime)
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
			velocity += move * pDeltaTime * moveSpeed;
		}
	}
}
