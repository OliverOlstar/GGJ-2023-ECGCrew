using UnityEngine;

public class Waypoint : MonoBehaviour
{
	private Waypoint nextWaypoint = null;
	public Waypoint NextWaypoint => nextWaypoint;
	private Waypoint previousWaypoint = null;
	public Waypoint PreviousWaypoint => previousWaypoint;

	private void Start()
	{
		// Waypoints are attached to Enemies so need to unparent them
		transform.SetParent(null);
	}

	public void Setup(Waypoint nextWaypoint, Waypoint previousWaypoint)
	{
		this.nextWaypoint = nextWaypoint;
		this.previousWaypoint = previousWaypoint;
	}
}