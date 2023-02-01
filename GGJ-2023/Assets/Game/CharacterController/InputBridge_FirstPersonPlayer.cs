using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher.Input;
using OliverLoescher;

public class InputBridge_FirstPersonPlayer : InputBridge_Base
{
	public static InputBridge_FirstPersonPlayer Instance { get; private set; }

	[SerializeField]
	private InputModule_Vector2 lookInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Vector2 lookDeltaInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Vector2 moveInput = new InputModule_Vector2();
	[SerializeField]
	private InputModule_Toggle crouchInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Toggle sprintInput = new InputModule_Toggle();
	[SerializeField]
	private InputModule_Trigger jumpInput = new InputModule_Trigger();
	[SerializeField]
	private InputModule_Trigger interactInput = new InputModule_Trigger();

	public InputModule_Vector2 Look => lookInput;
	public InputModule_Vector2 LookDelta => lookDeltaInput;
	public InputModule_Vector2 Move => moveInput;
	public InputModule_Toggle Crouch => crouchInput;
	public InputModule_Toggle Sprint => sprintInput;
	public InputModule_Trigger Jump => jumpInput;
	public InputModule_Trigger Interact => interactInput;

	public override UnityEngine.InputSystem.InputActionMap Actions => InputSystem.Instance.FirstPersonController.Get();
	public override IEnumerable<IInputModule> GetAllInputModules()
	{
		yield return lookInput;
		yield return lookDeltaInput;
		yield return moveInput;
		yield return crouchInput;
		yield return sprintInput;
		yield return jumpInput;
		yield return interactInput;
	}

	protected override void Awake()
	{
		Instance = this;

		lookInput.Initalize(InputSystem.Instance.FirstPersonController.CameraMove, IsValid);
		lookDeltaInput.Initalize(InputSystem.Instance.FirstPersonController.CameraMoveDelta, IsValid);
		moveInput.Initalize(InputSystem.Instance.FirstPersonController.Move, IsValid);
		crouchInput.Initalize(InputSystem.Instance.FirstPersonController.Crouch, IsValid);
		sprintInput.Initalize(InputSystem.Instance.FirstPersonController.Sprint, IsValid);
		jumpInput.Initalize(InputSystem.Instance.FirstPersonController.Jump, IsValid);
		interactInput.Initalize(InputSystem.Instance.FirstPersonController.Interact, IsValid);

		base.Awake();
	}

	protected override void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;

		base.OnEnable();
	}

	protected override void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;

		base.OnDisable();
	}
}