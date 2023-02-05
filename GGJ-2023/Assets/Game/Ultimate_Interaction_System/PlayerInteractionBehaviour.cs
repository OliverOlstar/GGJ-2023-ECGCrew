using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class PlayerInteractionBehaviour : MonoUtil.MonoBehaviour
{
	[SerializeField]
	private InputBridge_FirstPersonPlayer input = null;
	[SerializeField]
	private GameObject toggleObject = null;

	private Vector3 Position => MainCamera.Camera.transform.position;
	private Vector3 Forward => MainCamera.Camera.transform.forward;
	[Header("Raycast"), SerializeField]
	private float distance = 1.0f;
	[SerializeField]
	private LayerMask lineCastLayerMask = new LayerMask();

	private Tuple<Collider, IPlayerInteractable> interactable = null;
	private bool selected = false;

	void Start()
	{
		input.Interact.onPerformed.AddListener(OnInteractPressed);
	}

	protected override void Tick(float pDeltaTime)
	{
		if (selected)
		{
			return;
		}
		Vector3 posB = Position + Util.Horizontalize(Forward) * distance;
		if (Physics.Linecast(Position, posB, out RaycastHit hit, lineCastLayerMask))
		{
			SwitchInteractable(hit.collider);
		}
		else
		{
			SwitchInteractable(null);
		}
	}

	private void SwitchInteractable(Collider pCollider)
	{
		if (interactable != null)
		{
			if (pCollider == interactable.Item1)
			{
				return; // Already set to it
			}
			interactable.Item2.UnHover();
		}
		if (pCollider == null || !pCollider.TryGetComponent(out IPlayerInteractable interact))
		{
			interactable = null;
			toggleObject.SetActive(false);
			return;
		}
		interactable = new Tuple<Collider, IPlayerInteractable>(pCollider, interact);
		interact.Hover();
		toggleObject.SetActive(true);
	}

	private void OnInteractPressed()
	{
		if (interactable != null)
		{
			if (selected)
			{
				interactable.Item2.UnSelect();
				selected = false;
			}
			else
			{
				interactable.Item2.Select();
				if (interactable.Item2.IsSelectable)
				{
					selected = true;
					toggleObject.SetActive(false);
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		if (MainCamera.Camera != null)
		{
			Gizmos.color = Color.cyan;
			Vector3 posB = Position + Util.Horizontalize(Forward) * distance;
			Gizmos.DrawLine(Position, posB);
		}
	}
}
