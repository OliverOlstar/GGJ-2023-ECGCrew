using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyPatrolState
{
	NONE = 0,
	PATROL,
	INVESTIGATE,
	CHASE,
	SEARCH
}

public interface IFSM
{
	EnemyPatrolState CharacterState { get; }
	void OnStateTransition(EnemyPatrolState state);
}

public class EnemyController : MonoBehaviour, IFSM
{
	[Header("References")]
	[SerializeField]
	private Waypoint[] originalWaypoints = null;
	[SerializeField]
	private SoundDetector soundDetector = null;
	[SerializeField]
	private NavMeshAgent agent = null;
	[SerializeField]
	private Transform target = null;
	[SerializeField]
	private GameObject model = null;

	[Header("Settings")]
	[SerializeField]
	private float viewRadius = 15;
	[SerializeField]
	private float viewAngle = 90f;
	[SerializeField]
	private LayerMask playerLayer;
	[SerializeField]
	private LayerMask obstacleLayer;
	[SerializeField]
	private float walkSpeed = 5f;
	[SerializeField]
	private float chaseSpeed = 10f;

	private FirstPersonCharacterController player = null;

	private EnemyPatrolState characterState = EnemyPatrolState.NONE;
	EnemyPatrolState IFSM.CharacterState => characterState;

	private int currentWaypointIndex = 0;
	private Waypoint currentWaypoint = null;

	private void OnEnable()
	{
		SoundDetector.OnPlayerDetected += OnPlayerDetectedHandler;
		Reset();
		SetupWayPoints();
		if (player != null)
		{
			OnPlayerDetectedHandler(player);
		}
		else
		{
			StartCoroutine(MoveNextWaypoint());
		}
	}

	private void OnDisable() => SoundDetector.OnPlayerDetected -= OnPlayerDetectedHandler;

	private void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.TryGetComponent(out FirstPersonCharacterController firstPersonCharacter);
		if (firstPersonCharacter)
		{
			OnPlayerDetectedHandler(firstPersonCharacter);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Waypoint waypoint))
		{
			// Check if this Waypoint is our current destination
			for (int i = 0; i < originalWaypoints.Length; i++)
			{
				if (currentWaypointIndex == i)
				{
					OnWayPointReached();
					break;
				}
			}
		}
	}

	private void Update()
	{
		if (target != null)
		{
			transform.LookAt(target, Vector3.back);
		}
	}

	private void FixedUpdate()
	{
		RaycastHit hit;
		float raycastDistance = 20f;
		float sphereRadius = 5f;
		// Does the ray intersect any objects excluding the player layer
		Debug.DrawRay(transform.position, Vector3.forward * raycastDistance, Color.red);
		if (Physics.SphereCast(transform.position, sphereRadius, -Vector3.forward, out hit, raycastDistance))
		{
			hit.transform.gameObject.TryGetComponent(out FirstPersonCharacterController fpsController);
			if (fpsController)
			{
				Log($"Hit {fpsController}");
			}
		}
	}

	private void EnableCondition()
	{
		// Check if this Enemy should become "Active"
		// Used for disabling Enemies in rooms for "Jump Scares"
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
		IFSM stateMachine = this;
		stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
	}

	private void Move(float speed = 0.5f)
	{
		agent.isStopped = false;
		agent.speed = speed;
	}

	private void Patrol()
	{
		target = currentWaypoint.transform;
	}

	private void Stop()
	{
		agent.isStopped = true;
		agent.speed = 0f;
		target = null;
	}

	private IEnumerator MoveNextWaypoint()
	{
		float minWaitTime = 1.0f;
		float maxWaitTime = 2.0f;
		float randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
		yield return new WaitForSeconds(randomWaitTime);

		agent.isStopped = true;
		yield return null;
		RotateTowardsTarget();
		agent.isStopped = false;

		agent.SetDestination(target.position);
	}

	private void RotateTowardsTarget()
	{
		Vector3 turnTowardNavSteeringTarget = agent.steeringTarget;

		Vector3 rotDirection = (turnTowardNavSteeringTarget - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(rotDirection.x, 0, rotDirection.z));
		float rotateTime = 5f;
		model.transform.rotation = Quaternion.Slerp(model.transform.rotation, lookRotation, Time.deltaTime * rotateTime);
	}

	private void OnWayPointReached()
	{
		if (characterState == EnemyPatrolState.PATROL && player == null)
		{
			currentWaypointIndex++;
			currentWaypointIndex = MathUtil.LoopedValue(currentWaypointIndex, 0, originalWaypoints.Length - 1);
			currentWaypoint = originalWaypoints[currentWaypointIndex];
			target = currentWaypoint.transform;
			StartCoroutine(MoveNextWaypoint());
		}
	}

	void IFSM.OnStateTransition(EnemyPatrolState state)
	{
		characterState = state;

		switch (characterState)
		{
			case EnemyPatrolState.PATROL:
				Log("Patrolling...");
				break;
			case EnemyPatrolState.SEARCH:
				Log("Searching...");
				break;
			case EnemyPatrolState.CHASE:
				Log("Chasing player!");
				break;
			default:
				break;
		}
	}

	private void Reset()
	{
		currentWaypointIndex = 0;
		currentWaypoint = null;
		target = originalWaypoints[currentWaypointIndex].transform;
		agent.updateRotation = true;
		agent.isStopped = true;
		IFSM fsm = this;
		fsm.OnStateTransition(EnemyPatrolState.PATROL);
	}

	private void Log(string log)
	{
		Debug.Log($"|{this}| {log}");
	}

	private void OnDrawGizmos()
	{
		#region Search State Gizmos
		// Draw a yellow sphere at the transform's position
		Color enemyGizmoColor = Color.green;
		switch (characterState)
		{
			case EnemyPatrolState.PATROL:
				break;
			case EnemyPatrolState.SEARCH:
				enemyGizmoColor = Color.yellow;
				break;
			case EnemyPatrolState.CHASE:
				enemyGizmoColor = Color.red;
				break;
			default:
				break;
		}

		Gizmos.color = enemyGizmoColor;
		float sphereRadius = 1f;
		Gizmos.DrawWireSphere(transform.position, sphereRadius);
		#endregion

		/*
		#region Raycast Gizmos
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.forward);
		#endregion
		*/
	}
}