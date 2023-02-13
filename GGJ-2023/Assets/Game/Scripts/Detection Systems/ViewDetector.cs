using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ViewDetector : MonoBehaviour
{
	public Action<CharacterController> OnPlayerDetected;
	public Action<CharacterController> OnBecameParanoid;

	private List<GameObject> detectedObjects = new List<GameObject>();
	public List<GameObject> DetectedObjects => detectedObjects;

	[Header("Settings")]
	[SerializeField]
	private float distance = 10f;
	[SerializeField]
	private float angle = 30f;
	[SerializeField]
	private float height = 1.0f;
	[SerializeField]
	private Color meshDetectionColor = Color.red;
	[SerializeField]
	private int detectFrequency = 30;
	[SerializeField]
	private LayerMask detectionMask;
	[SerializeField]
	private LayerMask occlusionMask;
	[SerializeField]
	private float instantDetectionDistance = 1.0f;


	private Collider[] collidersBuffer = new Collider[50];
	private int hitCount = 0;
	private float detectInterval = 0f;
	private float detectTimer = 0f;

	private Mesh detectionMesh = null;

	private void OnEnable()
	{
		UpdateFrequency();
	}

	private void Update()
	{
		detectTimer -= Time.deltaTime;
		if (detectTimer <= 0f)
		{
			detectTimer += detectInterval;
			Detect();
		}
	}

	private void Detect()
	{
		hitCount = Physics.OverlapSphereNonAlloc(transform.position, distance, collidersBuffer, detectionMask, QueryTriggerInteraction.Collide);

		detectedObjects.Clear();
		for (int i = 0; i < hitCount; i++)
		{
			GameObject detecteObject = collidersBuffer[i].gameObject;
			if (IsDetected(detecteObject))
			{
				detectedObjects.Add(detecteObject);
			}
		}
	}

	public bool IsDetected(GameObject detectedObject)
	{
		Vector3 origin = transform.position;
		Vector3 destination = detectedObject.transform.position;
		Vector3 direction = destination - origin;

		//Debug.Log($"Direction Y: {direction.y}");
		// Check if the Object is Above/Below the Sensor
		/* ORIGINAL CODE!
		if (direction.y < 0 || direction.y > height)
		{
			return false;
		}
		*/

		detectedObject.TryGetComponent(out CharacterController player);

		// Check if the Object is within the ViewMesh
		direction.y = 0;
		float deltaAngle = Vector3.Angle(direction, transform.forward);
		if (deltaAngle > angle)
		{
			return false;
		}

		// Check if this Object is being blocked
		origin.y += height / 2;
		destination.y = origin.y;
		float crouchOffset = 0.5f;
		if (InputBridge_FirstPersonPlayer.Instance.Crouch.Input)
		{
			destination.y -= crouchOffset;
		}
		if (Physics.Linecast(origin, destination, occlusionMask))
		{
			return false;
		}

		// Check if the Player is standing or crouched behind an obstacle using the Character Controllers Center.Y
		if (player)
		{
			if (direction.z <= instantDetectionDistance)
			{
				OnPlayerDetected?.Invoke(player);
			}
			else
			{
				OnBecameParanoid?.Invoke(player);
			}

			return true;
		}
		else
		{
			return false;
		}
	}

	public void UpdateDetector(float angle, float distance, float height)
	{
		this.angle = angle;
		this.distance = distance;
		this.height = height;
		detectionMesh = CreateViewMesh();
	}

	private void UpdateFrequency()
	{
		detectInterval = 1.0f / detectFrequency;
	}

	private Mesh CreateViewMesh()
	{
		Mesh viewMesh = new Mesh();

		int segments = 10;
		int squareSides = 4;
		int leftRightSides = 2;
		int numTriangles = (segments * squareSides) + leftRightSides + leftRightSides;
		int triangleSides = 3;	// Using the amount of sides based on Triangles to calculate the amount of vert's we'll need
		int numVerticies = numTriangles * triangleSides;

		Vector3[] verticies = new Vector3[numVerticies];
		int[] triangles = new int[numVerticies];

		Vector3 bottomCenter = Vector3.zero;
		Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
		Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

		Vector3 topCenter = bottomCenter + (Vector3.up * height);
		Vector3 topLeft = bottomLeft + (Vector3.up * height);
		Vector3 topRight = bottomRight + (Vector3.up * height);

		int currentVert = 0;

		// Left
		verticies[currentVert++] = bottomCenter;
		verticies[currentVert++] = bottomLeft;
		verticies[currentVert++] = topLeft;

		verticies[currentVert++] = topLeft;
		verticies[currentVert++] = topCenter;
		verticies[currentVert++] = bottomCenter;

		// Right
		verticies[currentVert++] = bottomCenter;
		verticies[currentVert++] = topCenter;
		verticies[currentVert++] = topRight;

		verticies[currentVert++] = topRight;
		verticies[currentVert++] = bottomRight;
		verticies[currentVert++] = bottomCenter;

		// Angles
		float currentAngle = -angle;
		float deltaAngle = (angle * leftRightSides) / segments;

		for (int i = 0; i < segments; i++)
		{
			bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
			bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

			topLeft = bottomLeft + (Vector3.up * height);
			topRight = bottomRight + (Vector3.up * height);

			// Far
			verticies[currentVert++] = bottomLeft;
			verticies[currentVert++] = bottomRight;
			verticies[currentVert++] = topRight;

			verticies[currentVert++] = topRight;
			verticies[currentVert++] = topLeft;
			verticies[currentVert++] = bottomLeft;

			// Top
			verticies[currentVert++] = topCenter;
			verticies[currentVert++] = topLeft;
			verticies[currentVert++] = topRight;

			// Bottom
			verticies[currentVert++] = bottomCenter;
			verticies[currentVert++] = bottomRight;
			verticies[currentVert++] = bottomLeft;

			currentAngle += deltaAngle;
		}

		for (int i = 0; i < numVerticies; i++)
		{
			triangles[i] = i;
		}

		viewMesh.vertices = verticies;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();

		return viewMesh;
	}

	// Used for filtering all detected Objects
	public int FilterDetections(GameObject[] gameObjectBuffer, string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);
		int count = 0;

		foreach (GameObject filteredObject in detectedObjects)
		{
			if (filteredObject.layer == layer)
			{
				gameObjectBuffer[count++] = filteredObject;
			}

			if (gameObjectBuffer.Length == count)
			{
				// Buffer is full; break
				break;
			}
		}

		return count;
	}

	private void OnValidate()
	{
		detectionMesh = CreateViewMesh();
		UpdateFrequency();
	}

	private void OnDrawGizmos()
	{
		if (detectionMesh)
		{
			Gizmos.color = meshDetectionColor;
			Gizmos.DrawMesh(detectionMesh, transform.position, transform.rotation);
		}

		Gizmos.DrawWireSphere(transform.position, distance);
		float detectedSphereRadius = 1f;
		for (int i = 0; i < hitCount; i++)
		{
			Gizmos.DrawSphere(collidersBuffer[i].transform.position, detectedSphereRadius);
		}

		Gizmos.color = Color.green;
		foreach (GameObject detectedObject in detectedObjects)
		{
			Gizmos.DrawSphere(detectedObject.transform.position, detectedSphereRadius);

			// Vector3 origin = transform.position;
			// Vector3 destination = detectedObject.transform.position;
			// origin.y += height / 2;
			// destination.y = origin.y;
			// if (InputBridge_FirstPersonPlayer.Instance.Crouch.Input)
			// {
			// 	destination.y -= 1.0f;
			// }
			// Gizmos.DrawLine(origin, destination);
		}
	}
}
