using OliverLoescher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
	public EnemyPatrolState CharacterState => characterState; // IFSM

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

	[Header("Audio")]
	[SerializeField]
	private AudioSourcePool audioSource = null;
	// TODO: Rename to "Monster Chase SFX"
	[SerializeField]
	private AudioUtil.AudioPiece monsterSFX = new AudioUtil.AudioPiece();
	// TODO: Add "Monster Alerted SFX"

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
	[SerializeField]
	private float minWaitTime = 3.0f;
	[SerializeField]
	private float maxWaitTime = 4.0f;
	[SerializeField]
	private float defaultViewDistance = 5f;
	[SerializeField]
	private float defaultViewAngle = 30f;
	[SerializeField]
	private float defaultViewHeight = 0.5f;
	[SerializeField]
	private float investigateViewDistance = 6f;
	[SerializeField]
	private float investigateViewAngle = 45f;
	// -1 Lowers the height - which we want for when the player is Crouching
	[SerializeField]
	private float investigateViewHeight = -1f;


	// TODO: Implement "Investigating" where the Enemy walks to the "Noise" spot or "Sprints" to the noise spot, depending on how "Aggresive" the enemy is
	private float currentParanoia = 0f;
	private CharacterController player = null;
	private int currentWaypointIndex = 0;
	private Waypoint currentWaypoint = null;
	private IFSM stateMachine = null;

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

	private void OnTriggerStay(Collider other)
	{
		if (characterState == EnemyPatrolState.CHASE && other.gameObject.TryGetComponent(out CharacterController controller))
		{
			SceneManager.LoadScene("GameOver");
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

		if (!agent.isStopped && agent.remainingDistance <= 0.1f)
		{
			OnWayPointReached();
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
		if (characterState == state)
		{
			return;
		}

		characterState = state;
		viewDetector.UpdateDetector(defaultViewAngle, defaultViewDistance, defaultViewHeight);

		switch (characterState)
		{
			case EnemyPatrolState.PATROL:
				Log("Patrolling...");
				Patrol();
				break;
			case EnemyPatrolState.INVESTIGATE:
				Log("Investigating...");
				StartCoroutine(InvestigateRoutine());
				break;
			case EnemyPatrolState.SEARCH:
				Log("Searching...");
				Search();
				break;
			case EnemyPatrolState.CHASE:
				Log("Chasing player!");
				monsterSFX.Play(audioSource.GetSource());
				StartCoroutine(ChaseRoutine());
				break;
			default:
				break;
		}
	}

	private IEnumerator ChaseRoutine()
	{
		agent.isStopped = true;
		float preChaseDelay = 0.5f;
		yield return new WaitForSeconds(preChaseDelay);
		agent.isStopped = false;
		Chase();
	}

	private void Chase()
	{
		Vector3 playerPosition = player.transform.position;
		if (Vector3.Distance(transform.position, playerPosition) > playerChaseDistance)
		{
			// When losing the player, "Search" for them by creating some new Waypoints around the Enemy so they patrol this new area instead
			player = null;
			// TODO: SWITCH TO SEARCH AND FIX SEARCHING!
			stateMachine.OnStateTransition(EnemyPatrolState.INVESTIGATE);
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

	private IEnumerator InvestigateRoutine()
	{

		viewDetector.UpdateDetector(investigateViewAngle, investigateViewDistance, investigateViewHeight);

		if (characterState == EnemyPatrolState.INVESTIGATE)
		{
			ModifyAgentSpeed(currentInvestigateSpeed);
			agent.SetDestination(target.position);

			float minDetectableDistance = 3.0f;
			target.TryGetComponent(out CharacterController characterController);
			// Move to the Investigation location and Chase the Player if we find them
			if (characterController != null)
			{
				while (Vector3.Distance(transform.position, target.position) > minDetectableDistance)
				{
					if (Vector3.Distance(transform.position, characterController.transform.position) < minDetectableDistance)
					{
						player = characterController;
						stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
						break;
					}
					yield return null;
				}
			}

			// Chase the Player if we find them *after* getting to the Target Location
			if (characterController != null && Vector3.Distance(transform.position, characterController.transform.position) < minDetectableDistance)
			{
				player = characterController;
				stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
			}
			// Return to Patrolling otherwise
			else
			{
				stateMachine.OnStateTransition(EnemyPatrolState.PATROL);
			}
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


	#region Event Handlers
	private void OnPlayerDetectedHandler(CharacterController playerController)
	{
		if (characterState == EnemyPatrolState.CHASE) return;
		player = playerController;
		stateMachine.OnStateTransition(EnemyPatrolState.CHASE);
	}

	private void OnSoundDetectedHandler(float volume, GameObject detectedObject)
	{
		if (characterState == EnemyPatrolState.CHASE || characterState == EnemyPatrolState.INVESTIGATE) return;
		target = detectedObject.transform;
		currentInvestigateSpeed = investigateBaseSpeed * volume;
		stateMachine.OnStateTransition(EnemyPatrolState.INVESTIGATE);
	}

	private void OnBecameParanoidHandler(CharacterController playerController)
	{
		if (characterState == EnemyPatrolState.CHASE || characterState == EnemyPatrolState.INVESTIGATE)
		{
			return;
		}

		int paranoia = 10;
		currentParanoia += paranoia;

		if (currentParanoia > PARANOID_LIMIT)
		{
			Debug.Log($"{this} became Paranoid");
			currentParanoia = 0;
			player = playerController;
			target = player.transform;
			stateMachine.OnStateTransition(EnemyPatrolState.INVESTIGATE);
		}
	}
	#endregion

	private IEnumerator MoveNextWaypoint()
	{
		float randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
		agent.isStopped = true;
		yield return new WaitForSeconds(randomWaitTime);

		agent.isStopped = false;
		NavMeshPath path = new NavMeshPath();
		if (!agent.SetDestination(target.position))
		{
			agent.SetDestination(transform.position);
			OnWayPointReached();
		}
	}

	private void OnWayPointReached()
	{
		if (characterState == EnemyPatrolState.PATROL && player == null)
		{
			Patrol();
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
		agent.isStopped = true;
		stateMachine = this;
		stateMachine.OnStateTransition(EnemyPatrolState.PATROL);
	}

	private void Log(string log)
	{
		// Debug.Log($"|{this}| {log}");
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