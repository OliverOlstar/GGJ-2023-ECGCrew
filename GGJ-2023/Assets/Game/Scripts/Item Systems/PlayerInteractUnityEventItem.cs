using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteractUnityEventItem : PlayerInteractUnityEvent
{
	public UnityEngine.Events.UnityEvent<Item> onItemSelect = new UnityEngine.Events.UnityEvent<Item>();
}