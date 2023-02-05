using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider))]
public class PlayerHiddingSpot : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
	private new CinemachineVirtualCamera camera = null;

	bool IPlayerInteractable.IsSelectable => true;

	void IPlayerInteractable.Hover()
	{
		
	}

	void IPlayerInteractable.UnHover()
	{
		
	}

	void IPlayerInteractable.Select()
	{
		camera.gameObject.SetActive(true);
	}

	void IPlayerInteractable.UnSelect()
	{
		camera.gameObject.SetActive(false);
	}
}
