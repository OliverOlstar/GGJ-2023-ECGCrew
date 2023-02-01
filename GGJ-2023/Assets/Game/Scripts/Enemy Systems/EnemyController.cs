using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterState
{
	NONE = 0,
	PATROL,
	INVESTIGATE,
	CHASE,
	SEARCH
}

public interface IFSM
{
	CharacterState CharacterState { get; }
	void OnStateTransition(CharacterState state);
}

public class EnemyController : MonoBehaviour, IFSM
{
	[SerializeField]
	private Waypoint[] originalWaypoints = null;
	[SerializeField]
	private SoundDetector soundDetector = null;
	[SerializeField]
	private NavMeshAgent agent = null;

	private FirstPersonCharacterController player = null;

	private CharacterState characterState = CharacterState.NONE;
	CharacterState IFSM.CharacterState => characterState;

	private int currentWaypointIndex = 0;
	private Waypoint currentWaypoint = null;

	private void OnEnable()
	{
		SoundDetector.OnPlayerDetected += OnPlayerDetectedHandler;
		Reset();
		SetupWayPoints();
		if (player != null)
		{
			ChasePlayer();
		}
		else
		{
			StartCoroutine(MoveNextWaypoint());
		}
	}

	private void OnDisable() => SoundDetector.OnPlayerDetected -= OnPlayerDetectedHandler;

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Waypoint waypoint))
		{
			// Check if this Waypoint is our current destination
			for (int i = 0; i < originalWaypoints.Length; i++)
			{
				if (currentWaypointIndex == i)
				{
					currentWaypoint = waypoint;
					OnWayPointReached();
					break;
				}
			}
		}
	}

	private void SetupWayPoints()
	{
		for (int i = 0; i < originalWaypoints.Length; i++)
		{
			int nextWaypointIndex = MathUtil.LoopedValue(i++, 0, originalWaypoints.Length - 1);
			int previousWaypointIndex = MathUtil.LoopedValue(i--, 0, originalWaypoints.Length - 1);
			originalWaypoints[i].Setup(originalWaypoints[nextWaypointIndex], originalWaypoints[previousWaypointIndex]);
		}
	}

	private void OnPlayerDetectedHandler(FirstPersonCharacterController playerController)
	{
		player = playerController;
	}

	private void ChasePlayer()
	{

	}

	private IEnumerator MoveNextWaypoint()
	{
		float minWaitTime = 1.0f;
		float maxWaitTime = 2.0f;
		float randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
		yield return new WaitForSeconds(randomWaitTime);
		agent.SetDestination(originalWaypoints[currentWaypointIndex].NextWaypoint.transform.position);
	}

	private void OnWayPointReached()
	{
		if (characterState == CharacterState.PATROL && player == null)
		{
			StartCoroutine(MoveNextWaypoint());
			currentWaypointIndex++;
			currentWaypointIndex = MathUtil.LoopedValue(currentWaypointIndex, 0, originalWaypoints.Length - 1);
		}
	}

	void IFSM.OnStateTransition(CharacterState state)
	{
		characterState = state;
	}

	private void Reset()
	{
		currentWaypointIndex = 0;
		currentWaypoint = null;
		characterState = CharacterState.PATROL;
	}
}