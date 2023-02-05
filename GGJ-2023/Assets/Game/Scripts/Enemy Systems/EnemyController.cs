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
	private const float MIN_MOVE_SPEED = 1f;
	private const float MAX_MOVE_SPEED = 10f;
	private const int PARANOID_LIMIT = 50;

	private EnemyPatrolState characterState = EnemyPatrolState.NONE;
	EnemyPatrolState IFSM.CharacterState => characterState;

	[Header("References")]
	[SerializeField]
	private Waypoint[] originalWaypoints = null;
	[SerializeField]
	private ViewDetector viewDetector = null;
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
	private float walkSpeed = 5f;
	[SerializeField]
	private float investigateBaseSpeed = 2f;
	[SerializeField]
	private float currentInvestigateSpeed = 1f;
	[SerializeField]
	private float chaseSpeed = 10f;
	[SerializeField]
	private float chaseInterval = 0.33f;
	[SerializeField]
	private float chaseTimer = 0f;
	[SerializeField]
	private float playerChaseDistance = 10f;

	// TODO: Implement "Investigating" where the Enemy walks to the "Noise" spot or "Sprints" to the noise spot, depending on how "Aggresive" the enemy is
	private float currentParanoia = 0f;
	private CharacterController player = null;
	private int currentWaypointIndex = 0;
	private Waypoint currentWaypoint = null;

	private void OnEnable()
	{
		viewDetector.OnPlayerDetected += OnPlayerDetectedHandler;
		viewDetector.OnBecameParanoid += OnBecameParanoidHandler;
		soundDetector.OnSoundDetected += OnSoundDetectedHandler;
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

	private void OnDisable()
	{
		viewDetector.OnPlayerDetected -= OnPlayerDetectedHandler;
		soundDetector.OnSoundDetected -= OnSoundDetectedHandler;
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
		if (characterState == EnemyPatrolState.CHASE && player != null)
		{
			chaseTimer -= Time.deltaTime;
			if (chaseTimer <= 0)
			{
				Chase();
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

	#region States
	void IFSM.OnStateTransition(EnemyPatrolState state)
	{
		characterState = state;

		switch (characterState)
		{
			case EnemyPatrolState.PATROL:
				Log("Patrolling...");
				Patrol();
				break;
			case EnemyPatrolState.INVESTIGATE:
				Log("Investigating...");
				Investigate();
				break;
			case EnemyPatrolState.SEARCH:
				Log("Searching...");
				Search();
				break;
			case EnemyPatrolState.CHASE:
				Log("Chasing player!");
				Chase();
				break;
			default:
				break;
		}
	}

	private void Chase()
	{
		Vector3 playerPosition = player.transform.position;
		if (Vector3.Distance(transform.position, playerPosition) > playerChaseDistance)
		{
			// When losing the player, "Search" for them by creating some new Waypoints around the Enemy so they patrol this new area instead
			player = null;
			IFSM stateMachine = this;
			stateMachine.OnStateTransition(EnemyPatrolState.SEARCH);
			return;
		}
		agent.isStopped = false;
		SetAgentSpeed(chaseSpeed);
		agent.SetDestination(player.transform.position);
		chaseTimer = chaseInterval;
	}

	private void Patrol()
	{
		currentParanoia = 0; 
		currentWaypointIndex++;
		currentWaypointIndex = MathUtil.LoopedValue(currentWaypointIndex, 0, originalWaypoints.Length - 1);
		currentWaypoint = originalWaypoints[currentWaypointIndex];
		target = currentWaypoint.transform;
		agent.isStopped = false;
		SetAgentSpeed(walkSpeed);
		StartCoroutine(MoveNextWaypoint());
	}

	private void Investigate()
	{
		// TODO: Make the Vision Cone "bigger" if investigating
		if (characterState == EnemyPatrolState.INVESTIGATE && player == null)
		{
			ModifyAgentSpeed(currentInvestigateSpeed);
			agent.SetDestination(target.position);
		}
	}

	private void Search()
	{

	}

	private void Stop()
	{
		agent.isStopped = true;
		agent.speed = 0f;
		target = null;
	}
	#endregion

	private void OnPlayerDetectedHandler(CharacterController playerController)
	{
		player = playerController;
		IFSM stateMachine = this;
		stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
	}

	private void OnSoundDetectedHandler(float volume, GameObject detectedObject)
	{
		target = detectedObject.transform;
		IFSM stateMachine = this;
		currentInvestigateSpeed = investigateBaseSpeed * volume;
		stateMachine.OnStateTransition(EnemyPatrolState.INVESTIGATE);
	}

	private void OnBecameParanoidHandler(CharacterController playerController)
	{
		int paranoia = 10;
		currentParanoia += paranoia;

		if (currentParanoia > PARANOID_LIMIT)
		{
			currentParanoia = 0;
			IFSM stateMachine = this;
			player = playerController;
			target = player.transform;
			stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
		}
	}

	private IEnumerator MoveNextWaypoint()
	{
		float minWaitTime = 2.5f;
		float maxWaitTime = 4.0f;
		float randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
		yield return new WaitForSeconds(randomWaitTime);

		agent.isStopped = true;
		yield return null;
		agent.isStopped = false;

		agent.SetDestination(target.position);
	}

	/*
	private void RotateTowardsTarget()
	{
		Vector3 turnTowardNavSteeringTarget = agent.steeringTarget;

		Vector3 rotDirection = (turnTowardNavSteeringTarget - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(rotDirection.x, 0, rotDirection.z));
		float rotateTime = 5f;
		model.transform.rotation = Quaternion.Slerp(model.transform.rotation, lookRotation, Time.deltaTime * rotateTime);
	}
	*/

	private void OnWayPointReached()
	{
		if (characterState == EnemyPatrolState.PATROL && player == null)
		{
			IFSM fsm = this;
			fsm.OnStateTransition(EnemyPatrolState.PATROL);
		}
	}

	private void ModifyAgentSpeed(float modifier)
	{
		agent.speed = MIN_MOVE_SPEED;
		agent.speed = Mathf.Clamp(agent.speed * modifier, MIN_MOVE_SPEED, MAX_MOVE_SPEED);
	}

	private void SetAgentSpeed(float speed)
	{
		agent.speed = speed;
	}

	private void Reset()
	{
		currentWaypointIndex = 0;
		currentWaypoint = null;
		target = originalWaypoints[currentWaypointIndex].transform;
		//agent.updateRotation = true;
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
			case EnemyPatrolState.INVESTIGATE:
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
	}
}