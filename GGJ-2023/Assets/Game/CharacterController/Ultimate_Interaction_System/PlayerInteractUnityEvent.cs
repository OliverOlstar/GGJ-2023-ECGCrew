using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteractUnityEvent : MonoBehaviour, IPlayerInteractable
{
	public UnityEngine.Events.UnityEvent onHover = new UnityEngine.Events.UnityEvent();
	public UnityEngine.Events.UnityEvent onUnHover = new UnityEngine.Events.UnityEvent();
	public UnityEngine.Events.UnityEvent onSelect = new UnityEngine.Events.UnityEvent();
	public UnityEngine.Events.UnityEvent onUnSelect = new UnityEngine.Events.UnityEvent();

	public bool IsSelectable => true;

	void IPlayerInteractable.Hover() => onHover?.Invoke();
	void IPlayerInteractable.UnHover() => onUnHover?.Invoke();
	void IPlayerInteractable.Select() => onSelect?.Invoke();
	void IPlayerInteractable.UnSelect() => onUnSelect?.Invoke();
}
