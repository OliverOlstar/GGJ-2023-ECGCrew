using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using OliverLoescher;

public class FirstPersonCamera : MonoUtil.MonoBehaviour2
{
	[SerializeField]
	private Transform cameraTransform = null;

	[Space, SerializeField, MinMaxSlider(-90, 90, true)]
	private Vector2 cameraYClamp = new Vector2(-40, 50);
	[SerializeField, MinMaxSlider(-180, 180, true)]
	private Vector2 cameraXClamp = new Vector2(-180, 180);

	private Vector2 input = Vector2.zero;

	protected override MonoUtil.UpdateType UpdateType => MonoUtil.UpdateType.Late;
	protected override float UpdatePriority => MonoUtil.Priorities.Camera;

	private void OnEnabled()
	{
		InputBridge_FirstPersonPlayer.Instance.Look.onChanged.AddListener(SetCameraMove);
		InputBridge_FirstPersonPlayer.Instance.LookDelta.onChanged.AddListener(OnCameraMove);
	}
	private void OnDisabled()
	{
		if (InputBridge_FirstPersonPlayer.Instance == null)
		{
			return;
		}
		InputBridge_FirstPersonPlayer.Instance.Look.onChanged.RemoveListener(SetCameraMove);
		InputBridge_FirstPersonPlayer.Instance.LookDelta.onChanged.RemoveListener(OnCameraMove);
	}

	public void SetCameraMove(Vector2 pInput) => input = pInput;
	public void OnCameraMove(Vector2 pInput) => RotateCamera(pInput);

	private void RotateCamera(Vector2 pInput)
	{
		Vector3 euler = cameraTransform.localEulerAngles;
		euler.x = Mathf.Clamp(Util.SafeAngle(euler.x - pInput.y), cameraYClamp.x, cameraYClamp.y);
		
		if (cameraXClamp.x == -180 && cameraXClamp.y == 180)
		{
			euler.y += pInput.x;
		}
		else
		{
			if (pInput.x < 0)
			{
				euler.y = Mathf.Max(cameraXClamp.x, Util.SafeAngle(euler.y) + pInput.x);
			}
			else
			{
				euler.y = Mathf.Min(cameraXClamp.y, Util.SafeAngle(euler.y) + pInput.x);
			}
		}
		euler.z = 0.0f;
		cameraTransform.localRotation = Quaternion.Euler(euler);
	}

	protected override void Tick(float pDeltaTime)
	{
		RotateCamera(input * pDeltaTime);
	}
}